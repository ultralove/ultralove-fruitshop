USE master;
GO

-- initialize database
DROP DATABASE IF EXISTS fruitshop;
GO

CREATE DATABASE fruitshop ON
  (NAME = fruitshopdata, FILENAME = '/var/opt/mssql/data/fruitshop.mdf', SIZE = 1GB, FILEGROWTH=1GB)
LOG ON
  (NAME = fruitshoplog, FILENAME = '/var/opt/mssql/data/fruitshop.ldf', SIZE = 1GB, FILEGROWTH=1GB);
GO

USE fruitshop;
GO

-- initialize credentials
IF EXISTS (SELECT name
FROM master.sys.server_principals
WHERE name = 'fruitshop')
BEGIN
  DROP LOGIN fruitshop;
END;
GO

CREATE LOGIN fruitshop with password = N'Fruit!shop0';
GO

DROP USER IF EXISTS fruitshop;
GO

-- initialize user
CREATE USER fruitshop FOR LOGIN fruitshop;
GO
