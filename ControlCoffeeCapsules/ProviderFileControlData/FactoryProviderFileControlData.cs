namespace ControlCoffeeCapsules.ProviderFileControlData;

public static class FactoryProviderFileControlData
{
    public static IProviderFileControlData GetProvider() => new IOProviderFileControlData();
}
