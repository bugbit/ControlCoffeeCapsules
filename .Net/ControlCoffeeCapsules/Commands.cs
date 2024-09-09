namespace ControlCoffeeCapsules;

public static class Commands
{
    public static readonly Command Help = new Command { cmd = "help" };
    public static readonly Command Load = new Command { cmd = "load" };
    public static readonly Command Show = new Command { cmd = "show" };
}
