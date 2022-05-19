using HtmlAgilityPack;

namespace Fruitshop;

public class Scanner
{
  public static HashSet<String> ReadCollectionIds(String url)
  {
    var items = new ConcurrentHashSet<String>();
    if (url != String.Empty) {
      var categories = ReadCategories(url);
      Parallel.ForEach(categories, category => {
        var letters = ReadCategoryLetters(category);
        Parallel.ForEach(letters, letter => {
          var numbers = ReadCategoryLetterNumbers(letter);
          Parallel.ForEach(numbers, (String number, ParallelLoopState state) => {
            var (ids, completed) = ReadPodcasts(number);
            items.AddRange(ids);
            if (completed == true) {
              state.Break();
            }
          });
        });
      });
    }
    return items.ToHashSet();
  }

  public static List<String> ReadCategories(String url)
  {
    var categories = new ConcurrentHashSet<String>();
    if (TryLoadDocument(url, out var document)) {
      Parallel.ForEach(document!.DocumentNode.SelectNodes("//a[@href]"), link => {
        var href = link.Attributes["href"].Value;
        if (href.StartsWith("https://podcasts.apple.com/de/genre/podcasts-")) {
          categories.Add(href);
        }
      });
    }
    return categories.ToList();
  }

  public static List<String> ReadCategoryLetters(String url)
  {
    var pages = new ConcurrentHashSet<String>();
    if (TryLoadDocument(url, out var document)) {
      Parallel.ForEach(document!.DocumentNode.SelectNodes("//a[@href]"), link => {
        var href = link.Attributes["href"].Value;
        if (href.Contains("?letter=")) {
          pages.Add(href);
        }
      });
    }
    return pages.ToList();
  }

  public static List<String> ReadCategoryLetterNumbers(String url)
  {
    var numbers = new ConcurrentHashSet<String>();
    var baseUrl = GetCategoryLetterBaseUrl(url);
    if (String.IsNullOrEmpty(baseUrl) == false) {
      for (var i = 1; i < 100; i++) {
        var pagedUrl = $"{baseUrl}{i}#page";
        numbers.Add(pagedUrl);
      }
    }
    return numbers.ToList();
  }

  private static String GetCategoryLetterBaseUrl(String url)
  {
    var result = "";
    if (TryLoadDocument(url, out var document)) {
      var nodes = document!.DocumentNode.SelectNodes("//a[@href]");
      foreach (var node in nodes) {
        var href = node.GetAttributeValue("href", null);
        href = href.Replace("&amp;", "&");
        if (href != null) {
          var startIndex = href.IndexOf("page=");
          if (startIndex > 0) {
            result = href[0..(startIndex + 5)];
            return result;
          }
        }
      }
    }
    return result;
  }

  public static (List<String> ids, Boolean completed) ReadPodcasts(String url)
  {
    var completed = true;
    var ids = new List<String>();
    if (TryLoadDocument(url, out var document)) {
      var nodes = document!.DocumentNode.SelectNodes("//a[@href]");
      foreach (var node in nodes) {
        var href = node.GetAttributeValue("href", null);
        if (href != null) {
          if (href.StartsWith("https://podcasts.apple.com/de/podcast/")) {
            var id = ParsePodcastId(href);
            if (String.IsNullOrEmpty(id) == false) {
              ids.Add(id[2..(id.Length)]);
            }
          }
        }
        nodes = document.DocumentNode.SelectNodes("//a[@class='paginate-more']");
        if (nodes != null) {
          completed = false;
        }
      }
    }
    return (ids, completed);
  }

  private static String ParsePodcastId(String url)
  {
    var id = "";
    var offset = url.LastIndexOf("/");
    if (offset > 0) {
      id = url.Substring(offset + 1, url.Length - offset - 1);
    }
    return id;
  }

  private static Boolean TryLoadDocument(String url, out HtmlDocument? document)
  {
    document = null;
    var result = false;
    var retryCount = 3;
    while ((result == false) && (retryCount > 0)) {
      retryCount--;
      try {
        var reader = new HtmlAgilityPack.HtmlWeb();
        document = reader.Load(url);
        result = true;
      }
      catch (Exception e) {
        Console.WriteLine($"Retry: {retryCount} Error: {e.Message}");
        Thread.Sleep(1000);
      }
    }
    return result;
  }
}
