namespace Fruitshop;

public class Collection
{
  [JsonPropertyName("collectionId")]
  public String CollectionId { get; set; } = String.Empty;

  [JsonPropertyName("collectionName")]
  public String? CollectionName { get; set; } = String.Empty;

  [JsonPropertyName("feedUrl")]
  public String? FeedUrl { get; set; } = String.Empty;
}
