PRINT 'RecurringTaskGroups'
GO

CREATE TABLE [dbo].[RecurringTaskGroups] (
    GroupId                INT       NOT NULL IDENTITY,
    Interval               BIGINT    NOT NULL,
    StartTime              DATETIME2 NOT NULL,
    CONSTRAINT [PK_RecurringTasksGroups]        PRIMARY KEY (GroupId)
);
GO