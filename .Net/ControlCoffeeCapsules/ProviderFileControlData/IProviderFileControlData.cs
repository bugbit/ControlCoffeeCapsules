namespace ControlCoffeeCapsules.ProviderFileControlData;

public interface IProviderFileControlData
{
    Task<ControlData?> ReadFile();
    Task SaveFile(ControlData data);
}
