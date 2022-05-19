
namespace Ultralove;

public class Podcast
{
  [JsonPropertyName("collectionId")]
  public String CollectionId { get; set; } = String.Empty;

  [JsonPropertyName("collectionName")]
  public String? CollectionName { get; set; } = null;

  [JsonPropertyName("feedUrl")]
  public String? FeedUrl { get; set; } = null;

  [JsonIgnore]
  public Int32 ScanId { get; set; } = Int32.MinValue;

  [JsonIgnore]
  public Int32 RetryCount { get; set; } = -1;
}
