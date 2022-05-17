USE master;
GO

-- initialize database
DROP DATABASE IF EXISTS applepodcasts;
GO

CREATE DATABASE applepodcasts ON
  (NAME = applepodcastsdata, FILENAME = '/var/opt/mssql/data/applepodcasts.mdf', SIZE = 1GB, FILEGROWTH=1GB)
LOG ON
  (NAME = applepodcastslog, FILENAME = '/var/opt/mssql/data/applepodcasts.ldf', SIZE = 1GB, FILEGROWTH=1GB);
GO

USE applepodcasts;
GO

-- initialize credentials
IF EXISTS (SELECT name FROM master.sys.server_principals
WHERE name = 'applepodcasts')
BEGIN
  DROP LOGIN applepodcasts;
END;
GO

CREATE LOGIN applepodcasts with password = N'Apple!podcasts0';
GO

DROP USER IF EXISTS applepodcasts;
GO

-- initialize user
CREATE USER applepodcasts FOR LOGIN applepodcasts;
GO
