PRINT 'Achievements'
GO

CREATE TABLE [dbo].[Achievements] (
    AchievementId      INT             NOT NULL IDENTITY,
    Name               NVARCHAR(MAX)   NOT NULL,
    Alias              NVARCHAR(MAX)   NOT NULL,
    CONSTRAINT [PK_Achievements]        PRIMARY KEY (AchievementId)
);
GO