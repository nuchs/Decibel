using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mast.Dbo;

public class Parameter
{
	public Parameter(ProcedureParameter parameter)
	{
		Name = parameter.VariableName.Value;
		IsNullable = parameter.Nullable is null || parameter.Nullable.Nullable;
		DataType = new DataType(parameter.DataType);
		if(parameter.Value is not null)
		{
			var dfltTokens = parameter.ScriptTokenStream
					.Take(parameter.Value.FirstTokenIndex..(parameter.Value.LastTokenIndex + 1))
					.Select(d => d.Text);
		Default = string.Join("", dfltTokens).Trim();
		}
	}

	public string Name { get; }
	public bool IsNullable;
	public DataType DataType { get; set; }

	public string Default = String.Empty;
}
