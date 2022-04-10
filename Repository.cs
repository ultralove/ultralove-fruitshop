using System.Text.Json;

namespace Ultralove;
public class Repository
{
  private static readonly HttpClient s_client = new();
  public static async Task<Podcast?> LookupPodcast(String podcastId)
  {
    Podcast? result = null;
    if (String.IsNullOrWhiteSpace(podcastId) == false) {
      s_client.DefaultRequestHeaders.Accept.Clear();
      s_client.DefaultRequestHeaders.Add("User-Agent", "ultralove apple podcasts scanner v1.0.0");
      // client.DefaultRequestHeaders.Accept.Add(
      //         new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

      try {
        var url = $"https://itunes.apple.com/lookup?id={podcastId[2..(podcastId.Length)]}";
        // var stream = await s_client.GetStreamAsync(url);
        // var podcast = await JsonSerializer.DeserializeAsync<Podcast>(stream);
        // Console.WriteLine($"{podcast?.Name}");
        var json = await s_client.GetStringAsync(url);
        Console.WriteLine($"{json}");
      }
      catch (Exception e) {
        Console.WriteLine($"{e.Message}");
      }
    }
    return result;
  }
}
