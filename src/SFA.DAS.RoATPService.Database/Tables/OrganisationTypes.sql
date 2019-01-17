CREATE TABLE [dbo].[OrganisationTypes] (
    [Id]              INT            NOT NULL IDENTITY,
    [Type]            NVARCHAR (100) NOT NULL,
    [Status]          NVARCHAR (10)  NOT NULL,
    [Description] NVARCHAR(255) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

