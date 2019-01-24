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
	(Id, [Route])
	VALUES
	(1,
	'Main Provider'),
	(2,
	'Employer Provider'),
	(3,
	'Supporting Provider')

	SET IDENTITY_INSERT dbo.ApplicationRoutes OFF
END

-- Set up lookup data for organisation types

IF NOT EXISTS (SELECT 1 FROM dbo.OrganisationTypes)
BEGIN
	SET IDENTITY_INSERT dbo.OrganisationTypes ON

	INSERT INTO dbo.OrganisationTypes
	(Id, [Status], [Type])
	VALUES
	(0, 'Live', 'Unassigned')
	
	SET IDENTITY_INSERT dbo.OrganisationTypes OFF

END

-- Map application routes to available organisation types

IF NOT EXISTS (SELECT 1 FROM dbo.ApplicationRouteOrganisationTypes) 
BEGIN
	SET IDENTITY_INSERT dbo.ApplicationRouteOrganisationTypes ON

	INSERT INTO dbo.ApplicationRouteOrganisationTypes
	(Id, ApplicationRouteId, OrganisationTypeId)
	VALUES
	(1, 1, 0),
	(2, 1, 0),
	(3, 1, 0)

	SET IDENTITY_INSERT dbo.ApplicationRouteOrganisationTypes OFF
END

IF NOT EXISTS (SELECT 1 FROM dbo.EndReasons)
BEGIN
	SET IDENTITY_INSERT dbo.EndReasons ON
	
	INSERT INTO dbo.EndReasons
	([Id], [Status], [EndReason])
	VALUES
	(1, 'Live', 'Administration'),
	(2, 'Live', 'Change in Academy Trust'),
	(3, 'Live', 'Change of entity'),
	(4, 'Live', 'Contract termination'),
	(5, 'Live', 'Dissolved'),
	(6, 'Live', 'High risk policy'),
	(7, 'Live', 'Inadequate financial health'),
	(8, 'Live', 'Insolvency'),
	(9, 'Live', 'Internal error'),
	(10, 'Live', 'Liquidation'),
	(11, 'Live', 'Merger'),
	(12, 'Live', 'Minimum Standards'),
	(13, 'Live', 'Ofsted Grade 4 (Inadequate)'),
	(14, 'Live', 'Serious breach'),
	(15, 'Live', 'Unable to provide response(s)'),
	(16, 'Live', 'Voluntary removal'),
	(17, 'Live', 'Winding up proceedings')

	SET IDENTITY_INSERT dbo.EndReasons OFF
	
END