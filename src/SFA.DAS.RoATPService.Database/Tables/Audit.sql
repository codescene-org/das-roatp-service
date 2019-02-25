CREATE TABLE [dbo].[Audit]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1), 
    [OrganisationId] UNIQUEIDENTIFIER NOT NULL, 
    [UpdatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NOT NULL, 
    [FieldChanged] NVARCHAR(50) NOT NULL, 
    [PreviousValue] NVARCHAR(MAX) NOT NULL, 
    [NewValue] NVARCHAR(MAX) NOT NULL,
	[PreviousStatusDate] DATETIME2 NULL
)
