using FileHelpers;

namespace Fruitshop;

[Command(Name = "import", Description = "Import podcasts from .csv file.")]
public class ImportCommand
{
  [Option(LongName = "input", ShortName = "i", Description = "Specify input file")]
  private String? Input { get; set; } = String.Empty;

  public void OnExecute()
  {
    if ((String.IsNullOrEmpty(this.Input) == true) || (this.Input.ToLower() == "default")) {
      this.Input = Directory.EnumerateFiles(".", "fruitshop-*.csv", SearchOption.TopDirectoryOnly).ToList()
                             .OrderByDescending(file => File.GetLastWriteTimeUtc(file)).ToList()
                             .FirstOrDefault();
    }
    if (String.IsNullOrEmpty(this.Input) == false) {
      try {
        var existingCollectionIds = Repository.SelectCollectionIds().ToHashSet();
        var delta = ReadCollections(this.Input).Where(newCollection => existingCollectionIds.Contains(newCollection.CollectionId) == false).ToList();
        if (delta.Count > 0) {
          var scanId = Repository.StartScan();
          Repository.InsertCollections(scanId, delta);
          Repository.StopScan(scanId);
          Console.WriteLine($"Read {delta.Count} collection(s) from {this.Input}.");
        }
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
      }
    }
  }

  public static List<Collection> ReadCollections(String input)
  {
    var collections = new List<Collection>();
    if (String.IsNullOrEmpty(input) == false) {
      try {
        var engine = new FileHelperEngine<Collection>(Encoding.UTF8);
        collections = engine.ReadFile(input).ToList();
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
        collections.Clear();
      }
    }
    return collections;
  }

}
