namespace Fruitshop;

[Command(Name = "resolve", Description = "Fetch name and feed url for new podcasts.")]
public class ResolveCommand
{
  [Option(LongName = "count", ShortName = "c", Description = "Overall number of podcasts to resolve")]
  private Int32 Count { get; set; } = 100;

  [Option(LongName = "size", ShortName = "s", Description = "Number of podcasts to resolve per batch")]
  private Int32 Size { get; set; } = 100;

  [Option(LongName = "delay", ShortName = "d", Description = "Duration(ms) of the cool-down phase inbetween batches")]
  private Int32 Delay { get; set; } = 1000;

  [Option(LongName = "no-retries", ShortName = "n", Description = "Don't retry failed resolve requests.")]
  private Boolean NoRetries { get; set; } = false;

  [Option(LongName = "all", ShortName = "a", Description = "Resolve all pendings podcasts.")]
  private Boolean All { get; set; } = false;


  public void OnExecute()
  {
    if (this.All == true) {
      this.Count = Int32.MaxValue;
    }
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

      var retryCount = 0;
      var timeout = 10000;
      var succeeded = false;
      var collections = new List<Collection>();
      while ((retryCount < 5) && (succeeded == false)) {
        try {
          collections = Resolver.LookupCollections(batch);
          succeeded = true;
        }
        catch (Exception e) {
          Console.WriteLine($"Error({retryCount + 1}): {e.Message}");
          Console.WriteLine($"Cooling down for {timeout / 1000} seconds...");
          retryCount++;
          Thread.Sleep(timeout);
          timeout = (retryCount * timeout * 3);
        }
      }

      if (succeeded == true) {
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
      else {
        Console.WriteLine($"Request {batchCount} finally failed. Exiting...");
        break;
      }
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
