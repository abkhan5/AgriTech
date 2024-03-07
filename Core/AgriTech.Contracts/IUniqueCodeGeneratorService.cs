namespace AgriTech;

public interface IUniqueCodeGeneratorService
{
    public string GetUniqueCode(string inputString = null);

    public string GetPassword();

    public string CreateToken(string inputString);
    public string OriginalInput(string token);
    string TimeBasedToken(string inputstring);
    string OriginalTimeBasedToken(string token);
}