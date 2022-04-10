using Ultralove;

// var items = new ConcurrentHashSet<String>();
// var categories = AppleScanner.ReadCategories("https://podcasts.apple.com/de/genre/podcasts/id26");
// Parallel.ForEach(categories, category => {
//   var letters = AppleScanner.ReadCategoryLetters(category);
//   Parallel.ForEach(letters, letter => {
//     var numbers = AppleScanner.ReadCategoryLetterNumbers(letter);
//     Parallel.ForEach(numbers, (String number, ParallelLoopState state) => {
//       var (ids, completed) = AppleScanner.ReadPodcasts(number);
//       items.AddRange(ids);
//       if (completed == true) {
//         state.Break();
//       }
//     });
//   });
// });

var items = AppleScanner.ReadCollectionIds("https://podcasts.apple.com/de/genre/podcasts/id26");
Console.WriteLine($"Found {items.Count} podcasts in Apple Podcasts.");
if (SqlServerReaderWriter.CountCollectionIds() == 0) {
  SqlServerReaderWriter.FastInsertCollectionIds(items.ToList());
}
else {
  SqlServerReaderWriter.InsertCollectionIds(items.ToList());
}
Console.WriteLine($"Saved {SqlServerReaderWriter.CountCollectionIds()} to database.");
