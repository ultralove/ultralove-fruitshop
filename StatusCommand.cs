namespace Fruitshop;

[Command(Name = "status", Description = "Show the database status.")]
public class StatusCommand
{
  public void OnExecute()
  {
    var overallCollections = Repository.CountCollectionIds();
    var activeCollections = Repository.CountActiveCollectionIds();
    var incomingCollections = Repository.CountIncomingCollectionIds();
    var pendingCollections = Repository.CountPendingCollectionIds();
    var retiredCollection = Repository.CountRetiredCollectionIds();

    Console.WriteLine("Database status:");
    Console.WriteLine($"  - Overall collections:        {overallCollections}");
    Console.WriteLine($"    - Active collections:       {activeCollections}");
    Console.WriteLine($"      - Resolved collections:   {activeCollections - pendingCollections}");
    Console.WriteLine($"      - Pending collections:    {pendingCollections}");
    Console.WriteLine($"        - Incoming collections: {incomingCollections}");
    Console.WriteLine($"    - Retired collections:      {retiredCollection}");
  }
}
