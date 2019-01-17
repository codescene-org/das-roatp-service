CREATE TABLE [dbo].[ApplicationRouteOrganisationTypes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ApplicationRouteId] INT NOT NULL, 
    [OrganisationTypeId] INT NOT NULL, 
    CONSTRAINT [FK_ApplicationRouteOrganisationTypes_ApplicationRoutes] FOREIGN KEY ([ApplicationRouteId]) REFERENCES [ApplicationRoutes]([Id]), 
    CONSTRAINT [FK_ApplicationRouteOrganisationTypes_OrganisationTypes] FOREIGN KEY ([OrganisationTypeId]) REFERENCES [OrganisationTypes]([Id])
)
