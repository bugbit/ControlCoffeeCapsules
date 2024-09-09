namespace ControlCoffeeCapsules;

public class CommandHandler
{
    private readonly (string cmd, Func<string[]?, Task> executeCmd, string resourceName)[] cmds;

    private ControlData? controlData;
    private readonly IProviderFileControlData providerFileControlData;

    public CommandHandler()
    {
        cmds = [
            ("help",Help,"HelpUseCmd"),
            ("load",Load,"LoadUseCmd"),
            ("save",Save,"SaveUseCmd"),
            ("show",Show,"ShowUseCmd"),
            ("quit",Quit,"QuitUseCmd"),
            ];
        providerFileControlData = FactoryProviderFileControlData.GetProvider();
    }

    public bool IsQuit { get; private set; }

    public async Task ExecuteAsync(Command command)
    {
        foreach (var cmd in cmds)
        {
            if (string.Equals(cmd.cmd, command.cmd, StringComparison.CurrentCultureIgnoreCase))
            {
                await cmd.executeCmd(command.args);

                return;
            }
        }

        WriteLine(Resources.ResourceManager.GetString("CmdNotFound"));

        await Task.CompletedTask;
    }

    private Task Help(string[]? _)
    {
        WriteLine(Resources.ResourceManager.GetString("UseMsg"));
        foreach (var cmd in cmds)
        {
            WriteLine($"  {cmd.cmd} - {Resources.ResourceManager.GetString(cmd.resourceName)}");
        }

        return Task.CompletedTask;
    }

    private async Task Load(string[]? _)
    {
        await providerFileControlData.ReadFile();
    }

    private async Task Save(string[]? _)
    {
        controlData ??= new ControlData { Capsules = 20, CapsulesToKit = 600 };
        await providerFileControlData.SaveFile(controlData!);
    }

    private Task Show(string[]? _)
    {
        if (controlData is not null)
            WriteLine("Capsules: {0} CapsulesToKit:{1}", controlData.Capsules, controlData.CapsulesToKit);

        return Task.CompletedTask;
    }

    private Task Quit(string[]? _)
    {
        IsQuit = true;

        return Task.CompletedTask;
    }
}
