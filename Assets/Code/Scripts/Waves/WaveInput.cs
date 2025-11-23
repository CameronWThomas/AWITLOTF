using System.Collections.Generic;
using UnityEngine;

public class WaveInput : MonoBehaviour
{
    // Right now it does it by keys out of simplicity
    private static readonly Dictionary<int, (KeyCode, KeyCode)> InputDict = new Dictionary<int, (KeyCode, KeyCode)>()
    {
        { 1, (KeyCode.Q, KeyCode.A) },
        { 2, (KeyCode.W, KeyCode.S) },
        { 3, (KeyCode.E, KeyCode.D) },
    };

    // Update is called once per frame
    void Update()
    {
        
    }
}
