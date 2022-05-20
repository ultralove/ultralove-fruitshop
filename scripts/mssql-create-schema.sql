
-- switch context
USE fruitshop
GO

-- initialize scan table
DROP TABLE IF EXISTS fruitshop_scans;
GO

CREATE TABLE fruitshop_scans
(
  id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
  start_date DATETIME2 DEFAULT GETDATE(),
  stop_date DATETIME2
);
GO

GRANT INSERT, UPDATE, DELETE, SELECT ON fruitshop_scans TO fruitshop;
GO

-- sp_start_scan
DROP PROCEDURE IF EXISTS sp_start_scan;
GO

CREATE PROCEDURE sp_start_scan
AS
BEGIN
  SET NOCOUNT ON
  INSERT INTO fruitshop_scans
    (start_date)
  VALUES
    (GETDATE());
  DECLARE @id INT = -1;
  SELECT @id = MAX(DISTINCT(id))
  FROM fruitshop_scans;
  RETURN @id;
END;
GO

GRANT EXECUTE ON sp_start_scan TO fruitshop;
GO

-- sp_stop_scan
DROP PROCEDURE IF EXISTS sp_stop_scan;
GO

CREATE PROCEDURE sp_stop_scan
  @id INT
AS
BEGIN
  SET NOCOUNT ON
  UPDATE fruitshop_scans SET stop_date = GETDATE() WHERE id = @id;
END;
GO

GRANT EXECUTE ON sp_stop_scan TO fruitshop;
GO

-- sp_select_scans
DROP PROCEDURE IF EXISTS sp_select_scans;
GO

CREATE PROCEDURE sp_select_scans
  @id INT
AS
BEGIN
  SET NOCOUNT ON
  SELECT id, start_date, stop_date
  FROM fruitshop_scans
  WHERE stop_date IS NOT NULL
  ORDER BY id ASC;
END;
GO

GRANT EXECUTE ON sp_select_scans TO fruitshop;
GO

-- initialize podcasts table
DROP INDEX IF EXISTS ix_fruitshop_collection_id_scan_id ON fruitshop_collections;
GO

IF OBJECT_ID('fruitshop_collections', 'U') IS NOT NULL
BEGIN
  ALTER TABLE fruitshop_collections DROP CONSTRAINT IF EXISTS pk_fruitshop_collections
END;
GO

DROP TABLE IF EXISTS fruitshop_collections;
GO

CREATE TABLE fruitshop_collections
(
  id UNIQUEIDENTIFIER DEFAULT NEWID(),
  collection_id NVARCHAR(32) NOT NULL,
  collection_name NVARCHAR(1024),
  feed_url NVARCHAR(1024),
  scan_id INT NOT NULL REFERENCES fruitshop_scans (id),
  retired INT NOT NULL DEFAULT -1,
  retry_count INT NOT NULL DEFAULT -1
);
GO

GRANT INSERT, UPDATE, DELETE, SELECT ON fruitshop_collections TO fruitshop;
GO

-- ALTER TABLE fruitshop_collections ADD CONSTRAINT pk_fruitshop_collections PRIMARY KEY NONCLUSTERED (id)
-- GO

CREATE UNIQUE CLUSTERED INDEX ix_fruitshop_collection_id_scan_id ON fruitshop_collections (collection_id, scan_id);
GO

-- sp_count_collection_ids
DROP PROCEDURE IF EXISTS sp_count_collection_ids;
GO

CREATE PROCEDURE sp_count_collection_ids
AS
BEGIN
  SET NOCOUNT ON
  DECLARE @count INT = -1;
  SELECT @count = COUNT(collection_id)
  FROM fruitshop_collections;
  RETURN @count;
END;
GO

GRANT EXECUTE ON sp_count_collection_ids TO fruitshop;
GO

-- sp_retire_collection_id
DROP PROCEDURE IF EXISTS sp_retire_collection_id;
GO

CREATE PROCEDURE sp_retire_collection_id
  @collection_id NVARCHAR(32),
  @scan_id INT
AS
BEGIN
  SET NOCOUNT ON
  UPDATE fruitshop_collections SET retired = @scan_id WHERE collection_id = @collection_id;
END;
GO

GRANT EXECUTE ON sp_retire_collection_id TO fruitshop;
GO

-- sp_select_active_collection_ids
DROP PROCEDURE IF EXISTS sp_select_active_collection_ids;
GO

CREATE PROCEDURE sp_select_active_collection_ids
AS
BEGIN
  SET NOCOUNT ON
  SELECT collection_id
  FROM fruitshop_collections
  WHERE retired < 0;
END;
GO

GRANT EXECUTE ON sp_select_active_collection_ids TO fruitshop;
GO

-- sp_count_active_collection_ids
DROP PROCEDURE IF EXISTS sp_count_active_collection_ids;
GO

CREATE PROCEDURE sp_count_active_collection_ids
AS
BEGIN
  SET NOCOUNT ON
  DECLARE @count INT = -1;
  SELECT @count = COUNT(collection_id)
  FROM fruitshop_collections
  WHERE retired < 0;
  RETURN @count;
END;
GO

GRANT EXECUTE ON sp_count_active_collection_ids TO fruitshop;
GO

-- sp_select_retired_collection_ids
DROP PROCEDURE IF EXISTS sp_select_retired_collection_ids;
GO

CREATE PROCEDURE sp_select_retired_collection_ids
AS
BEGIN
  SET NOCOUNT ON
  SELECT collection_id
  FROM fruitshop_collections
  WHERE retired >= 0;
END;
GO

GRANT EXECUTE ON sp_select_retired_collection_ids TO fruitshop;
GO

-- sp_count_retired_collection_ids
DROP PROCEDURE IF EXISTS sp_count_retired_collection_ids;
GO

CREATE PROCEDURE sp_count_retired_collection_ids
AS
BEGIN
  SET NOCOUNT ON
  DECLARE @count INT = -1;
  SELECT @count = COUNT(collection_id)
  FROM fruitshop_collections
  WHERE retired >= 0;
  RETURN @count;
END;
GO

GRANT EXECUTE ON sp_count_retired_collection_ids TO fruitshop;
GO

-- sp_select_incoming_collection_ids
DROP PROCEDURE IF EXISTS sp_select_incoming_collection_ids;
GO

CREATE PROCEDURE sp_select_incoming_collection_ids
  @size INT
AS
BEGIN
  SET NOCOUNT ON
  SELECT TOP(@size)
    collection_id
  FROM fruitshop_collections
  WHERE retry_count = -1
END;
GO

GRANT EXECUTE ON sp_select_incoming_collection_ids TO fruitshop;
GO

-- sp_count_incoming_collection_ids
DROP PROCEDURE IF EXISTS sp_count_incoming_collection_ids;
GO

CREATE PROCEDURE sp_count_incoming_collection_ids
AS
BEGIN
  SET NOCOUNT ON
  DECLARE @count INT = -1;
  SELECT @count = COUNT(collection_id)
  FROM fruitshop_collections
  WHERE retry_count = -1;
  RETURN @count;
END;
GO

GRANT EXECUTE ON sp_count_incoming_collection_ids TO fruitshop;
GO

-- sp_select_pending_collection_ids
DROP PROCEDURE IF EXISTS sp_select_pending_collection_ids;
GO

CREATE PROCEDURE sp_select_pending_collection_ids
  @size INT
AS
BEGIN
  SET NOCOUNT ON
  SELECT TOP(@size)
    collection_id
  FROM fruitshop_collections
  WHERE (retry_count < 0) AND (retry_count > -5)
END;
GO

GRANT EXECUTE ON sp_select_pending_collection_ids TO fruitshop;
GO

-- sp_count_pending_collection_ids
DROP PROCEDURE IF EXISTS sp_count_pending_collection_ids;
GO

CREATE PROCEDURE sp_count_pending_collection_ids
AS
BEGIN
  SET NOCOUNT ON
  DECLARE @count INT = -1;
  SELECT @count = COUNT(collection_id)
  FROM fruitshop_collections
  WHERE (retry_count < 0) AND (retry_count > -5);
  RETURN @count;
END;
GO

GRANT EXECUTE ON sp_count_pending_collection_ids TO fruitshop;
GO

-- sp_update_collection
DROP PROCEDURE IF EXISTS sp_update_collection;
GO

CREATE PROCEDURE sp_update_collection
  @collectionId NVARCHAR(32),
  @collectionName NVARCHAR(1024),
  @feedUrl NVARCHAR(1024)
AS
BEGIN
  SET NOCOUNT ON
  UPDATE fruitshop_collections SET collection_name = @collectionName, feed_url = @feedUrl, retry_count = 0 WHERE collection_id = @collectionId;
END;
GO

GRANT EXECUTE ON sp_update_collection TO fruitshop;
GO

-- sp_update_failed_collection
DROP PROCEDURE IF EXISTS sp_update_failed_collection;
GO

CREATE PROCEDURE sp_update_failed_collection
  @p_collection_id NVARCHAR(32)
AS
BEGIN
  SET NOCOUNT ON
  DECLARE @retryCount INT = -1;
  SELECT @retryCount = retry_count
  FROM fruitshop_collections
  WHERE collection_id = @p_collection_id;
  UPDATE fruitshop_collections SET retry_count = @retryCount - 1 WHERE collection_id = @p_collection_id;
END;
GO

GRANT EXECUTE ON sp_update_failed_collection TO fruitshop;
GO
