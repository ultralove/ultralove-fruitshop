using System.Text;
using FileHelpers;

namespace Fruitshop;

public class FileReaderWriter
{
  public static void ExportPodcasts(String filePath, List<Podcast> podcasts)
  {
    try {
      var engine = new FileHelperEngine<Podcast>(Encoding.UTF8);
      engine.WriteFile(filePath, podcasts);
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
  }

  public static List<Podcast> ImportPodcasts(String filePath)
  {
    var podcasts = new List<Podcast>();
    try {
      var engine = new FileHelperEngine<Podcast>(Encoding.UTF8);
      podcasts = engine.ReadFile(filePath).ToList();
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
      podcasts.Clear();
    }
    return podcasts;
  }
}
