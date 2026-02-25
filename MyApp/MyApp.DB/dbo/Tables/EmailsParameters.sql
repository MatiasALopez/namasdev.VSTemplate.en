create table [dbo].[EmailsParameters]
(
	[Id] smallint not null,
	[Subject] nvarchar(256) not null,
	[Content] nvarchar(max) not null,
	[CC] nvarchar(max) null, 

    constraint [PK_EmailsParameters] primary key clustered ([Id])
)
go
