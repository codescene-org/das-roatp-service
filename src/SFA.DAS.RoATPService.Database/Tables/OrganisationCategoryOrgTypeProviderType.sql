CREATE TABLE [dbo].[OrganisationCategoryOrgTypeProviderType]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OrganisationTypeId] INT NOT NULL, 
    [OrganisationCategoryId] INT NOT NULL, 
	[ProviderTypeId] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL, 
    [CreatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NULL, 
    [UpdatedBy] NVARCHAR(30) NULL, 
    [Status] NVARCHAR(20) NOT NULL, 
    CONSTRAINT [FK_OrganisationCategoryType_OrganisationType] FOREIGN KEY ([OrganisationTypeId]) REFERENCES [OrganisationTypes]([Id]), 
    CONSTRAINT [FK_OrganisationCategoryType_OrganisationCategory] FOREIGN KEY ([OrganisationCategoryId]) REFERENCES [OrganisationCategory]([Id]),
	CONSTRAINT [FK_OrganisationCategoryType_ProviderType] FOREIGN KEY (ProviderTypeId) REFERENCES [ProviderTypes] (Id)
)