//UNITY_SHADER_NO_UPGRADE
#ifndef WAVEFUNCTIONSINCLUDE_INCLUDED
#define WAVEFUNCTIONSINCLUDE_INCLUDED

// This is included in Macros.hlsl but I seem to be unable to include it. Not sure why
#ifndef PI
#define PI 3.141592654
#endif

// Masks used to check which waves to add together
#ifndef Wave1Mask1
#define Wave1Mask1 1
#define Wave2Mask1 1 << 1
#define Wave3Mask1 1 << 2
#endif

#ifndef H // We may want to set this in the function call. Probably needs a little playing around with
#define H 0.1
#endif

float2 ConvertUVToCoordinate(float2 uV)
{
    float x = uV.x * 2.0 * PI;
    float y = (uV.y * 2.0) - 1.0;
    
    return float2(x, y);
}

bool CheckMask(int waveType, int mask)
{
    return (waveType & mask) != 0;
}

float CalculateWave1(float x, float v)
{
    return sin(x * v);
}

float CalculateWave2(float x, float v)
{
    return cos(pow(x, v));
}

float CalculateWave3(float x, float v)
{
    float cosinePart = cos(2.0 * x * v);
    float otherPart = pow(v, 2.0) * x;
    return sin(cosinePart + otherPart);
}

float CalculateWavePartValue(float x, float variableValue, int waveType)
{
    float y = 0.0;
    int wavesAppliedCount = 0;
    
    if (CheckMask(waveType, Wave1Mask1))
    {
        y += CalculateWave1(x, variableValue);
        wavesAppliedCount++;
    }
    
    if (CheckMask(waveType, Wave2Mask1))
    {
        y += CalculateWave2(x, variableValue);
        wavesAppliedCount++;
    }
    if (CheckMask(waveType, Wave3Mask1))
    {
        y += CalculateWave3(x, variableValue);
        wavesAppliedCount++;
    }
    
    if (wavesAppliedCount > 1)
        return y / (float) wavesAppliedCount;
    
    return y;
}

float CalculateWaveValue(float x, float3 variableValues, int3 waveTypes)
{
    float y = 0.0;
    int wavesAppliedCount = 0;
    
    if (waveTypes.x != 0)
    {
        y += CalculateWavePartValue(x, variableValues.x, waveTypes.x);
        wavesAppliedCount++;
    }
    
    if (waveTypes.y != 0)
    {
        y += CalculateWavePartValue(x, variableValues.y, waveTypes.y);
        wavesAppliedCount++;
    }
    
    if (waveTypes.z != 0)
    {
        y += CalculateWavePartValue(x, variableValues.z, waveTypes.z);
        wavesAppliedCount++;
    }
    
    if (wavesAppliedCount > 1)
        y = y / wavesAppliedCount;
    
    return y;
}

float2 CalculateWaveCoordinate(float x, float3 variableValues, int3 waveTypes)
{
    float2 coordinate = { x, CalculateWaveValue(x, variableValues, waveTypes) };
    return coordinate;
}

//TODO save the last variable values so we can use that to show the last drawn wave

void IsInWave_float(float4 uV, int3 waveTypes, float3 variableValues, float distortionPercent, float noise, float timeValue, out float Out)
{
    // Get the coordinate x:[0, 2pi] y:[-1, 1]
    float2 coordinate = ConvertUVToCoordinate(uV.xy);    
    
    // Expand the y range by a bit so we don't cutoff the tip of the waves
    coordinate.y *= 1.25;
    
    // Get the coordinate of the wave with our current x
    float2 waveCoordinate = CalculateWaveCoordinate(coordinate.x, variableValues, waveTypes);
    
    // Factor in noise. We are only going to factor in up to 50% noise. At 50% its pretty much impossible
    float correctedDistorationPerecent = distortionPercent * .5;
    waveCoordinate.y = lerp(waveCoordinate.y, noise, correctedDistorationPerecent);
        
    // How far away are we on the y axis. This is the core factor we use to determine if we are in the wave
    float diff = abs(waveCoordinate.y - coordinate.y);
    
    // Bring our difference to a range of [0, 1] where 1 is closest to the wave and 0 is furthest. Make it a fall off faster by making bringing to a power 
    float pixelValue = (clamp(diff, 0.0, 1.0) - 1.0) * -1.0;
    pixelValue = pow(pixelValue, 5.0);
    
    // Now that we have the distance, we want to factor in the time
    float xTime = timeValue % (2.0 * PI);
    float brightnessRange = 0.1f;
    float xTimeDiff = abs(coordinate.x - xTime);
    float min = coordinate.x < xTime ? .75f : .5f;
    float modifier = lerp(1.0, min, clamp(xTimeDiff / brightnessRange, 0, 1.0));
    
    Out = pixelValue * modifier;
}

#endif //WAVEFUNCTIONSINCLUDE_INCLUDED