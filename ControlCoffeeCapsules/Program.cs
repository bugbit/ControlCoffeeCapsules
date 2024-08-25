using ControlCoffeeCapsules;

var commandHandler = new CommandHandler();

WriteLine(Resources.ResourceManager.GetString("Welcome"));

await commandHandler.ExecuteAsync(Commands.Help);
await commandHandler.ExecuteAsync(Commands.Load);
await commandHandler.ExecuteAsync(Commands.Show);

Command cmd = new();

do
{
    WriteLine(Resources.ResourceManager.GetString("CommandMsg"));

    var line = ReadLine();

    if (!string.IsNullOrEmpty(line))
    {
        cmd.Parse(line);
        await commandHandler.ExecuteAsync(cmd);
    }
} while (!commandHandler.IsQuit);