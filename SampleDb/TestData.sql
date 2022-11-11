insert into dbo.Customer (Name, Enroled) values ('Daddy', GETUTCDATE())
insert into dbo.Customer (Name, Enroled) values ('Mummy', GETUTCDATE())
insert into dbo.Customer (Name, Enroled) values ('Peppa', GETUTCDATE())
insert into dbo.Customer (Name, Enroled) values ('George', GETUTCDATE())

insert into dbo.Customer (Name, Enroled) values ('Homer', GETUTCDATE())
insert into dbo.Customer (Name, Enroled) values ('Marge', GETUTCDATE())
insert into dbo.Customer (Name, Enroled) values ('Bart', GETUTCDATE())
insert into dbo.Customer (Name, Enroled) values ('Lisa', GETUTCDATE())
insert into dbo.Customer (Name, Enroled) values ('Maggie', GETUTCDATE())

insert into dbo.Family (Surname) values ('Pig')
insert into dbo.Family (Surname) values ('Simpson')

insert into dbo.Members 
select c.Id, f.Id 
from dbo.Customer c, dbo.Family f
where c.Name in ('Daddy', 'Mummy','Peppa','George')
and f.Surname = 'Pig'

insert into dbo.Members 
select c.Id, f.Id 
from dbo.Customer c, dbo.Family f
where c.Name in ('Homer', 'Marge','Bart','Lisa', 'Maggie')
and f.Surname = 'Simpson'