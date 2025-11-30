using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour
{
    [Header("Title card stuff")]
    public RawImage title1;
    public RawImage title2;
    [Range(0f, 10f)] public float hold1Duration = 5f;
    [Range(0f, 10f)] public float fade1Duration = 5f;
    [Range(0f, 10f)] public float hold2Duration = 5f;
    [Range(0f, 10f)] public float fade2Duration = 5f;
    [Range(0f, 10f)] public float hold3Duration = 5f;
    [Range(0f, 10f)] public float fade3Duration = 5f;

    Coroutine introCoroutine = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        introCoroutine = StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        SetAlpha(title1, 0f);
        SetAlpha(title2, 0f);

        yield return new WaitForSeconds(hold1Duration);
        yield return Fade(title1, fade1Duration, 0f, 1f);
        yield return new WaitForSeconds(hold2Duration);

        yield return Fade(title2, fade2Duration, 0f, 1f);
        yield return new WaitForSeconds(hold3Duration);

        SetAlpha(title1, 0f);
        yield return Fade(title2, fade3Duration, 1f, 0f);

        // Just temp 
        if (Application.isEditor)
        {
            yield return new WaitForSeconds(1f);
            EditorApplication.isPlaying = false;
        }
    }

    private IEnumerator Fade(RawImage image, float duration, float startAlpha, float endAlpha)
    {
        var time = Time.time;

        var startColor = image.color;
        startColor.a = startAlpha;

        var endColor = image.color;
        endColor.a = endAlpha;

        while (Time.time - time < duration)
        {

            var t = (Time.time - time) / duration;
            //var alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            var alpha = Mathf.SmoothStep(startAlpha, endAlpha, t);
            
            SetAlpha(image, alpha);
            //image.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        image.color = endColor;
        yield return null;
    }

    private void SetAlpha(RawImage image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
