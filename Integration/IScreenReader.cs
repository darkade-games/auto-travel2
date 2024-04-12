namespace AutoTravel2.Integration;

public interface IScreenReader
{
    public bool SayWithMenuChecker(string text, bool interrupt, string customQuery = null);
}
