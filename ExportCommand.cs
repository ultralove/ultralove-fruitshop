namespace Fruitshop;

[Command(Name = "export", Description = "Export podcasts to .csv file.")]
public class ExportCommand
{
  [Option(LongName = "target", ShortName = "t", Description = "Specify output file")]
  private String Target { get; set; } = String.Empty;

  public void OnExecute()
  {
  }
}
