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
	'Main provider', 'System', SYSDATETIME(), 'Live'),
	(2,
	'Employer provider', 'System', SYSDATETIME(), 'Live'),
	(3,
	'Supporting provider', 'System', SYSDATETIME(), 'Live')

	SET IDENTITY_INSERT dbo.ProviderTypes OFF
END
ELSE
BEGIN
	UPDATE dbo.ProviderTypes
	SET [ProviderType] = 'Main provider'
	WHERE Id = 1
	UPDATE dbo.ProviderTypes
	SET [ProviderType] = 'Employer provider'
	WHERE Id = 2
	UPDATE dbo.ProviderTypes
	SET [ProviderType] = 'Supporting provider'
	WHERE Id = 3
END

-- Set up lookup data for organisation types

IF NOT EXISTS (SELECT 1 FROM dbo.OrganisationTypes) 
BEGIN
	SET IDENTITY_INSERT dbo.OrganisationTypes ON

	INSERT INTO dbo.OrganisationTypes
	([Id], [Type], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(0, 'Unassigned', 'System', SYSDATETIME(), 'Live'),
	(1, 'School', 'System', SYSDATETIME(), 'Live'),
	(2, 'GFE college', 'System', SYSDATETIME(), 'Live'),
	(3, 'National college', 'System', SYSDATETIME(), 'Live'),
	(4, 'Sixth form college', 'System', SYSDATETIME(), 'Live'),
	(5, 'Further education institution', 'System', SYSDATETIME(), 'Live'),
	(6, 'Higher education institution', 'System', SYSDATETIME(), 'Live'),
	(7, 'Academy', 'System', SYSDATETIME(), 'Live'),
	(8, 'Multi-academy trust', 'System', SYSDATETIME(), 'Live'),
	(9, 'NHS Trust', 'System', SYSDATETIME(), 'Live'),
	(10, 'Police', 'System', SYSDATETIME(), 'Live'),
	(11, 'Fire authority', 'System', SYSDATETIME(), 'Live'),
	(12, 'Local authority', 'System', SYSDATETIME(), 'Live'),
	(13, 'Government department', 'System', SYSDATETIME(), 'Live'),
	(14, 'Non-departmental public body', 'System', SYSDATETIME(), 'Live'),
	(15, 'Execute agency', 'System', SYSDATETIME(), 'Live'),
	(16, 'Independent training provider', 'System', SYSDATETIME(), 'Live'),
	(17, 'Apprenticeship training agency', 'System', SYSDATETIME(), 'Live'),
	(18, 'Group training association', 'System', SYSDATETIME(), 'Live'),
	(19, 'Employer', 'System', SYSDATETIME(), 'Live'),
	(20, 'Other employer', 'System', SYSDATETIME(), 'Live')

	SET IDENTITY_INSERT dbo.OrganisationTypes OFF
END
ELSE
BEGIN
	DELETE FROM dbo.ProviderTypeOrganisationTypes
	WHERE OrganisationTypeId NOT IN (0)

	DELETE FROM dbo.OrganisationTypes
	WHERE Id NOT IN (0)

	SET IDENTITY_INSERT dbo.OrganisationTypes ON

	INSERT INTO dbo.OrganisationTypes
	([Id], [Type], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(1, 'School', 'System', SYSDATETIME(), 'Live'),
	(2, 'GFE college', 'System', SYSDATETIME(), 'Live'),
	(3, 'National college', 'System', SYSDATETIME(), 'Live'),
	(4, 'Sixth form college', 'System', SYSDATETIME(), 'Live'),
	(5, 'Further education institution', 'System', SYSDATETIME(), 'Live'),
	(6, 'Higher education institution', 'System', SYSDATETIME(), 'Live'),
	(7, 'Academy', 'System', SYSDATETIME(), 'Live'),
	(8, 'Multi-academy trust', 'System', SYSDATETIME(), 'Live'),
	(9, 'NHS Trust', 'System', SYSDATETIME(), 'Live'),
	(10, 'Police', 'System', SYSDATETIME(), 'Live'),
	(11, 'Fire authority', 'System', SYSDATETIME(), 'Live'),
	(12, 'Local authority', 'System', SYSDATETIME(), 'Live'),
	(13, 'Government department', 'System', SYSDATETIME(), 'Live'),
	(14, 'Non-departmental public body', 'System', SYSDATETIME(), 'Live'),
	(15, 'Executive agency', 'System', SYSDATETIME(), 'Live'),
	(16, 'Independent training provider', 'System', SYSDATETIME(), 'Live'),
	(17, 'Apprenticeship training agency', 'System', SYSDATETIME(), 'Live'),
	(18, 'Group training association', 'System', SYSDATETIME(), 'Live'),
	(19, 'Employer', 'System', SYSDATETIME(), 'Live'),
	(20, 'Other employer', 'System', SYSDATETIME(), 'Live')

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
	(2, 2, 0, 'System', SYSDATETIME(), 'Live'),
	(3, 3, 0, 'System', SYSDATETIME(), 'Live'),
	(4, 1, 1, 'System', SYSDATETIME(), 'Live'),
	(5, 3, 1, 'System', SYSDATETIME(), 'Live'),
	(6, 1, 2, 'System', SYSDATETIME(), 'Live'),
	(7, 3, 2, 'System', SYSDATETIME(), 'Live'),
	(8, 1, 3, 'System', SYSDATETIME(), 'Live'),
	(9, 3, 3, 'System', SYSDATETIME(), 'Live'),
	(10, 1, 4, 'System', SYSDATETIME(), 'Live'),
	(11, 3, 4, 'System', SYSDATETIME(), 'Live'),
	(12, 1, 5, 'System', SYSDATETIME(), 'Live'),
	(13, 3, 5, 'System', SYSDATETIME(), 'Live'),
	(14, 1, 6, 'System', SYSDATETIME(), 'Live'),
	(15, 3, 6, 'System', SYSDATETIME(), 'Live'),
	(16, 1, 7, 'System', SYSDATETIME(), 'Live'),
	(17, 3, 7, 'System', SYSDATETIME(), 'Live'),
	(18, 1, 8, 'System', SYSDATETIME(), 'Live'),
	(19, 3, 8, 'System', SYSDATETIME(), 'Live'),
	(20, 1, 9, 'System', SYSDATETIME(), 'Live'),
	(21, 3, 9, 'System', SYSDATETIME(), 'Live'),
	(22, 1, 10, 'System', SYSDATETIME(), 'Live'),
	(23, 3, 10, 'System', SYSDATETIME(), 'Live'),
	(24, 1, 11, 'System', SYSDATETIME(), 'Live'),
	(25, 3, 11, 'System', SYSDATETIME(), 'Live'),
	(26, 1, 12, 'System', SYSDATETIME(), 'Live'),
	(27, 3, 12, 'System', SYSDATETIME(), 'Live'),
	(28, 1, 13, 'System', SYSDATETIME(), 'Live'),
	(29, 3, 13, 'System', SYSDATETIME(), 'Live'),
	(30, 1, 14, 'System', SYSDATETIME(), 'Live'),
	(31, 3, 14, 'System', SYSDATETIME(), 'Live'),
	(32, 1, 15, 'System', SYSDATETIME(), 'Live'),
	(33, 3, 15, 'System', SYSDATETIME(), 'Live'),
	(34, 1, 16, 'System', SYSDATETIME(), 'Live'),
	(35, 3, 16, 'System', SYSDATETIME(), 'Live'),
	(36, 1, 17, 'System', SYSDATETIME(), 'Live'),
	(37, 3, 17, 'System', SYSDATETIME(), 'Live'),
	(38, 1, 18, 'System', SYSDATETIME(), 'Live'),
	(39, 3, 18, 'System', SYSDATETIME(), 'Live'),
	(40, 1, 19, 'System', SYSDATETIME(), 'Live'),
	(41, 3, 19, 'System', SYSDATETIME(), 'Live'),
	(42, 2, 1, 'System', SYSDATETIME(), 'Live'),
	(43, 2, 2, 'System', SYSDATETIME(), 'Live'),
	(44, 2, 3, 'System', SYSDATETIME(), 'Live'),
	(45, 2, 4, 'System', SYSDATETIME(), 'Live'),
	(46, 2, 5, 'System', SYSDATETIME(), 'Live'),
	(47, 2, 6, 'System', SYSDATETIME(), 'Live'),
	(48, 2, 7, 'System', SYSDATETIME(), 'Live'),
	(49, 2, 8, 'System', SYSDATETIME(), 'Live'),
	(50, 2, 9, 'System', SYSDATETIME(), 'Live'),
	(51, 2, 10, 'System', SYSDATETIME(), 'Live'),
	(52, 2, 11, 'System', SYSDATETIME(), 'Live'),
	(53, 2, 12, 'System', SYSDATETIME(), 'Live'),
	(54, 2, 13, 'System', SYSDATETIME(), 'Live'),
	(55, 2, 14, 'System', SYSDATETIME(), 'Live'),
	(56, 2, 15, 'System', SYSDATETIME(), 'Live'),
	(57, 2, 20, 'System', SYSDATETIME(), 'Live')

	SET IDENTITY_INSERT dbo.ProviderTypeOrganisationTypes OFF
END
ELSE 
BEGIN 
	SET IDENTITY_INSERT dbo.ProviderTypeOrganisationTypes ON

	INSERT INTO dbo.ProviderTypeOrganisationTypes
	([Id], [ProviderTypeId], [OrganisationTypeId], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(4, 1, 1, 'System', SYSDATETIME(), 'Live'),
	(5, 3, 1, 'System', SYSDATETIME(), 'Live'),
	(6, 1, 2, 'System', SYSDATETIME(), 'Live'),
	(7, 3, 2, 'System', SYSDATETIME(), 'Live'),
	(8, 1, 3, 'System', SYSDATETIME(), 'Live'),
	(9, 3, 3, 'System', SYSDATETIME(), 'Live'),
	(10, 1, 4, 'System', SYSDATETIME(), 'Live'),
	(11, 3, 4, 'System', SYSDATETIME(), 'Live'),
	(12, 1, 5, 'System', SYSDATETIME(), 'Live'),
	(13, 3, 5, 'System', SYSDATETIME(), 'Live'),
	(14, 1, 6, 'System', SYSDATETIME(), 'Live'),
	(15, 3, 6, 'System', SYSDATETIME(), 'Live'),
	(16, 1, 7, 'System', SYSDATETIME(), 'Live'),
	(17, 3, 7, 'System', SYSDATETIME(), 'Live'),
	(18, 1, 8, 'System', SYSDATETIME(), 'Live'),
	(19, 3, 8, 'System', SYSDATETIME(), 'Live'),
	(20, 1, 9, 'System', SYSDATETIME(), 'Live'),
	(21, 3, 9, 'System', SYSDATETIME(), 'Live'),
	(22, 1, 10, 'System', SYSDATETIME(), 'Live'),
	(23, 3, 10, 'System', SYSDATETIME(), 'Live'),
	(24, 1, 11, 'System', SYSDATETIME(), 'Live'),
	(25, 3, 11, 'System', SYSDATETIME(), 'Live'),
	(26, 1, 12, 'System', SYSDATETIME(), 'Live'),
	(27, 3, 12, 'System', SYSDATETIME(), 'Live'),
	(28, 1, 13, 'System', SYSDATETIME(), 'Live'),
	(29, 3, 13, 'System', SYSDATETIME(), 'Live'),
	(30, 1, 14, 'System', SYSDATETIME(), 'Live'),
	(31, 3, 14, 'System', SYSDATETIME(), 'Live'),
	(32, 1, 15, 'System', SYSDATETIME(), 'Live'),
	(33, 3, 15, 'System', SYSDATETIME(), 'Live'),
	(34, 1, 16, 'System', SYSDATETIME(), 'Live'),
	(35, 3, 16, 'System', SYSDATETIME(), 'Live'),
	(36, 1, 17, 'System', SYSDATETIME(), 'Live'),
	(37, 3, 17, 'System', SYSDATETIME(), 'Live'),
	(38, 1, 18, 'System', SYSDATETIME(), 'Live'),
	(39, 3, 18, 'System', SYSDATETIME(), 'Live'),
	(40, 1, 19, 'System', SYSDATETIME(), 'Live'),
	(41, 3, 19, 'System', SYSDATETIME(), 'Live'),
	(42, 2, 1, 'System', SYSDATETIME(), 'Live'),
	(43, 2, 2, 'System', SYSDATETIME(), 'Live'),
	(44, 2, 3, 'System', SYSDATETIME(), 'Live'),
	(45, 2, 4, 'System', SYSDATETIME(), 'Live'),
	(46, 2, 5, 'System', SYSDATETIME(), 'Live'),
	(47, 2, 6, 'System', SYSDATETIME(), 'Live'),
	(48, 2, 7, 'System', SYSDATETIME(), 'Live'),
	(49, 2, 8, 'System', SYSDATETIME(), 'Live'),
	(50, 2, 9, 'System', SYSDATETIME(), 'Live'),
	(51, 2, 10, 'System', SYSDATETIME(), 'Live'),
	(52, 2, 11, 'System', SYSDATETIME(), 'Live'),
	(53, 2, 12, 'System', SYSDATETIME(), 'Live'),
	(54, 2, 13, 'System', SYSDATETIME(), 'Live'),
	(55, 2, 14, 'System', SYSDATETIME(), 'Live'),
	(56, 2, 15, 'System', SYSDATETIME(), 'Live'),
	(57, 2, 20, 'System', SYSDATETIME(), 'Live')

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




 -- STORY APR-388  April 2019
IF NOT EXISTS (SELECT 1 FROM dbo.organisationStatus WHERE id = 3) 
	BEGIN
		SET IDENTITY_INSERT dbo.[OrganisationStatus] ON;
		INSERT INTO [dbo].[OrganisationStatus]
			   ([ID]
			   ,[Status]
			   ,[CreatedAt]
			   ,[CreatedBy])
		 VALUES
			   (3,
			   'On-boarding'
			   ,getdate()
			   ,'System')
		set identity_insert dbo.[OrganisationStatus] OFF;
			
	END
	
-- STORY APR-390 April 2019


IF NOT EXISTS (SELECT 1 FROM dbo.[ProviderTypeOrganisationStatus]) 
BEGIN
	SET IDENTITY_INSERT dbo.[ProviderTypeOrganisationStatus] ON

	INSERT INTO dbo.[ProviderTypeOrganisationStatus]
	([Id], [ProviderTypeId], [OrganisationStatusId], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(1, 1, 0, 'System', SYSDATETIME(), 'Live'),
	(2, 2, 0, 'System', SYSDATETIME(), 'Live'),
	(3, 3, 0, 'System', SYSDATETIME(), 'Live'),
	(4, 1, 1, 'System', SYSDATETIME(), 'Live'),
	(5, 2, 1, 'System', SYSDATETIME(), 'Live'),
	(6, 3, 1, 'System', SYSDATETIME(), 'Live'),
	(7, 1, 2, 'System', SYSDATETIME(), 'Live'),
	(8, 2, 2, 'System', SYSDATETIME(), 'Live'),
	(9, 3, 2, 'System', SYSDATETIME(), 'Live'),
	(10, 1, 3, 'System', SYSDATETIME(), 'Live'),
	(11, 2, 3, 'System', SYSDATETIME(), 'Live')
	SET IDENTITY_INSERT dbo.[ProviderTypeOrganisationStatus] OFF
END

UPDATE dbo.ProviderTypes
SET Description = 'Your organisation can train apprentices for other organisations, its own employees, employees of connected organisations or act as a subcontractor for other main and employer providers.'
WHERE Id = 1

UPDATE dbo.ProviderTypes
SET Description = 'Your organisation can train its own employees, employees of connected organisations or act as a subcontractor for other employer or main providers.'
WHERE Id = 2

UPDATE dbo.ProviderTypes
SET Description = 'Your organisation will act as a subcontractor for main and employer providers to train apprentices up to a maximum of £500,000 per year. If your organisation is new on the register, this will be limited to £100,000 in its first year.'
WHERE Id = 3

-- APR-474 update any non-set SourceIsUKRLP to true for Organisations.OrganisationData
update organisations set OrganisationData = JSON_Modify(OrganisationData,'$.SourceIsUKRLP',CAST(1 as BIT)) where JSON_VALUE(OrganisationData,'$.SourceIsUKRLP') is NULL




-- APR-638 adding categories to types --------
  
  SET IDENTITY_INSERT [OrganisationCategory] ON;
  if (not exists(select * from organisationCategory where id = 1))
	  insert into [OrganisationCategory] (Id,Category,CreatedAt,CreatedBy,Status) VALUES (1,'Educational Institute',getdate(),'System','Live');
  
   if (not exists(select * from organisationCategory where id = 2))
	  insert into [OrganisationCategory] (Id,Category,CreatedAt,CreatedBy,Status) VALUES (2,'Public Sector Body',getdate(),'System','Live');
   
SET IDENTITY_INSERT [OrganisationCategory] OFF;

if (not exists (select * from OrganisationCategoryType where OrganisationCategoryId = 1 and OrganisationTypeId = 1 ))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (1,1,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 1 and OrganisationTypeId = 2))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (1,2,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 1 and OrganisationTypeId = 3))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (1,3,GetDate(),'System','Live')
	END          
	
if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 1 and OrganisationTypeId = 4))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (1,4,GetDate(),'System','Live')
	END 

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 1 and OrganisationTypeId = 5))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (1,5,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 1 and OrganisationTypeId = 6))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (1,6,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 1 and OrganisationTypeId = 7))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (1,7,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 1 and OrganisationTypeId = 8))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (1,8,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 2 and OrganisationTypeId = 9))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (2,9,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 2 and OrganisationTypeId = 10))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (2,10,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 2 and OrganisationTypeId = 11))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (2,11,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 2 and OrganisationTypeId = 12))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (2,12,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 2 and OrganisationTypeId = 13))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (2,13,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 2 and OrganisationTypeId = 14))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (2,14,GetDate(),'System','Live')
	END

if (not exists (select * from OrganisationCategoryType where  OrganisationCategoryId = 2 and OrganisationTypeId = 15))
	BEGIN
	INSERT INTO [dbo].[OrganisationCategoryType]
			   ([OrganisationCategoryId],[OrganisationTypeId],[CreatedAt],[CreatedBy],[Status])
		 VALUES (2,15,GetDate(),'System','Live')
	END



---- END OF APR-638 ------------------

