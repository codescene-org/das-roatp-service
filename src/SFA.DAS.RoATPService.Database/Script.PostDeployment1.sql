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

IF NOT EXISTS (SELECT 1 FROM dbo.EndReasons)
BEGIN
	SET IDENTITY_INSERT dbo.EndReasons ON
	
	INSERT INTO dbo.EndReasons
	([Id], [Status], [EndReason], [CreatedBy], [CreatedAt])
	VALUES
	(1, 'Live', 'Administration', 'System', SYSDATETIME()),
	(2, 'Live', 'Change in Academy Trust', 'System', SYSDATETIME()),
	(3, 'Live', 'Change of entity', 'System', SYSDATETIME()),
	(4, 'Live', 'Contract termination', 'System', SYSDATETIME()),
	(5, 'Live', 'Dissolved', 'System', SYSDATETIME()),
	(6, 'Live', 'High risk policy', 'System', SYSDATETIME()),
	(7, 'Live', 'Inadequate financial health', 'System', SYSDATETIME()),
	(8, 'Live', 'Insolvency', 'System', SYSDATETIME()),
	(9, 'Live', 'Internal error', 'System', SYSDATETIME()),
	(10, 'Live', 'Liquidation', 'System', SYSDATETIME()),
	(11, 'Live', 'Merger', 'System', SYSDATETIME()),
	(12, 'Live', 'Minimum Standards', 'System', SYSDATETIME()),
	(13, 'Live', 'Ofsted Grade 4 (Inadequate)', 'System', SYSDATETIME()),
	(14, 'Live', 'Serious breach', 'System', SYSDATETIME()),
	(15, 'Live', 'Unable to provide response(s)', 'System', SYSDATETIME()),
	(16, 'Live', 'Voluntary removal', 'System', SYSDATETIME()),
	(17, 'Live', 'Winding up proceedings', 'System', SYSDATETIME())

	SET IDENTITY_INSERT dbo.EndReasons OFF
	
END