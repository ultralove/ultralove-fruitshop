namespace Fruitshop;

public class Repository
{
  private static readonly SqlServerReaderWriter s_readerWriter = new();

  public static Int32 StartScan() => s_readerWriter.StartScan();
  public static void StopScan(Int32 id) => s_readerWriter.StopScan(id);

  public static List<Collection> SelectCollections() => s_readerWriter.SelectCollections();

  public static void InsertCollections(Int32 scanId, List<Collection> collections) => s_readerWriter.InsertCollections(scanId, collections);

  public static List<String> SelectActiveCollectionIds() => s_readerWriter.SelectActiveCollectionIds();
  public static Int32 CountActiveCollectionIds() => s_readerWriter.CountActiveCollectionIds();

  public static List<String> SelectRetiredCollectionIds() => s_readerWriter.SelectRetiredCollectionIds();
  public static Int32 CountRetiredCollectionIds() => s_readerWriter.CountRetiredCollectionIds();

  public static List<String> SelectIncomingCollectionIds(Int32 count) => s_readerWriter.SelectIncomingCollectionIds(count);
  public static Int32 CountIncomingCollectionIds() => s_readerWriter.CountIncomingCollectionIds();

  public static List<String> SelectPendingCollectionIds(Int32 count) => s_readerWriter.SelectPendingCollectionIds(count);
  public static Int32 CountPendingCollectionIds() => s_readerWriter.CountPendingCollectionIds();

  public static List<String> SelectCollectionIds() => s_readerWriter.SelectCollectionIds();
  public static Int32 CountCollectionIds() => s_readerWriter.CountCollectionIds();

  public static void InsertCollectionIds(Int32 id, List<String> collectionIds) => s_readerWriter.InsertCollectionIds(id, collectionIds);
  public static void RetireCollectionIds(Int32 id, List<String> collectionIds) => s_readerWriter.RetireCollectionIds(id, collectionIds);
  public static void UpdateCollection(Collection collection) => s_readerWriter.UpdateCollection(collection);
  public static void UpdateFailedCollection(String collectionId) => s_readerWriter.UpdateFailedCollection(collectionId);

  public static List<Collection> FindAnyCollection(String pattern) => s_readerWriter.FindAnyCollection(pattern);
}
