PRINT 'UserAchievements'
GO

CREATE TABLE [dbo].[UserAchievements] (
    AchievementId        INT    NOT NULL,
    UserId               INT    NOT NULL,
    CONSTRAINT [PK_UserAchievements]        PRIMARY KEY (AchievementId, UserId),
    CONSTRAINT [FK_UserAchievements_Users]  FOREIGN KEY (UserId)  REFERENCES [dbo].[Users] (UserId),
    CONSTRAINT [FK_UserAchievements_Achievements]  FOREIGN KEY (AchievementId)  REFERENCES [dbo].[Achievements] (AchievementId)
);
GO