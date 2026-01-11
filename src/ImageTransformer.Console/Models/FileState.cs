namespace ImageTransformer.Console.Models;

internal record FileState
{
    public double Percent { get; init; }
    public bool IsFit { get; init; }
    public string FilePath { get; init; }
}
