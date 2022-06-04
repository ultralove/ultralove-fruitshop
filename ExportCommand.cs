using NodaTime;
using FileHelpers;
namespace Fruitshop;

[Command(Name = "export", Description = "Export podcasts to .csv file.")]
public class ExportCommand
{
  [Option(LongName = "output", ShortName = "o", Description = "Specify output file")]
  private String Output { get; set; } = String.Empty;

  public void OnExecute()
  {
    if ((String.IsNullOrEmpty(this.Output) == true) || (this.Output.ToLower() == "default")) {
      var now = SystemClock.Instance.GetCurrentInstant();
      this.Output = String.Format(@"fruitshop-{0}.csv", now.ToString().Replace(':', '-'));
    }
    if (String.IsNullOrEmpty(this.Output) == false) {
      var collections = Repository.SelectCollections();
      collections.RemoveAll(collection => ((collection.CollectionName == String.Empty) || (collection.FeedUrl == String.Empty)));
      var exportedItems = WriteCollections(this.Output, collections);
      Console.WriteLine($"Exported {exportedItems} collection(s) to {this.Output}.");
    }
  }

  private static Int32 WriteCollections(String output, List<Collection> collections)
  {
    var exportedItems = 0;
    if (String.IsNullOrEmpty(output) == false) {
      if (collections.Count > 0) {
        try {
          var engine = new FileHelperEngine<Collection>(Encoding.UTF8);
          engine.WriteFile(output, collections);
          exportedItems = collections.Count;
        }
        catch (Exception e) {
          Console.WriteLine(e.Message);
          Console.WriteLine(e.StackTrace);
          exportedItems = 0;
        }
      }
    }
    return exportedItems;
  }
}
