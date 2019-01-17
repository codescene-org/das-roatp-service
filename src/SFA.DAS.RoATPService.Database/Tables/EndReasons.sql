CREATE TABLE [dbo].[EndReasons]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Status] NVARCHAR(10) NOT NULL, 
    [EndReason] NVARCHAR(100) NOT NULL, 
    [Description] NVARCHAR(255) NULL 
)
