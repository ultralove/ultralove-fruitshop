
-- switch context
USE applepodcasts
GO

-- initialize scan table
DROP TABLE IF EXISTS apple_podcasts_scans;
GO

CREATE TABLE apple_podcasts_scans
(
  id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
  start_date DATETIME2 DEFAULT GETDATE(),
  stop_date DATETIME2
);
GO

GRANT INSERT, UPDATE, DELETE, SELECT ON apple_podcasts_scans TO applepodcasts;
GO

-- sp_start_scan
DROP PROCEDURE IF EXISTS sp_start_scan;
GO

CREATE PROCEDURE sp_start_scan
AS
BEGIN
  SET NOCOUNT ON
  INSERT INTO apple_podcasts_scans
    (start_date)
  VALUES
    (GETDATE());
  DECLARE @id INT = -1;
  SELECT @id = MAX(DISTINCT(id))
  FROM apple_podcasts_scans;
  RETURN @id;
END;
GO

GRANT EXECUTE ON sp_start_scan TO applepodcasts;
GO

-- sp_stop_scan
DROP PROCEDURE IF EXISTS sp_stop_scan;
GO

CREATE PROCEDURE sp_stop_scan
  @id INT
AS
BEGIN
  SET NOCOUNT ON
  UPDATE apple_podcasts_scans SET stop_date = GETDATE() WHERE id = @id;
END;
GO

GRANT EXECUTE ON sp_stop_scan TO applepodcasts;
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
  FROM apple_podcasts_scans
  WHERE stop_date IS NOT NULL
  ORDER BY id ASC;
END;
GO

GRANT EXECUTE ON sp_select_scans TO applepodcasts;
GO

-- initialize podcasts table
DROP INDEX IF EXISTS ix_apple_podcasts_collection_id_scan_id ON apple_podcasts;
GO

IF OBJECT_ID('apple_podcasts', 'U') IS NOT NULL
BEGIN
  ALTER TABLE apple_podcasts DROP CONSTRAINT IF EXISTS pk_apple_podcasts
END;
GO

DROP TABLE IF EXISTS apple_podcasts;
GO

CREATE TABLE apple_podcasts
(
  collection_id VARCHAR(32) NOT NULL,
  collection_name VARCHAR(1024),
  feed_url VARCHAR(1024),
  discovered INT NOT NULL REFERENCES apple_podcasts_scans (id),
  retired INT NOT NULL DEFAULT -1,
  retry_count INT NOT NULL DEFAULT -1
);
GO

GRANT INSERT, UPDATE, DELETE, SELECT ON apple_podcasts TO applepodcasts;
GO

-- ALTER TABLE apple_podcasts ADD CONSTRAINT pk_apple_podcasts PRIMARY KEY NONCLUSTERED (id)
-- GO

CREATE UNIQUE CLUSTERED INDEX ix_apple_podcasts_collection_id_scan_id ON apple_podcasts (collection_id, discovered);
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
  FROM apple_podcasts;
  RETURN @count;
END;
GO

GRANT EXECUTE ON sp_count_collection_ids TO applepodcasts;
GO

-- sp_retire_collection_id
DROP PROCEDURE IF EXISTS sp_retire_collection_id;
GO

CREATE PROCEDURE sp_retire_collection_id
  @collection_id VARCHAR(32),
  @scan_id INT
AS
BEGIN
  SET NOCOUNT ON
  UPDATE apple_podcasts SET retired = @scan_id WHERE collection_id = @collection_id;
END;
GO

GRANT EXECUTE ON sp_retire_collection_id TO applepodcasts;
GO

-- sp_select_active_collection_ids
DROP PROCEDURE IF EXISTS sp_select_active_collection_ids;
GO

CREATE PROCEDURE sp_select_active_collection_ids
AS
BEGIN
  SET NOCOUNT ON
  SELECT collection_id
  FROM apple_podcasts
  WHERE retired < 0;
END;
GO

GRANT EXECUTE ON sp_select_active_collection_ids TO applepodcasts;
GO

-- sp_select_retired_collection_ids
DROP PROCEDURE IF EXISTS sp_select_retired_collection_ids;
GO

CREATE PROCEDURE sp_select_retired_collection_ids
AS
BEGIN
  SET NOCOUNT ON
  SELECT collection_id
  FROM apple_podcasts
  WHERE retired >= 0;
END;
GO

GRANT EXECUTE ON sp_select_retired_collection_ids TO applepodcasts;
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
  FROM apple_podcasts
  WHERE retry_count = -1
END;
GO

GRANT EXECUTE ON sp_select_incoming_collection_ids TO applepodcasts;
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
  FROM apple_podcasts
  WHERE (retry_count < 0) AND (retry_count > -5)
END;
GO

GRANT EXECUTE ON sp_select_pending_collection_ids TO applepodcasts;
GO

-- sp_update_collection
DROP PROCEDURE IF EXISTS sp_update_collection;
GO

CREATE PROCEDURE sp_update_collection
  @collectionId VARCHAR(32),
  @collectionName VARCHAR(1024),
  @feedUrl VARCHAR(1024)
AS
BEGIN
  SET NOCOUNT ON
  UPDATE apple_podcasts SET collection_name = @collectionName, feed_url = @feedUrl, retry_count = 0 WHERE collection_id = @collectionId;
END;
GO

GRANT EXECUTE ON sp_update_collection TO applepodcasts;
GO

-- sp_update_failed_collection
DROP PROCEDURE IF EXISTS sp_update_failed_collection;
GO

CREATE PROCEDURE sp_update_failed_collection
  @p_collection_id VARCHAR(32)
AS
BEGIN
  SET NOCOUNT ON
  DECLARE @retryCount INT = -1;
  SELECT @retryCount = retry_count
  FROM apple_podcasts
  WHERE collection_id = @p_collection_id;
  UPDATE apple_podcasts SET retry_count = @retryCount - 1 WHERE collection_id = @p_collection_id;
END;
GO

GRANT EXECUTE ON sp_update_failed_collection TO applepodcasts;
GO

use applepodcasts

select count(collection_id) as valid
from apple_podcasts
where retry_count > -1;
select count(collection_id) as new
from apple_podcasts
where retry_count = -1;
select count(collection_id) as invalid
from apple_podcasts
where retry_count < -1;

select top(100)
  collection_name
from apple_podcasts
where retry_count > -1;

select
  count(feed_url)
from apple_podcasts
where feed_url like '%anchor%';
