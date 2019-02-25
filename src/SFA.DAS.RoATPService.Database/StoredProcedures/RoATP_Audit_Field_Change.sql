CREATE PROCEDURE dbo.RoATP_Audit_Field_Change
	@organisationId UNIQUEIDENTIFIER,
	@updatedBy NVARCHAR(30),
	@updatedAt DATETIME2,
	@fieldChanged NVARCHAR(50),
	@previousValue NVARCHAR(MAX),
	@newValue NVARCHAR(MAX)
AS
BEGIN
	DECLARE @previousStatusDate DATETIME2

	SELECT @previousStatusDate = MAX(UpdatedAt)
	FROM [Audit]
	WHERE OrganisationId = @organisationId
	AND FieldChanged = @fieldChanged

	IF @previousStatusDate IS NULL
	BEGIN
		SELECT @previousStatusDate = CreatedAt
		FROM dbo.Organisations
		WHERE Id = @organisationId
	END
	
	INSERT INTO [Audit] 
    ([OrganisationId], [UpdatedBy], [UpdatedAt], [FieldChanged], [PreviousValue], [NewValue], [PreviousStatusDate]) 
    VALUES(@organisationId, @updatedBy, @updatedAt, @fieldChanged, @previousValue, @newValue, @previousStatusDate)

	RETURN 1
END

