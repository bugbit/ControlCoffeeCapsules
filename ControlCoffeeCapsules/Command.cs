namespace ControlCoffeeCapsules;

public struct Command
{
    public string cmd;
    public string[] args;

    public void Parse(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        cmd = parts[0];
        args = new string[parts.Length - 1];

        for (int i = 0; i < args.Length; i++)
        {
            args[i] = parts[i + 1];
        }
    }
}
