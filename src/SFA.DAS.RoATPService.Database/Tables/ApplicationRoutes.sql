CREATE TABLE [dbo].[ApplicationRoutes] (
    [Id]				[int] NOT NULL IDENTITY,
    [Route]				NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR(255) NULL, 
	[CreatedAt] DATETIME2 NOT NULL, 
    [CreatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NULL, 
    [UpdatedBy] NVARCHAR(30) NULL, 
    [Status] NVARCHAR(20) NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

