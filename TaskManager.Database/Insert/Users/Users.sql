SET IDENTITY_INSERT [Users] ON

INSERT INTO [Users] ([UserId],[Username],[PasswordHash],[PasswordSalt],[Fullname],[IsActive]) 
VALUES
(1       ,N'admin'   ,N'AMWr4Peeajc9Q0bsdV7mWBwK6fLJN/Cr/ksp2jV4M50RQ587JQHptHzA7HLDOLSHGg==',N'88SHIJlJUThYeUFDUQKJoQ==', N'Администратор'      ,  1)

SET IDENTITY_INSERT [Users] OFF
GO
