using System.Reflection;

namespace Ultralove;

[Command(Name = "fruitshop"), Subcommand(typeof(ScanCommand), typeof(ResolveCommand), typeof(ConfigCommand))]
[VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
[HelpOption("--help")]

internal class Program
{
  public static void Main(String[] args) => CommandLineApplication.Execute<Program>(args);

  public void OnExecute(CommandLineApplication app)
  {
    app.ShowHelp();
  }

  private static String GetVersion()
  => "fruitshop v" + typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
}
