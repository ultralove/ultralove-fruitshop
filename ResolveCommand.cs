namespace Ultralove;

public class ResolveCommand
{
  [Option(LongName = "count", ShortName = "c", Description = "The number of podcasts to resolve (default 100).")]
  private Int32 Count { get; set; } = 100;

  [Option(LongName = "size", ShortName = "s", Description = "The number of podcasts to resolve (default 200).")]
  private Int32 Size { get; set; } = 200;

  [Option(LongName = "delay", ShortName = "d", Description = "The number of podcasts to resolve (default 1000ms).")]
  private Int32 Delay { get; set; } = 1000;

  [Option(LongName = "no-retries", ShortName = "n", Description = "Don't try to fetch failed requests.")]
  private Boolean NoRetries { get; set; } = false;

  public void OnExecute()
  {
    var collectionIds = this.CollectionIds;
    Console.WriteLine($"Resolving {collectionIds.Count} collections...");

    // Metrics
    var overallRequested = 0;
    var overallLoss = 0;
    var batchCount = 0;

    while (collectionIds.Count > 0) {
      var timestamp = DateTime.Now;
      var successfulCollections = new List<String>();
      var batch = collectionIds.Take(this.Size).ToList();
      var collections = Resolver.LookupCollections(batch);
      collections.ForEach(collection => {
        Repository.UpdateCollection(collection);
        successfulCollections.Add(collection.CollectionId);
      });
      collectionIds.RemoveRange(0, batch.Count);

      var failedCollections = batch.Except(successfulCollections).ToList();
      failedCollections.ForEach(failedCollection => {
        Repository.UpdateFailedCollection(failedCollection);
      });

      ++batchCount;
      overallRequested += batch.Count;
      overallLoss += failedCollections.Count;
      Console.WriteLine($"{batchCount}: {successfulCollections.Count} of {batch.Count} collections resolved in {(DateTime.Now - timestamp).TotalMilliseconds}ms.");

      this.CoolDown(timestamp);
    }

    var overallLossPercent = ((Double)overallLoss / (Double)(overallRequested)) * 100;
    Console.WriteLine($"{overallLossPercent:0.00}% collection requests failed.");
  }

  private List<String> CollectionIds => (this.NoRetries == true) ? Repository.SelectIncomingCollectionIds(this.Count) : Repository.SelectPendingCollectionIds(this.Count);

  private void CoolDown(DateTime timestamp)
  {
    var delta = DateTime.Now - timestamp;
    if (delta.TotalMilliseconds < this.Delay) {
      Thread.Sleep(this.Delay - (Int32)delta.TotalMilliseconds);
    }
  }
}
