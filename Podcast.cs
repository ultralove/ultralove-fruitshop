using System.Text.Json.Serialization;

namespace Ultralove;

public class Podcast
{
  [JsonPropertyName("collectionId")]
  public String CollectionId { get; set; } = String.Empty;

  [JsonPropertyName("collectionName")]
  public String CollectionName { get; set; } = String.Empty;

  [JsonPropertyName("feedUrl")]
  public String FeedUrl { get; set; } = String.Empty;

  [JsonIgnore]
  public DateTime LastSeen { get; set; } = DateTime.MinValue;

  [JsonIgnore]
  public Int32 RetryCount { get; set; } = Int32.MinValue;
}
