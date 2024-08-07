﻿namespace AutoTravel2.Integration;

public interface IScreenReader
{
    public bool SayWithMenuChecker(string text, bool interrupt, string? customQuery = null);

    public string PrevMenuQueryText { get; set; }

    public void RegisterCustomMenuAsAccessible(string? fullNameOfClass);
}
