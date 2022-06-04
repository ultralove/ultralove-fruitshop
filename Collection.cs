using FileHelpers;
namespace Fruitshop;

[DelimitedRecord(",")]
public class Collection
{
  [JsonPropertyName("collectionId")]
  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public String CollectionId { get; set; } = String.Empty;

  [JsonPropertyName("collectionName")]
  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public String? CollectionName { get; set; } = String.Empty;

  [JsonPropertyName("feedUrl")]
  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public String? FeedUrl { get; set; } = String.Empty;
}
