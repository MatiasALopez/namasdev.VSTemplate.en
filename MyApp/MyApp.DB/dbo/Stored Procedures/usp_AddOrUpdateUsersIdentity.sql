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
