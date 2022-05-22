namespace Fruitshop;

[Command(Name = "find", Description = "Find podcasts.")]
public class FindCommand
{
  [Argument(0, "pattern", "The pattern to search for")]
  private String[]? Patterns { get; } = null;

  public void OnExecute()
  {
    if ((this.Patterns != null) && (this.Patterns.Length > 0)) {
      this.Patterns.ToList().ForEach(pattern => {
        var collections = Repository.FindAnyCollection(pattern);
        collections.ForEach(collection => {
          Console.WriteLine($"{collection.CollectionId}, {collection.CollectionName}, {collection.FeedUrl}");
        });
      });
    }
  }
}
