CREATE TABLE [dbo].[ProviderTypeOrganisationStatus]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProviderTypeId] INT NOT NULL, 
    [OrganisationStatusId] INT NOT NULL, 
    [CreatedAt] DATETIME2 NOT NULL, 
    [CreatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NULL, 
    [UpdatedBy] NVARCHAR(30) NULL, 
    [Status] NVARCHAR(20) NOT NULL, 
    CONSTRAINT [FK_ProviderTypeOrganisationStatus_ProviderTypes] FOREIGN KEY ([ProviderTypeId]) REFERENCES [ProviderTypes]([Id]), 
    CONSTRAINT [FK_ProviderTypeOrganisationStatus_OrganisationStatuses] FOREIGN KEY ([OrganisationStatusId]) REFERENCES [OrganisationStatus]([Id])
)