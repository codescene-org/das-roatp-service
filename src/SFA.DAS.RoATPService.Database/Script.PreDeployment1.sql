/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF EXISTS (
	SELECT 1 from INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'Audit'
	AND COLUMN_NAME = 'FieldChanged'
	)
BEGIN
	DELETE FROM dbo.[Audit]

END

IF EXISTS (
    SELECT 1 from INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Organisations'
)
BEGIN
	IF EXISTS (
		SELECT 1 FROM dbo.Organisations WHERE OrganisationTypeId NOT IN (0)
		)
	BEGIN
		UPDATE dbo.Organisations
		SET OrganisationTypeId = 0
	END
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
