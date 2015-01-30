/*[User]*/
CREATE TABLE [User]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY
	,[RealName] nvarchar(200) NOT NULL
	,[Username] nvarchar(200) NOT NULL
	,[Password] varchar(100) NOT NULL
);

/*[ADMIN USER]*/
INSERT INTO [User] VALUES(NEWID(),'Admin','admin','123456')