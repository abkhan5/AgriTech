namespace AgriTech.Contracts.Options;

public class ApplicationProfileSetting
{
    public const string ApplicationProfileOptions = "ApplicationProfileSetting";
    public string ApplicationName { get; set; }
    public string BuildNumber { get; set; }

    public string ApplicationEnv { get; set; }
}