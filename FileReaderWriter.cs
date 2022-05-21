using System.Text;
using FileHelpers;

namespace Fruitshop;

public class FileReaderWriter
{
  public static void ExportPodcasts(String filePath, List<Collection> collections)
  {
    try {
      var engine = new FileHelperEngine<Collection>(Encoding.UTF8);
      engine.WriteFile(filePath, collections);
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
    }
  }

  public static List<Collection> ImportPodcasts(String filePath)
  {
    var collections = new List<Collection>();
    try {
      var engine = new FileHelperEngine<Collection>(Encoding.UTF8);
      collections = engine.ReadFile(filePath).ToList();
    }
    catch (Exception e) {
      Console.WriteLine(e.Message);
      collections.Clear();
    }
    return collections;
  }
}
