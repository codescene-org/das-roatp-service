CREATE TABLE [dbo].[ProviderTypeOrganisationTypes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProviderTypeId] INT NOT NULL, 
    [OrganisationTypeId] INT NOT NULL, 
    [CreatedAt] DATETIME2 NOT NULL, 
    [CreatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NULL, 
    [UpdatedBy] NVARCHAR(30) NULL, 
    [Status] NVARCHAR(20) NOT NULL, 
    CONSTRAINT [FK_ProviderTypeOrganisationTypes_ProviderTypes] FOREIGN KEY ([ProviderTypeId]) REFERENCES [ProviderTypes]([Id]), 
    CONSTRAINT [FK_ProviderTypeOrganisationTypes_OrganisationTypes] FOREIGN KEY ([OrganisationTypeId]) REFERENCES [OrganisationTypes]([Id])
)
