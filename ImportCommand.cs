namespace Fruitshop;

[Command(Name = "import", Description = "Import podcasts from .csv file.")]
public class ImportCommand
{
  [Option(LongName = "source", ShortName = "s", Description = "Specify input file")]
  private String Source { get; set; } = String.Empty;

  public void OnExecute()
  {
  }
}
