/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- Set up lookup data for application routes

IF NOT EXISTS (SELECT 1 FROM dbo.ApplicationRoutes) 
BEGIN
	SET IDENTITY_INSERT dbo.ApplicationRoutes ON
	
	INSERT INTO dbo.ApplicationRoutes
	([Id], [Route], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(1,
	'Main Provider', 'System', SYSDATETIME(), 'Live'),
	(2,
	'Employer Provider', 'System', SYSDATETIME(), 'Live'),
	(3,
	'Supporting Provider', 'System', SYSDATETIME(), 'Live')

	SET IDENTITY_INSERT dbo.ApplicationRoutes OFF
END

-- Set up lookup data for organisation types

IF NOT EXISTS (SELECT 1 FROM dbo.OrganisationTypes)
BEGIN
	SET IDENTITY_INSERT dbo.OrganisationTypes ON

	INSERT INTO dbo.OrganisationTypes
	([Id], [Type], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(0, 'Unassigned', 'System', SYSDATETIME(), 'Live')
	
	SET IDENTITY_INSERT dbo.OrganisationTypes OFF

END

-- Map application routes to available organisation types

IF NOT EXISTS (SELECT 1 FROM dbo.ApplicationRouteOrganisationTypes) 
BEGIN
	SET IDENTITY_INSERT dbo.ApplicationRouteOrganisationTypes ON

	INSERT INTO dbo.ApplicationRouteOrganisationTypes
	([Id], [ApplicationRouteId], [OrganisationTypeId], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(1, 1, 0, 'System', SYSDATETIME(), 'Live'),
	(2, 1, 0, 'System', SYSDATETIME(), 'Live'),
	(3, 1, 0, 'System', SYSDATETIME(), 'Live')

	SET IDENTITY_INSERT dbo.ApplicationRouteOrganisationTypes OFF
END

IF NOT EXISTS (SELECT 1 FROM dbo.InactiveReasons)
BEGIN
	SET IDENTITY_INSERT dbo.InactiveReasons ON
	
	INSERT INTO dbo.InactiveReasons
	([Id], [Status], [EndReason], [CreatedBy], [CreatedAt])
	VALUES
	(1, 'Live', 'Breach', 'System', SYSDATETIME()),
	(2, 'Live', 'Change of trading status', 'System', SYSDATETIME()),
	(3, 'Live', 'High risk policy', 'System', SYSDATETIME()),
	(4, 'Live', 'Inadequate financial health', 'System', SYSDATETIME()),
	(5, 'Live', 'Inadequate Ofsted grade', 'System', SYSDATETIME()),
	(6, 'Live', 'Internal error', 'System', SYSDATETIME()),
	(7, 'Live', 'Merger', 'System', SYSDATETIME()),
	(8, 'Live', 'Minimum standards not met', 'System', SYSDATETIME()),
	(9, 'Live', 'Provider error', 'System', SYSDATETIME()),
	(10, 'Live', 'Provider request', 'System', SYSDATETIME()),
	(11, 'Live', 'Other', 'System', SYSDATETIME())

	SET IDENTITY_INSERT dbo.InactiveReasons OFF
	
END