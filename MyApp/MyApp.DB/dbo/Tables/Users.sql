create table [dbo].[Users]
(
	[UserId] nvarchar(128) NOT NULL,
	[Email] nvarchar(100) NOT NULL,
	[FirstName] nvarchar(100) NOT NULL,
	[LastName] nvarchar(100) NOT NULL,
	[FullName] AS CAST(concat([FirstName],' ',[LastName]) AS nvarchar(200)), 
	[FullNameInverted] AS CAST(concat([LastName],' ',[FirstName]) AS nvarchar(200)), 
	[CreatedBy] nvarchar(128) NOT NULL,
	[CreatedAt] datetime NOT NULL,
	[ModifiedBy] nvarchar(128) NOT NULL,
	[ModifiedAt] datetime NOT NULL,
	[DeletedBy] nvarchar(128) NULL,
	[DeletedAt] datetime NULL,
	[Deleted] AS (ISNULL(CONVERT(bit,CASE WHEN [DeletedAt] IS NULL THEN 0 ELSE 1 END), 0)),

	constraint [PK_Users] primary key clustered ([UserId])
)
go

create unique nonclustered index IX_Users_Email on dbo.Users (Email)
go
