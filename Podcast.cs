using System.Text.Json.Serialization;
using FileHelpers;

namespace Ultralove;

[DelimitedRecord(";")]
public class Podcast
{
  [JsonPropertyName("collectionId")]
  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public String CollectionId { get; set; } = String.Empty;

  [JsonPropertyName("collectionName")]
  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public String CollectionName { get; set; } = String.Empty;

  [JsonPropertyName("feedUrl")]
  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public String FeedUrl { get; set; } = String.Empty;

  [JsonIgnore]
  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  [FieldConverter(ConverterKind.Date, "yyyyMMddTHHmmsszzz")]
  public DateTime LastSeen { get; set; } = DateTime.MinValue;

  [JsonIgnore]
  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public Int32 RetryCount { get; set; } = Int32.MinValue;
}
