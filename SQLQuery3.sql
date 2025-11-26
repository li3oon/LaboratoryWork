IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'qwerty')
    DROP LOGIN qwerty;
GO

CREATE LOGIN qwerty WITH PASSWORD = '123456', DEFAULT_DATABASE = master;
GO

USE MMM;
GO

IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'qwerty')
    DROP USER qwerty;
GO

CREATE USER qwerty FOR LOGIN qwerty;
GO

GRANT SELECT, INSERT ON dbo.модель TO qwerty;
GO

EXEC xp_instance_regread
    'HKEY_LOCAL_MACHINE',
    'Software\Microsoft\MSSQLServer\MSSQLServer',
    'LoginMode';
GO
