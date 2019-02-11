CREATE TABLE [dbo].[RemovedReasons]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Status] NVARCHAR(20) NOT NULL, 
    [RemovedReason] NVARCHAR(100) NOT NULL, 
    [Description] NVARCHAR(255) NULL,
	[CreatedAt] DATETIME2 NOT NULL, 
    [CreatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NULL, 
    [UpdatedBy] NVARCHAR(30) NULL
)
