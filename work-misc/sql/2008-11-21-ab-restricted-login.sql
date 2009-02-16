USE [ABDB]
GO
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name='abu')
	CREATE LOGIN [abu] -- a.k.a. Asset Bank User
		WITH 
			PASSWORD=N'AssetBankUser2009', 
			DEFAULT_DATABASE=[ABDB], 
			DEFAULT_LANGUAGE=[us_english], 
			CHECK_EXPIRATION=ON, 
			CHECK_POLICY=ON
GO
IF NOT EXISTS (SELECT * FROM sysusers WHERE name='abu')
	CREATE USER [abu]
		FOR LOGIN [abu] 
		WITH DEFAULT_SCHEMA=[db_datareader]
GO
EXEC sp_addrolemember 'db_datareader', 'abu'
GO
EXEC sp_addrolemember 'db_datawriter', 'abu'
GO