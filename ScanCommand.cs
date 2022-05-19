namespace Fruitshop;

[Command(Name = "scan", Description = "Scan Apple Podcasts directory for new podcasts.")]
public class ScanCommand
{
  private String _url = "https://podcasts.apple.com/de/genre/podcasts/id26";

  [Option(LongName = "source", ShortName = "s", Description = "The directory url to scan.")]
  private String Url { get; set; } = String.Empty;

  public void OnExecute()
  {
    if (this.Url != String.Empty) {
      this._url = this.Url;
    }

    var storedIds = Repository.SelectActiveCollectionIds().ToHashSet();
    Console.WriteLine($"Read {storedIds.Count} podcasts from local repository.");

    var scanId = Repository.StartScan();
    var fetchedIds = Scanner.ReadCollectionIds(this._url);
    Console.WriteLine($"Read {fetchedIds.Count} podcasts from Apple Podcasts directory.");
    Repository.StopScan(scanId);

    var newIds = fetchedIds.Except(storedIds).ToHashSet();
    if (newIds.Count > 0) {
      Console.WriteLine($"Discovered {newIds.Count} new podcasts.");
      Repository.InsertCollectionIds(scanId, newIds.ToList());
    }

    var retiredIds = storedIds.Except(fetchedIds).ToHashSet();
    if (retiredIds.Count > 0) {
      Console.WriteLine($"Retired {retiredIds.Count} podcasts.");
      Repository.RetireCollectionIds(scanId, retiredIds.ToList());
    }

    Console.WriteLine($"=> {Repository.CountCollectionIds()}");
  }
}
