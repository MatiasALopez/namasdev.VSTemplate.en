CREATE TABLE [dbo].[Errors]
(
	[Id] uniqueidentifier NOT NULL,
	[Message] nvarchar(max) NOT NULL,
	[StackTrace] nvarchar(max) NOT NULL,
	[Source] nvarchar(200) NOT NULL,
	[Arguments] nvarchar(max) NULL,
	[DateTime] datetime NOT NULL,
	[UserId] nvarchar(128) NULL,
 
	CONSTRAINT [PK_Errors] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
