﻿USE [master]
GO

print 'Create database $(dbName)'
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'$(dbName)')
BEGIN
    ALTER DATABASE [$(dbName)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [$(dbName)]
END
GO

CREATE DATABASE [$(dbName)] COLLATE Cyrillic_General_CI_AS
GO

ALTER DATABASE [$(dbName)]
SET ALLOW_SNAPSHOT_ISOLATION ON

ALTER DATABASE [$(dbName)]
SET READ_COMMITTED_SNAPSHOT ON
GO

USE [$(dbName)]
GO

---------------------------------------------------------------
-- Tables
---------------------------------------------------------------

:r $(rootPath)"\Tables\Users\Users.sql"
:r $(rootPath)"\Tables\Users\Achievements.sql"
:r $(rootPath)"\Tables\Users\UserAchievements.sql"

:r $(rootPath)"\Tables\Tasks\Tasks.sql"

