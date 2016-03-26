SET QUOTED_IDENTIFIER ON
GO

PRINT '------ Creating TaskManager'
:setvar rootPath "..\TaskManager.Database\Create"
:r $(rootPath)"\Create.sql"
