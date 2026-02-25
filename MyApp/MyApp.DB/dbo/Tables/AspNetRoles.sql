CREATE TABLE [dbo].[AspNetRoles] 
(
    [Id]   NVARCHAR (128) NOT NULL,
    [Name] NVARCHAR (256) NOT NULL,
    
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
)
go

CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetRoles_Name] ON [dbo].[AspNetRoles] ([Name] ASC)
GO