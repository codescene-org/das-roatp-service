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

-- Set up lookup data for provider types

IF NOT EXISTS (SELECT 1 FROM dbo.ProviderTypes) 
BEGIN
	SET IDENTITY_INSERT dbo.ProviderTypes ON
	
	INSERT INTO dbo.ProviderTypes
	([Id], [ProviderType], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(1,
	'Main Provider', 'System', SYSDATETIME(), 'Live'),
	(2,
	'Employer Provider', 'System', SYSDATETIME(), 'Live'),
	(3,
	'Supporting Provider', 'System', SYSDATETIME(), 'Live')

	SET IDENTITY_INSERT dbo.ProviderTypes OFF
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

-- Map provider types to available organisation types

IF NOT EXISTS (SELECT 1 FROM dbo.ProviderTypeOrganisationTypes) 
BEGIN
	SET IDENTITY_INSERT dbo.ProviderTypeOrganisationTypes ON

	INSERT INTO dbo.ProviderTypeOrganisationTypes
	([Id], [ProviderTypeId], [OrganisationTypeId], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(1, 1, 0, 'System', SYSDATETIME(), 'Live'),
	(2, 1, 0, 'System', SYSDATETIME(), 'Live'),
	(3, 1, 0, 'System', SYSDATETIME(), 'Live')

	SET IDENTITY_INSERT dbo.ProviderTypeOrganisationTypes OFF
END

IF NOT EXISTS (SELECT 1 FROM dbo.RemovedReasons)
BEGIN
	SET IDENTITY_INSERT dbo.RemovedReasons ON
	
	INSERT INTO dbo.RemovedReasons
	([Id], [Status], [RemovedReason], [CreatedBy], [CreatedAt])
	VALUES
	(1, 'Live', 'Breach', 'System', SYSDATETIME()),
	(2, 'Live', 'Change of trading status', 'System', SYSDATETIME()),
	(3, 'Live', 'High risk policy', 'System', SYSDATETIME()),
	(4, 'Live', 'Inadequate financial health', 'System', SYSDATETIME()),
	(5, 'Live', 'Inadequate Ofsted grade', 'System', SYSDATETIME()),
	(6, 'Live', 'Internal error', 'System', SYSDATETIME()),
	(7, 'Live', 'Merger', 'System', SYSDATETIME()),
	(8, 'Live', 'Minimum standards not met', 'System', SYSDATETIME()),
	(9, 'Live', 'Non-direct delivery in 12 month period', 'System', SYSDATETIME()),
	(10, 'Live', 'Provider error', 'System', SYSDATETIME()),
	(11, 'Live', 'Provider request', 'System', SYSDATETIME()),
	(12, 'Live', 'Other', 'System', SYSDATETIME())

	SET IDENTITY_INSERT dbo.RemovedReasons OFF
	
END

IF NOT EXISTS (SELECT 1 FROM dbo.OrganisationStatus)
BEGIN
	SET IDENTITY_INSERT dbo.OrganisationStatus ON
	
	INSERT INTO dbo.OrganisationStatus
	([Id], [Status], [CreatedBy], [CreatedAt])
	VALUES
	(0, 'Removed', 'System', SYSDATETIME()),
	(1, 'Active', 'System', SYSDATETIME()),
	(2, 'Active - but not taking on apprentices', 'System', SYSDATETIME())

	SET IDENTITY_INSERT dbo.OrganisationStatus OFF
	
END

-- Drop old lookup tables

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES   
     WHERE TABLE_CATALOG = db_name() 
      AND TABLE_SCHEMA = 'dbo' 
      AND TABLE_NAME = 'InactiveReasons'))
BEGIN
   DROP TABLE dbo.InactiveReasons
END

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES   
     WHERE TABLE_CATALOG = db_name() 
      AND TABLE_SCHEMA = 'dbo' 
      AND TABLE_NAME = 'ApplicationRouteOrganisationTypes'))
BEGIN
   DROP TABLE dbo.ApplicationRouteOrganisationTypes
END

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES   
     WHERE TABLE_CATALOG = db_name() 
      AND TABLE_SCHEMA = 'dbo' 
      AND TABLE_NAME = 'ApplicationRoutes'))
BEGIN
   DROP TABLE dbo.ApplicationRoutes
END