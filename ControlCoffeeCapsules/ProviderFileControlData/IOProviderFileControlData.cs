
using System.Reflection;

namespace ControlCoffeeCapsules.ProviderFileControlData;

public class IOProviderFileControlData : IProviderFileControlData
{
    private static string file;

    static IOProviderFileControlData()
    {
        file = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".data")!;
    }

    public async Task<ControlData?> ReadFile()
    {
        if (!File.Exists(file))
            return null;

        using var stream = File.OpenRead(file);

        var data = await JsonSerializer.DeserializeAsync(stream, SourceGenerationContext.Default.ControlData);

        return data;
    }

    public async Task SaveFile(ControlData data)
    {
        using var stream = File.Create(file);

        await JsonSerializer.SerializeAsync(stream, data, SourceGenerationContext.Default.ControlData);
    }
}
