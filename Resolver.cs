using System.Net.Http.Headers;

namespace Fruitshop;

public class Resolver
{
  // private static readonly HttpClient s_client = new();
  public static List<Collection> LookupCollections(List<String> collectionIds)
  {
    var result = new List<Collection>();
    if (collectionIds.Count > 0) {
      var url = BuildRequestUrl(collectionIds);
      Task.Run(async () => result = await RequestCollections(url)).Wait();
    }
    return result;
  }

  private static String BuildRequestUrl(List<String> collectionIds)
  {
    var url = "https://itunes.apple.com/lookup?id=";
    for (var i = 0; i < collectionIds.Count; i++) {
      url += collectionIds[i];
      if (i < (collectionIds.Count - 1)) {
        url += ",";
      }
    }
    return url;
  }

  private static async Task<List<Collection>> RequestCollections(String url)
  {
    var collections = new List<Collection>();

    var client = new HttpClient();
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("User-Agent", Configuration.UserAgent);
    try {
      var json = await client.GetStringAsync(url);
      using var document = JsonDocument.Parse(json);
      var resultCount = document.RootElement.GetProperty("resultCount").GetInt32();
      if (resultCount > 0) {
        var results = document.RootElement.GetProperty("results");
        for (var i = 0; i < results.EnumerateArray().Count(); i++) {
          if (results[i].TryGetProperty("collectionId", out var collectionId)) {
            if (results[i].TryGetProperty("collectionName", out var collectionName)) {
              if (results[i].TryGetProperty("feedUrl", out var feedUrl)) {
                collections.Add(new Collection { CollectionId = collectionId.GetInt32().ToString(), CollectionName = collectionName.GetString(), FeedUrl = feedUrl.GetString() });
              }
            }
          }
        }
      }
    }
    // DeserializeAsync
    catch (JsonException e) {
      Console.WriteLine($"{e.Message}");
    }
    catch (NotSupportedException e) {
      Console.WriteLine($"{e.Message}");
    }
    catch (ArgumentNullException e) {
      Console.WriteLine($"{e.Message}");
    }
    // Pass on network errors...
    return collections;
  }
}
