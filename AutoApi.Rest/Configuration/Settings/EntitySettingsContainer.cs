namespace AutoApi.Rest.Configuration.Settings;

public class EntitySettingsContainer : List<EntitySettings>
{
    public EntitySettingsContainer(IEnumerable<EntitySettings> settings)
    {
        AddRange(settings);
    }
}
