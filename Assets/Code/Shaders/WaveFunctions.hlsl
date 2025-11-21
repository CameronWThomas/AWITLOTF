//UNITY_SHADER_NO_UPGRADE
#ifndef WAVEFUNCTIONSINCLUDE_INCLUDED
#define WAVEFUNCTIONSINCLUDE_INCLUDED

// This is included in Macros.hlsl but I seem to be unable to include it. Not sure why
#define PI 3.141592654

// Masks used to check which waves to add together
#define Wave1Mask 1
#define Wave2Mask 1 << 1
#define Wave3Mask 1 << 2

float CalcWaveYValue(float x, int3 waveTypes, float3 variableValues, out int waveCount);
float MapTo0To2Pi(int waveType, float x, float v);
bool CheckMask(int waveType, int mask);
float Saw(float x, float v);

void IsCoordinateInWave_float(float uV_x, float uV_y, float width, int3 waveTypes, float3 variableValues, out float Out)
{
    // Taking the x texture value ([0, 1]) and mapping it to [0, 2 pi]
    float x = uV_x * 2.0 * PI;
    
    // Get the y value for the wave
    int waveCount;
    float waveY = CalcWaveYValue(x, waveTypes, variableValues, waveCount);
    
    // Each wave will give values between [-1, 1], so average the number of waves so the output stays in that range
    if (waveCount > 1)
        waveY /= waveCount;
    
    // We will decrease the range by a bit so we don't cut off at the top
    waveY *= 0.8;
    
    // Figure out what the UV coordinate for this y would be (Mapping from from [-1, 1] to [0, 1])
    float waveUV_y = (waveY / 2.0) + 0.5;
    
    float uVDiff = waveUV_y - uV_y;
    uVDiff = abs(uVDiff);
    
    if (uVDiff < width)
        Out = 1.0;
    else
        Out = 0.0;
}

float CalcWaveYValue(float x, int3 waveTypes, float3 variableValues, out int waveCount)
{
    float y = 0.0;
    
    waveCount = 0;
    if (waveTypes.r != 0)
    {
        y += MapTo0To2Pi(waveTypes.r, x, variableValues.r);
        waveCount++;
    }
    if (waveTypes.g != 0)
    {
        y += MapTo0To2Pi(waveTypes.g, x, variableValues.g);
        waveCount++;
    }
    if (waveTypes.b != 0)
    {
        y += MapTo0To2Pi(waveTypes.b, x, variableValues.b);
        waveCount++;
    }
    
    return y;
}

float MapTo0To2Pi(int waveType, float x, float v)
{
    int wavesApplied = 0;
    float returnValue = 0.0;
    
    if (CheckMask(waveType, Wave1Mask))
    {
        returnValue += sin(x * v);
        wavesApplied++;
    }
    if (CheckMask(waveType, Wave2Mask))
    {
        returnValue += cos(pow(x, v));
        wavesApplied++;
    }
    if (CheckMask(waveType, Wave3Mask))
    {
        float cosinePart = cos(2.0 * x * v);
        float otherPart = pow(v, 2.0) * x;
        returnValue += sin(otherPart + cosinePart);
        //returnValue += Saw(x, v);
        wavesApplied++;
    }
    
    if (wavesApplied > 0)
        return returnValue / wavesApplied;
    
    return 0.0;
}

bool CheckMask(int waveType, int mask)
{
    return (waveType & mask) != 0;
}

float Saw(float x, float v)
{
    float sawPeriod = (PI / 2.0) / v;
    float sawDecayPercentage = .08;
    float sawDecayDistance = sawPeriod * sawDecayPercentage;
    float sawDecayPoint = sawPeriod - sawDecayDistance;
    float sawDecayStartHeight = 2.0 * (1.0 - sawDecayPercentage);
        
    float sawX = x % sawPeriod;
        
    float sawValue;
    if (sawX < sawDecayPoint)
    {
        sawValue = sawX * (2.0 / sawPeriod);
    }
    else
    {
        sawValue = -(sawX - sawPeriod) * (sawDecayStartHeight / sawDecayDistance);
    }

    return sawValue - 1.0;
}

#endif //WAVEFUNCTIONSINCLUDE_INCLUDED