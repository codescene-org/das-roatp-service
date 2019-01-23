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
	(0, 'Live', 'Unassigned'),
	(1, 'Live', 'Academy'),
	(2, 'Live', 'Central government department, executive agency or Non-Departmental Public Body'),
	(3, 'Live', 'Delivery organisation connected to an Apprenticeship Training Agency (ATA)'),
	(4, 'Live', 'Employer training apprentices within their Connected Companies'),
	(5, 'Live', 'Employer training their own staff and those within other organisations'),
	(6, 'Live', 'Further Education Institution'),
	(7, 'Live', 'General Further Education College (GFE)'),
	(8, 'Live', 'Group Training Association (GTA)'),
	(9, 'Live', 'Higher Education Institution (HEI)'),
	(10, 'Live', 'Independent Training Provider'),
	(11, 'Live', 'Local Authority, including LEA schools'),
	(12, 'Live', 'Multi-Academy Trust'),
	(13, 'Live', 'National College'),
	(14, 'Live', 'NHS Trust or Fire Authority'),
	(15, 'Live', 'Police Constabulary or Police and Crime Commissioner'),
	(16, 'Live', 'Sixth Form College'),
	(17, 'Live', 'Employer training apprentices in own organisation'),
	(18, 'Live', 'Employer training apprentices within their connected companies'),
	(19, 'Live', 'Employer training apprentices in own organisation and those in their connected companies'),
	(20, 'Live', 'None of the above')
	
	SET IDENTITY_INSERT dbo.OrganisationTypes OFF

END

-- Map application routes to available organisation types

IF NOT EXISTS (SELECT 1 FROM dbo.ApplicationRouteOrganisationTypes) 
BEGIN
	SET IDENTITY_INSERT dbo.ApplicationRouteOrganisationTypes ON

	INSERT INTO dbo.ApplicationRouteOrganisationTypes
	(Id, ApplicationRouteId, OrganisationTypeId)
	VALUES
	(1, 1, 1),
	(2, 1, 2),
	(3, 1, 3),
	(4, 1, 4),
	(5, 1, 5),
	(6, 1, 6),
	(7, 1, 7),
	(8, 1, 8),
	(9, 1, 9),
	(10, 1, 10),
	(11, 1, 11),
	(12, 1, 12),
	(13, 1, 13),
	(14, 1, 14),
	(15, 1, 15),
	(16, 1, 16),
	(17, 1, 20),
	(18, 3, 1),
	(19, 3, 2),
	(20, 3, 3),
	(21, 3, 4),
	(22, 3, 5),
	(23, 3, 6),
	(24, 3, 7),
	(25, 3, 8),
	(26, 3, 9),
	(27, 3, 10),
	(28, 3, 11),
	(29, 3, 12),
	(30, 3, 13),
	(31, 3, 14),
	(32, 3, 15),
	(33, 3, 16),
	(34, 3, 20),
	(35, 2, 17),
	(36, 2, 18),
	(37, 2, 19),
	(38, 1, 0),
	(39, 1, 0),
	(40, 1, 0)

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