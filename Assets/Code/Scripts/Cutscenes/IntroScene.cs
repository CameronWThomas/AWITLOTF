using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour
{
    

    [Header("Newspaper")]
    public Newspaper Newspaper;

    [Header("Opening Cards")]
    public GameObject TitleParent;
    public GameObject SeriousQuoteParent;
    public GameObject GoofQuoteParent;
    [Range(0f, 10f)] public float quoteHoldDuration = 3.5f;
    [Range(0f, 10f)] public float quoteFadeDuration = 4f;
    [Range(0f, 10f)] public float titleHoldDuration = 3.5f;
    [Range(0f, 10f)] public float titleFadeDuration = 3.5f;

    [Header("Newspaper")]
    [Range(0f, 10f)] public float ShowIntroAndOutroDuration = 2f;

    [Header("Debug")]
    public bool SkipOpeningCards = false;

    Coroutine introCoroutine = null;

    void Start()
    {
        Initialize();

        introCoroutine = StartCoroutine(IntroRoutine());
    }

    private void Initialize()
    {
        TitleParent.SetActive(true);
        SeriousQuoteParent.SetActive(true);
        GoofQuoteParent.SetActive(true);

        Newspaper.Hide();
        HideAllIntroCards();
    }

    private IEnumerator IntroRoutine()
    {
        if (!SkipOpeningCards)
        {
            yield return FadeBothTitles(SeriousQuoteParent, quoteHoldDuration, quoteFadeDuration);
            yield return FadeBothTitles(GoofQuoteParent, quoteHoldDuration, quoteFadeDuration);
            yield return FadeBothTitles(TitleParent, titleHoldDuration, titleFadeDuration);
        }

        HideAllIntroCards();

        var articleWriter = Newspaper.GetComponent<ArticleWriter>();
        articleWriter.ApplyMainArticle(articleWriter.PopeArticle);
        articleWriter.ApplyRandomSideArticles();

        yield return ShowNewspaper(Newspaper);

        // Just temp 
        if (Application.isEditor)
        {
            yield return new WaitForSeconds(1f);
            EditorApplication.isPlaying = false;
        }
    }

    private IEnumerator ShowNewspaper(Newspaper newspaper)
    {
        newspaper.SetPositionAndRotation(0f);
        newspaper.Show();

        yield return NewspaperDisplay(newspaper, 0f, 1f, ShowIntroAndOutroDuration);

        yield return new WaitForSeconds(.25f);
        yield return newspaper.WaitForInputRoutine();
        yield return new WaitForSeconds(.25f);

        yield return NewspaperDisplay(newspaper, 1f, 0f, ShowIntroAndOutroDuration);

        newspaper.Hide();

        yield return new WaitForSeconds(5f);

    }

    private static IEnumerator NewspaperDisplay(Newspaper newspaper, float t_0, float t_end, float duration)
    {
        var startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            var t = (Time.time - startTime) / duration;

            var newsPaperT = Mathf.Lerp(t_0, t_end, t);

            newspaper.SetPositionAndRotation(newsPaperT);
            yield return null;
        }

        newspaper.SetPositionAndRotation(t_end);
        yield return null;
    }


    private static IEnumerator FadeBothTitles(GameObject parent, float holdDuration, float fadeDuration)
    {
        yield return Fade(Image1(parent), fadeDuration, 0f, 1f);

        yield return new WaitForSeconds(holdDuration);

        yield return Fade(Image2(parent), fadeDuration, 0f, 1f);
        SetAlpha(Image1(parent), 0f);

        yield return new WaitForSeconds(holdDuration);

        yield return Fade(Image2(parent), fadeDuration, 1f, 0f);

        yield return new WaitForSeconds(holdDuration);
    }

    private static IEnumerator Fade(RawImage image, float duration, float startAlpha, float endAlpha)
    {
        var startTime = Time.time;
        while (Time.time - startTime < duration)
        {

            var t = (Time.time - startTime) / duration;
            var alpha = Mathf.SmoothStep(startAlpha, endAlpha, t);
            
            SetAlpha(image, alpha);

            yield return null;
        }

        SetAlpha(image, endAlpha);

        yield return null;
    }

    private void HideAllIntroCards()
    {
        SetAlpha(Image1(TitleParent), 0f);
        SetAlpha(Image2(TitleParent), 0f);

        SetAlpha(Image1(SeriousQuoteParent), 0f);
        SetAlpha(Image2(SeriousQuoteParent), 0f);

        SetAlpha(Image1(GoofQuoteParent), 0f);
        SetAlpha(Image2(GoofQuoteParent), 0f);
    }

    private static RawImage Image1(GameObject parent) => Image(parent, 0);
    private static RawImage Image2(GameObject parent) => Image(parent, 1);

    private static RawImage Image(GameObject parent, int index)
    {
        return parent.transform.GetChild(index).GetComponent<RawImage>();
    }

    private static void SetAlpha(RawImage image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
