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
