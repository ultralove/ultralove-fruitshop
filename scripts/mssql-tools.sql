
SELECT collection_id, collection_name, feed_url
FROM fruitshop_collections
WHERE collection_id IN (
  SELECT collection_id
FROM fruitshop_collections
GROUP BY collection_id
HAVING COUNT(collection_id) > 1)
ORDER BY collection_id

DELETE FROM fruitshop_collections WHERE collection_id IN (
  SELECT collection_id
FROM fruitshop_collections
GROUP BY collection_id
HAVING COUNT(collection_id) > 1)

SELECT collection_id, collection_name, feed_url
FROM fruitshop_collections
WHERE feed_url IN (
  SELECT feed_url
FROM fruitshop_collections
GROUP BY feed_url
HAVING COUNT(feed_url) > 1)
ORDER BY feed_url
