CREATE TABLE [dbo].[Members]
(
	[FamilyId] int not null, 
    [CustomerId] int not null
	CONSTRAINT [FK_Members_DboCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [Customer]([id]),
	CONSTRAINT [FK_Members_DboFamily] FOREIGN KEY ([FamilyId]) REFERENCES [Family]([id])
)
