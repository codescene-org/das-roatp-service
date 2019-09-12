CREATE TABLE [dbo].[OrganisationStatus]
(
	[Id] INT NOT NULL IDENTITY PRIMARY KEY, 
    [Status] NVARCHAR(50) NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL, 
    [CreatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NULL, 
    [UpdatedBy] NVARCHAR(30) NULL, 
    [EventDescription] NVARCHAR(20) NULL
)
