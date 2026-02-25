--====
CREATE TABLE [dbo].[AspNetUsers] 
(
    [Id]                   NVARCHAR (128) NOT NULL,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    [UserName]             NVARCHAR (256) NOT NULL,
    
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers_UserName] ON [dbo].[AspNetUsers] ([UserName] ASC)
go
--====


--====
CREATE TABLE [dbo].[AspNetRoles] 
(
    [Id]   NVARCHAR (128) NOT NULL,
    [Name] NVARCHAR (256) NOT NULL,
    
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
)
go

CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetRoles_Name] ON [dbo].[AspNetRoles] ([Name] ASC)
GO
--====


--====
CREATE TABLE [dbo].[AspNetUserLogins] 
(
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC, [UserId] ASC),
    CONSTRAINT [FK_AspNetUserLogins_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins] ([UserId] ASC)
GO
--====


--====
CREATE TABLE [dbo].[AspNetUserClaims] 
(
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (128) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUserClaims_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims] ([UserId] ASC)
GO
--====


--====
CREATE TABLE [dbo].[AspNetUserRoles] 
(
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL,
    
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AspNetUserRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles] ([RoleId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_UserId] ON [dbo].[AspNetUserRoles] ([UserId] ASC)
GO
--====


--====
CREATE TABLE [dbo].[Parameters]
(
	[Name] nvarchar(100) not null,
	[Value] nvarchar(max) null,

	constraint [PK_Parameters] primary key clustered ([Name])
)
go
--====


--====
create table [dbo].[EmailsParameters]
(
	[Id] smallint not null,
	[Subject] nvarchar(256) not null,
	[Content] nvarchar(max) not null,
	[CC] nvarchar(max) null, 

    constraint [PK_EmailsParameters] primary key clustered ([Id])
)
go
--====

--===
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
--===


--===
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
--===

--===
go
CREATE PROCEDURE [dbo].[usp_AddOrUpdateUsersIdentity]
	@UserId nvarchar(128) = null,
	@RoleName nvarchar(100) = null
AS
	set nocount on;

	-- Lock inactive
	update dbo.AspNetUsers 
	set LockoutEndDateUtc = '2100-1-1'
	where 
		Id in (select UserId from dbo.Users where Deleted = 1)
		and (@UserId is null or Id = @UserId)

	-- Update active
	update mu
	set UserName = u.Email,
		Email = u.Email
	from 
		dbo.AspNetUsers mu
		join dbo.Users u on mu.Id = u.UserId
	where 
		u.Deleted = 0
		and (@UserId is null or mu.Id = @UserId)
		
	-- Add new
	insert into dbo.AspNetUsers (Id,UserName,Email,SecurityStamp,EmailConfirmed,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount)
		select u.UserId,u.Email,u.Email,NEWID(),0,0,0,1,0
		from dbo.Users u
		where 
			u.Deleted = 0
			and not exists(select * from dbo.AspNetUsers where Id = u.UserId)
			and (@UserId is null or u.UserId = @UserId)
		
	-- Add/Delete User Roles
	if @RoleName is not null
	begin
		delete dbo.AspNetUserRoles
		where 
			UserId in (select UserId from dbo.Users)
			and (@UserId is null or UserId = @UserId)

		declare @RoleId nvarchar(128) = (select top 1 Id from dbo.AspNetRoles where Name = @RoleName)
		insert into dbo.AspNetUserRoles (UserId,RoleId)
			select UserId,@RoleId
			from dbo.Users
			where
				Deleted = 0
				and exists(select * from dbo.AspNetUsers where Id = UserId)
				and (@UserId is null or UserId = @UserId)
	end

	RETURN 0
GO
--===

--===
GO
CREATE FUNCTION [dbo].[uf_GetIdentityUserRoles]
(	
	@UserId nvarchar(128)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT r.Id, r.Name
	FROM 
		dbo.AspNetUserRoles ur
		join dbo.AspNetRoles r on ur.RoleId = r.Id
	WHERE
		ur.UserId = @UserId
)
GO
--===
