public enum WaveTrait
{
    Body = 1,
    Mind = 2,
    Spirit = 3
}

public static class WaveTraitExtensions
{
    public static WaveType ToWaveType(this WaveTrait waveTrait)
    {
        return waveTrait switch
        {
            WaveTrait.Body => WaveType.Wave1,
            WaveTrait.Mind => WaveType.Wave2,
            WaveTrait.Spirit => WaveType.Wave3,
            _ => throw new System.NotImplementedException()
        };
    }
}