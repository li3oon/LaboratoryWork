BACKUP DATABASE MMM
TO DISK = N'D:\программы\SQLServer2022\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup\MMM_FULL_1.BAK'
WITH FORMAT, INIT, NAME = N'MMM Full Backup 1';
GO

-- Бэкап журнала транзакций
BACKUP DATABASE MMM
TO DISK = N'D:\программы\SQLServer2022\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup\MMM.TRN'
WITH INIT, NAME = N'MMM Log Backup 1';
GO

-- Разностный бэкап (DIFFERENTIAL)
BACKUP DATABASE MMM
TO DISK = N'D:\программы\SQLServer2022\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup\MMM.BAK'
WITH DIFFERENTIAL, NAME = N'MMM Differential Backup 1';
GO

-- Восстановление из полного бэкапа (пример)
-- Если восстанавливаете поверх существующей БД — нужно WITH REPLACE
/*RESTORE DATABASE MMM
FROM DISK = N'D:\программы\SQLServer2022\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup\MMM_FULL_1.BAK'
WITH REPLACE, RECOVERY;
GO*/