public enum WaveTrait
{
    Mind = 1,
    Spirit = 2,
    Body = 3
}

public static class WaveTraitExtensions
{
    public static WaveType ToWaveType(this WaveTrait waveTrait)
    {
        return waveTrait switch
        {
            WaveTrait.Mind => WaveType.Wave1,
            WaveTrait.Spirit => WaveType.Wave2,
            WaveTrait.Body => WaveType.Wave3,
            _ => throw new System.NotImplementedException()
        };
    }
}