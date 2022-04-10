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
IF EXISTS (SELECT name
FROM master.sys.server_principals
WHERE name = 'applepodcasts')
BEGIN
  DROP LOGIN applepodcasts;
END;
GO

CREATE LOGIN applepodcasts with password = N'Apple!podcasts0';
GO

DROP USER IF EXISTS applepodcasts;
GO

CREATE USER applepodcasts FOR LOGIN applepodcasts;
GO

-- initialize tables
DROP TABLE IF EXISTS apple_podcasts;
GO

CREATE TABLE apple_podcasts
(
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  collection_id VARCHAR(32) NOT NULL,
  collection_name VARCHAR(1024),
  feed_url VARCHAR(1024),
  last_seen DATETIME2 DEFAULT GETDATE(),
  retry_count INT NOT NULL DEFAULT -1
);
GO

GRANT INSERT, UPDATE, DELETE, SELECT ON apple_podcasts TO applepodcasts;
GO

-- initialize stored procedures
DROP PROCEDURE IF EXISTS sp_count_collection_ids;
GO

CREATE PROCEDURE sp_count_collection_ids
AS
BEGIN
  DECLARE @count INT = -1;
  SELECT @count = COUNT(collection_id)
  FROM apple_podcasts;
  RETURN @count;
END;
GO

GRANT EXECUTE ON sp_count_collection_ids TO applepodcasts;
GO

CREATE PROCEDURE sp_insert_collection_id
  @collection_id VARCHAR(32)
AS
BEGIN
  BEGIN TRAN
  IF NOT EXISTS (SELECT collection_id
  FROM apple_podcasts WITH (UPDLOCK, SERIALIZABLE)
  WHERE collection_id = @collection_id)
  BEGIN
    INSERT INTO apple_podcasts WITH (UPDLOCK, SERIALIZABLE)
      (collection_id)
    VALUES
      (@collection_id);
  END
  COMMIT TRAN
END;
GO

GRANT EXECUTE ON sp_insert_collection_id TO applepodcasts;
GO

DROP PROCEDURE IF EXISTS sp_insert_or_update;
GO

CREATE PROCEDURE sp_insert_or_update
  @collection_id VARCHAR(32),
  @collection_name VARCHAR(1024),
  @feed_url VARCHAR(1024)
AS
BEGIN
  BEGIN TRAN
  IF EXISTS (SELECT collection_id
  FROM apple_podcasts WITH (UPDLOCK, SERIALIZABLE)
  WHERE collection_id = @collection_id)
  BEGIN
    UPDATE apple_podcasts
    SET
      collection_id = @collection_id,
      feed_url = @feed_url,
      last_seen = GETDATE(),
      retry_count = 0
      WHERE collection_id = @collection_id
  END
  ELSE
  BEGIN
    INSERT INTO apple_podcasts
      (collection_id, collection_name, feed_url, last_seen, retry_count)
    VALUES
      (@collection_id, @collection_name, @feed_url, GETDATE(), 0)
  END
  COMMIT TRAN
END;
GO

GRANT EXECUTE ON sp_insert_or_update TO applepodcasts;
GO
