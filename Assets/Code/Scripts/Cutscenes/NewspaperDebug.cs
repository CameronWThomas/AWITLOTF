using System;
using UnityEngine;

[ExecuteInEditMode]
public class NewspaperDebug : MonoBehaviour
{
    [Range(0f, 1f)] public float T = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isEditor || Application.isPlaying)
            return;


        var newspaper = GetComponent<Newspaper>();
        newspaper.SetPositionAndRotation(T);
    }
}
