CREATE TABLE [dbo].[OrganisationStatusEvent]
(
	[Id] BIGINT IDENTITY(30000,1) NOT NULL PRIMARY KEY, 
    [OrganisationStatusId] INT NOT NULL, 
    [CreatedOn] DATETIME NOT NULL DEFAULT Getdate(), 
    [ProviderId] BIGINT NOT NULL, 
    CONSTRAINT [FK_OrganisationProviderTypeEvents_OrganisationStatus] FOREIGN KEY (OrganisationStatusId) REFERENCES dbo.[OrganisationStatus]([Id])
)
