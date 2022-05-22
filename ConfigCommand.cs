namespace Fruitshop;

[Command(Name = "config", Description = "Override configuration values.")]
public class ConfigCommand
{
  [Option(LongName = "storagePath", ShortName = "", Description = "Specify the store path.")]
  private String StoragePath { get; set; } = String.Empty;

  [Option(LongName = "databaseEngine", ShortName = "", Description = "Specify the database engine.")]
  private String DatabaseEngine { get; set; } = String.Empty;

  [Option(LongName = "userAgent", ShortName = "", Description = "Specify the user agent for http requests.")]
  private String UserAgent { get; set; } = String.Empty;

  [Option(LongName = "dump", ShortName = "d", Description = "Write current configuration to file.")]
  private Boolean Dump { get; set; } = false;

  [Option(LongName = "get", ShortName = "", Description = "Print current configuration.")]
  private Boolean Get { get; set; } = false;

  public void OnExecute()
  {
    if (String.IsNullOrEmpty(this.StoragePath) == false) {
      Configuration.StoragePath = this.StoragePath;
    }
    if (String.IsNullOrEmpty(this.DatabaseEngine) == false) {
      Configuration.DatabaseEngine = this.DatabaseEngine;
    }
    if (String.IsNullOrEmpty(this.UserAgent) == false) {
      Configuration.UserAgent = this.UserAgent;
    }
    if (this.Dump == true) {
      Configuration.Dump();
    }
    if (this.Get) {
      Console.WriteLine($"DatabaseEngine: {Configuration.DatabaseEngine}");
      Console.WriteLine($"StoragePath:    {Configuration.StoragePath}");
      Console.WriteLine($"UserAgent:      {Configuration.UserAgent}");
    }
  }
}
