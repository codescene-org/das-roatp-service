CREATE TABLE [dbo].[ApplicationRouteOrganisationTypes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ApplicationRouteId] INT NOT NULL, 
    [OrganisationTypeId] INT NOT NULL, 
    [CreatedAt] DATETIME2 NOT NULL, 
    [CreatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NULL, 
    [UpdatedBy] NVARCHAR(30) NULL, 
    [Status] NVARCHAR(20) NOT NULL, 
    CONSTRAINT [FK_ApplicationRouteOrganisationTypes_ApplicationRoutes] FOREIGN KEY ([ApplicationRouteId]) REFERENCES [ApplicationRoutes]([Id]), 
    CONSTRAINT [FK_ApplicationRouteOrganisationTypes_OrganisationTypes] FOREIGN KEY ([OrganisationTypeId]) REFERENCES [OrganisationTypes]([Id])
)
