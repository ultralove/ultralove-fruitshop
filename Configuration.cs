namespace Fruitshop;

public sealed class Configuration
{
  public static String StoragePath
  {
    get => s_values.StoragePath;
    set => s_values.StoragePath = value;
  }

  public static String DatabaseEngine
  {
    get => s_values.DatabaseEngine;
    set => s_values.DatabaseEngine = value;
  }

  public static String UserAgent
  {
    get => s_values.UserAgent;
    set => s_values.UserAgent = value;
  }

  public static void Dump()
  {
    var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize<Values>(s_values, serializerOptions);
    if (String.IsNullOrEmpty(json) == false) {
      File.WriteAllText(DataFilePath, json);
    }
  }

  static Configuration()
  {
    if (File.Exists(DataPath) == false) {
      _ = Directory.CreateDirectory(DataPath);
    }
    else {
      if (File.Exists(DataFilePath) == true) {
        var json = File.ReadAllText(DataFilePath);
        if (String.IsNullOrEmpty(json) == false) {
          s_values = JsonSerializer.Deserialize<Values>(json) ?? new();
        }
      }
    }
    if (Directory.Exists(StoragePath) == false) {
      _ = Directory.CreateDirectory(StoragePath);
    }
  }

  private static String DataPath => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "fruitshop");
  private static String DataFilePath => Path.Combine(DataPath, "fruitshop.json");

  private class Values
  {
    [JsonPropertyName("storagePath")]
    public String StoragePath { get; set; } = Path.Combine(DataPath, "storage");

    [JsonPropertyName("databasesEngine")]
    public String DatabaseEngine { get; set; } = "mssql";

    [JsonPropertyName("userAgent")]
    public String UserAgent { get; set; } = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.5 Safari/605.1.15";
  }

  private static readonly Values s_values = new();
}
