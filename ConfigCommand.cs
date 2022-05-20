namespace Fruitshop;

[Command(Name = "config", Description = "Override configuration values.")]
public class ConfigCommand
{
  [Option(LongName = "storagePath", ShortName = "", Description = "Specify the store path.")]
  private String StorePath { get; set; } = String.Empty;

  [Option(LongName = "databaseEngine", ShortName = "", Description = "Specify the database engine.")]
  private String DatabaseEngine { get; set; } = String.Empty;

  [Option(LongName = "dump", ShortName = "d", Description = "Write current configuration to file.")]
  private Boolean Dump { get; set; } = false;

  [Option(LongName = "get", ShortName = "", Description = "Print current configuration.")]
  private Boolean Get { get; set; } = false;

  public void OnExecute()
  {
    if (String.IsNullOrEmpty(this.StorePath) == false) {
      Configuration.StoragePath = this.StorePath;
    }
    if (String.IsNullOrEmpty(this.DatabaseEngine) == false) {
      Configuration.DatabaseEngine = this.DatabaseEngine;
    }
    if (this.Dump == true) {
      Configuration.Dump();
    }
    if (this.Get) {
      Console.WriteLine($"DatabaseEngine: {Configuration.DatabaseEngine}");
      Console.WriteLine($"StoragePath:    {Configuration.StoragePath}");
    }
  }
}
