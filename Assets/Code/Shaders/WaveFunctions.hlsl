//UNITY_SHADER_NO_UPGRADE
#ifndef WAVEFUNCTIONSINCLUDE_INCLUDED
#define WAVEFUNCTIONSINCLUDE_INCLUDED

// This is included in Macros.hlsl but I seem to be unable to include it. Not sure why
#define PI 3.141592654

// Masks used to check which waves to add together
#define SinMask 1
#define CosMask 1 << 1
#define SawMask 1 << 2

float MapTo0To2Pi(int waveType, float x, float v);

void MapTo0To2Pi_float(int waveType, float x, float variableValue, out float Out)
{
    Out = MapTo0To2Pi(waveType, x, variableValue);
}

void MapTo0To2Pi_float(float x, int3 waveTypes, float3 variableValues, out float Out)
{
    Out = 0.0;
    
    int count = 0;
    if (waveTypes.r != 0)
    {
        Out += MapTo0To2Pi(waveTypes.r, x, variableValues.r);
        count++;
    }
    if (waveTypes.g != 0)
    {
        Out += MapTo0To2Pi(waveTypes.g, x, variableValues.g);
        count++;
    }
    if (waveTypes.b != 0)
    {
        Out += MapTo0To2Pi(waveTypes.b, x, variableValues.b);
        count++;
    }
    
    if (count > 0)
        Out = Out / count;
}

bool CheckMask(int waveType, int mask);
float Saw(float x, float v);

float MapTo0To2Pi(int waveType, float x, float v)
{
    int wavesApplied = 0;
    float returnValue = 0.0;
    
    if (CheckMask(waveType, SinMask))
    {
        returnValue += sin(x * v);
        wavesApplied++;
    }
    if (CheckMask(waveType, CosMask))
    {
        returnValue += cos(x * v);
        wavesApplied++;
    }
    if (CheckMask(waveType, SawMask))
    {
        returnValue += Saw(x, v);
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