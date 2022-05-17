using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Ultralove;

public class SqlServerReaderWriter
{
  private readonly String _connectionString = SqlServerCredentials.ConnectionString;

  public Int32 StartScan()
  {
    var result = -1;
    using var connection = new SqlConnection(this._connectionString);
    connection.Open();
    try {
      var parameters = new DynamicParameters();
      parameters.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
      connection.Query("sp_start_scan", parameters, commandType: CommandType.StoredProcedure);
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

  public void StopScan(Int32 scanId)
  {
    using var connection = new SqlConnection(this._connectionString);
    connection.Open();
    try {
      var parameters = new
      {
        id = scanId
      };
      connection.Query("sp_stop_scan", parameters, commandType: CommandType.StoredProcedure);
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
  }

  public List<String> SelectActiveCollectionIds()
  {
    var result = new List<String>();
    using var connection = new SqlConnection(this._connectionString);
    connection.Open();
    try {
      var parameters = new DynamicParameters();
      result = connection.Query<String>("sp_select_active_collection_ids", commandType: CommandType.StoredProcedure).ToList();
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
    return result;
  }

  public List<String> SelectRetiredCollectionIds()
  {
    var result = new List<String>();
    using var connection = new SqlConnection(this._connectionString);
    connection.Open();
    try {
      var parameters = new DynamicParameters();
      result = connection.Query<String>("sp_select_retired_collection_ids", commandType: CommandType.StoredProcedure).ToList();
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
    return result;
  }

  public Int32 CountCollectionIds()
  {
    var result = -1;
    using var connection = new SqlConnection(this._connectionString);
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

  public void InsertCollectionIds(Int32 id, List<String> collectionIds)
  {
    var table = new DataTable();
    table.Columns.Add("collection_id", typeof(String));
    table.Columns.Add("discovered", typeof(String));
    collectionIds.ForEach(collectionId => table.Rows.Add(new Object[] { collectionId, id }));

    using var connection = new SqlConnection(this._connectionString);
    connection.Open();
    var transaction = connection.BeginTransaction();
    try {
      var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
      bulkCopy.BatchSize = 1000;
      bulkCopy.DestinationTableName = "apple_podcasts";
      bulkCopy.ColumnMappings.Add("collection_id", "collection_id");
      bulkCopy.ColumnMappings.Add("discovered", "discovered");
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

  public void RetireCollectionIds(Int32 id, List<String> collectionIds)
  {
    if (collectionIds.Count > 0) {
      using var connection = new SqlConnection(this._connectionString);
      connection.Open();
      var transaction = connection.BeginTransaction();
      try {
        collectionIds.ForEach(collectionId => {
          var parameters = new
          {
            collection_id = collectionId,
            scan_id = id
          };
          connection.Query<String>("sp_retire_collection_id", parameters, transaction, commandType: CommandType.StoredProcedure);
        });
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

  public List<String> SelectIncomingCollectionIds(Int32 count)
  {
    var result = new List<String>();
    using var connection = new SqlConnection(this._connectionString);
    connection.Open();
    try {
      var parameters = new
      {
        size = count
      };
      result = connection.Query<String>("sp_select_incoming_collection_ids", parameters, commandType: CommandType.StoredProcedure).ToList();
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
    return result;
  }

  public List<String> SelectPendingCollectionIds(Int32 count)
  {
    var result = new List<String>();
    using var connection = new SqlConnection(this._connectionString);
    connection.Open();
    try {
      var parameters = new
      {
        size = count
      };
      result = connection.Query<String>("sp_select_pending_collection_ids", parameters, commandType: CommandType.StoredProcedure).ToList();
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
    return result;
  }

  public void UpdateCollection(Collection collection)
  {
    using var connection = new SqlConnection(this._connectionString);
    connection.Open();
    try {
      var parameters = new
      {
        collectionId = collection.CollectionId,
        collectionName = collection.CollectionName,
        feedUrl = collection.FeedUrl
      };
      connection.Query<String>("sp_update_collection", parameters, commandType: CommandType.StoredProcedure);
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
  }

  public void UpdateFailedCollection(String collectionId)
  {
    using var connection = new SqlConnection(this._connectionString);
    connection.Open();
    try {
      var parameters = new
      {
        p_collection_id = collectionId
      };
      connection.Query<String>("sp_update_failed_collection", parameters, commandType: CommandType.StoredProcedure);
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
    finally {
      connection.Close();
    }
  }

  // public void Insert(List<Podcast> podcasts)
  // {
  //   using var connection = new SqlConnection(this._connectionString);
  //   connection.Open();
  //   var transaction = connection.BeginTransaction();
  //   try {
  //     var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
  //     bulkCopy.BatchSize = 1000;
  //     bulkCopy.DestinationTableName = "apple_podcasts";
  //     bulkCopy.ColumnMappings.Add("CollectionId", "collection_id");
  //     bulkCopy.ColumnMappings.Add("CollectionName", "collection_name");
  //     bulkCopy.ColumnMappings.Add("FeedUrl", "feed_url");
  //     bulkCopy.WriteToServer(podcasts.AsDataTable());
  //     transaction.Commit();
  //   }
  //   catch (Exception e) {
  //     Console.WriteLine(e.Message);
  //     transaction.Rollback();
  //   }
  //   finally {
  //     connection.Close();
  //   }
  // }
}
