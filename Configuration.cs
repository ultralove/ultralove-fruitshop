namespace Fruitshop;

public sealed class Configuration
{
  public static String StorePath
  {
    get => s_values.StorePath;
    set => s_values.StorePath = value;
  }

  public static String DatabaseEngine
  {
    get => s_values.DatabaseEngine;
    set => s_values.DatabaseEngine = value;
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
    if (Directory.Exists(StorePath) == false) {
      _ = Directory.CreateDirectory(StorePath);
    }
  }

  private static String DataPath => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "fruitshop");
  private static String DataFilePath => Path.Combine(DataPath, "fruitshop.json");

  private class Values
  {
    [JsonPropertyName("storePath")]
    public String StorePath { get; set; } = Path.Combine(DataPath, "store");
    [JsonPropertyName("databasesEngine")]
    public String DatabaseEngine { get; set; } = "mssql";
  }

  private static readonly Values s_values = new();
}
