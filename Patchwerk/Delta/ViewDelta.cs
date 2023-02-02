using Mast;
using Mast.Dbo;
using System.Globalization;

namespace Patchwerk.Delta;

internal sealed class ViewDelta : DboDelta<View>
{
    public ViewDelta()
        : base("View")
    {
    }

    /* Views can reference others views, to ensure the patches are able to be
       applied correctly referred to views must be patched before the views
       that refer to them.
    
       More formally: If view A is referenced by view B then we can say that it 
       precedes view B. If neither A nor B refer to each other than we can say
       there is no relation between them. Together these two statements define
       a partial order for the views. 

       Kahn's algorithm can be used to sort sets with a partial order
    */
    internal override IEnumerable<string> GenerateAddsAndUpdates(IDatabase before, IDatabase after)
    {
        Kahn<FullyQualifiedName, string> sorter = new();

        foreach (var view in after.Views)
        {
            var candidate = before.Views.FirstOrDefault(v => v.Identifier == view.Identifier);

            if (candidate is null)
            {
                sorter.AddNode(view.Identifier, view.Content, view.ReferencedBy.Select(r => r.Identifier));
            }
            else if(candidate != view)
            {
                sorter.AddNode(
                    view.Identifier, 
                    view.Content.Replace("CREATE", "ALTER", ignoreCase: true, CultureInfo.InvariantCulture), 
                    view.ReferencedBy.Select(r => r.Identifier));
            }
        }

        return sorter.Sort();
    }


    protected override IEnumerable<DbObject> Selector(IDatabase db)
        => db.Views;

    private record Edge(FullyQualifiedName From, FullyQualifiedName To);

    private record Node(FullyQualifiedName Id, string Content);
}
