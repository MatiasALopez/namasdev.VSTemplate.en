CREATE TABLE [dbo].[Parameters]
(
	[Name] nvarchar(100) not null,
	[Value] nvarchar(max) null,

	constraint [PK_Parameters] primary key clustered ([Name])
)
go
