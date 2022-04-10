using Ultralove;

Console.WriteLine($"Found {SqlServerReaderWriter.CountCollectionIds()} podcasts in the database.");
var items = AppleScanner.ReadCollectionIds("https://podcasts.apple.com/de/genre/podcasts/id26");
Console.WriteLine($"Found {items.Count} podcasts in Apple Podcasts.");
if (SqlServerReaderWriter.CountCollectionIds() == 0) {
  SqlServerReaderWriter.FastInsertCollectionIds(items.ToList());
}
else {
  SqlServerReaderWriter.InsertOrUpdateCollectionIds(items.ToList());
}
Console.WriteLine($"Saved {SqlServerReaderWriter.CountCollectionIds()} to database.");
