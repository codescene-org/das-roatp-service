CREATE TABLE [dbo].[ApplicationRoutes] (
    [Id]				[int] NOT NULL IDENTITY,
    [Route]				NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR(255) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

