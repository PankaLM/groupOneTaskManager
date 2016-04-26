PRINT 'Tasks'
GO

CREATE TABLE [dbo].[Tasks] (
    TaskId                INT             NOT NULL IDENTITY,
    UserId                INT             NOT NULL,
    InternalImportance    BIT             NOT NULL,
    ЕxternalImportance    BIT             NOT NULL,
    Clearness             BIT             NOT NULL,
    Closeness             BIT             NOT NULL,
    Simplicity            BIT             NOT NULL,

    FlyScore              INT             NOT NULL,

    GroupId               INT             NULL,

    Title                 NVARCHAR(MAX)   NOT NULL,
    [Description]         NVARCHAR(MAX)   NULL,
    Tag                   NVARCHAR(MAX)   NULL,
    Thumbnail             NVARCHAR(MAX)   NULL,
    Deadline              DATETIME2       NULL,
    Duration              INT             NULL,
    PostponeDeadline      DATETIME2       NULL,
    StateId               INT             NOT NULL,
    ActionId              INT             NULL,
    DependantTaskId       INT             NULL,
    StartedOn             DATETIME2       NOT NULL,
    CompletedOn           DATETIME2       NULL,
    ModifyDate            DATETIME2       NULL,
    Notified              BIT             NOT NULL,

    CONSTRAINT [PK_Tasks]        PRIMARY KEY (TaskId),
    CONSTRAINT [FK_Tasks_Users]  FOREIGN KEY (UserId)  REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Tasks_RecurringTaskGroups]  FOREIGN KEY (GroupId)  REFERENCES [dbo].[RecurringTaskGroups] (GroupId),
    CONSTRAINT [FK_Tasks_Tasks]  FOREIGN KEY (DependantTaskId)  REFERENCES [dbo].[Tasks] ([TaskId])
);
GO