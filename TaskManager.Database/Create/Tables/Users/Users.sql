PRINT 'Users'
GO

CREATE TABLE [dbo].[Users] (
    [UserId]                INT                 NOT NULL IDENTITY,
    [Username]              NVARCHAR (200)      NOT NULL UNIQUE,
    [PasswordHash]          NVARCHAR (200)      NULL,
    [PasswordSalt]          NVARCHAR (200)      NULL,
    [Fullname]              NVARCHAR (200)      NULL,
    [Email]                 NVARCHAR (100)      NULL,
    [IsActive]              BIT                 NOT NULL,
    [FlyCutoff]             INT                 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);
GO