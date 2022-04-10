using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Ultralove;

public class SqlServerReaderWriter
{
  public static String ConnectionString => SqlServerCredentials.ConnectionString;

  public static void Reset()
  {
    using var connection = new SqlConnection(ConnectionString);
    connection.Open();
    try {
      connection.Execute("truncate table apple_podcasts");
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
  }

  public static Int32 CountCollectionIds()
  {
    var result = -1;
    using var connection = new SqlConnection(ConnectionString);
    connection.Open();
    try {
      var parameters = new DynamicParameters();
      parameters.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
      connection.Query("sp_count_collection_ids", parameters, commandType: CommandType.StoredProcedure);
      result = parameters.Get<Int32>("@result");
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
    return result;
  }

  public static void InsertCollectionIds(List<String> collectionIds)
  {
    using var connection = new SqlConnection(ConnectionString);
    connection.Open();
    try {
      collectionIds.ForEach(collectionId => {
        var parameters = new
        {
          collection_id = collectionId
        };
        connection.Query("sp_insert_collection_id", parameters, commandType: CommandType.StoredProcedure);
      });
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
  }

  public static void FastInsertCollectionIds(List<String> collectionIds)
  {
    var table = new DataTable();
    table.Columns.Add("CollectionId", typeof(String));
    collectionIds.ForEach(collectionId => table.Rows.Add(new Object[] { collectionId }));

    using var connection = new SqlConnection(ConnectionString);
    connection.Open();
    var transaction = connection.BeginTransaction();
    try {
      var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
      bulkCopy.BatchSize = 1000;
      bulkCopy.DestinationTableName = "apple_podcasts";
      bulkCopy.ColumnMappings.Add("CollectionId", "collection_id");
      bulkCopy.WriteToServer(table);
      transaction.Commit();
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
      transaction.Rollback();
    }
    finally {
      connection.Close();
    }
  }

  public static void InsertOrUpdate(List<Podcast> podcasts)
  {
    using var connection = new SqlConnection(ConnectionString);
    connection.Open();
    try {
      podcasts.ForEach(podcast => {
        var parameters = new
        {
          collection_id = podcast.CollectionId,
          collection_name = podcast.CollectionName,
          feed_url = podcast.FeedUrl
        };
        connection.Query("sp_insert_collection_id", parameters, commandType: CommandType.StoredProcedure);
      });
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
  }

  public static void FastInsertOrUpdate(List<String> podcasts)
  {
    using var connection = new SqlConnection(ConnectionString);
    connection.Open();
    var transaction = connection.BeginTransaction();
    try {
      var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
      bulkCopy.BatchSize = 1000;
      bulkCopy.DestinationTableName = "apple_podcasts";
      bulkCopy.ColumnMappings.Add("CollectionId", "collection_id");
      bulkCopy.ColumnMappings.Add("CollectionName", "collection_name");
      bulkCopy.ColumnMappings.Add("FeedUrl", "feed_url");
      bulkCopy.WriteToServer(podcasts.AsDataTable());
      transaction.Commit();
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
      transaction.Rollback();
    }
    finally {
      connection.Close();
    }
  }
}
