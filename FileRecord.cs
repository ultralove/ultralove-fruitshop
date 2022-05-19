using FileHelpers;

namespace Fruitshop;

[DelimitedRecord(";")]
public class FileRecord
{
  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public String CollectionId { get; set; } = String.Empty;

  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public String CollectionName { get; set; } = String.Empty;

  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public String FeedUrl { get; set; } = String.Empty;

  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  [FieldConverter(ConverterKind.Date, "yyyyMMddTHHmmsszzz")]
  public DateTime FirstSeen { get; set; } = DateTime.MinValue;

  [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForBoth)]
  public Int32 RetryCount { get; set; } = -1;
}
