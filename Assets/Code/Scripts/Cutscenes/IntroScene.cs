using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : ScenePlayer
{
    [Header("Opening Cards")]
    public GameObject TitleParent;
    public GameObject SeriousQuoteParent;
    public GameObject GoofQuoteParent;
    [Range(0f, 10f)] public float quoteHoldDuration = 3.5f;
    [Range(0f, 10f)] public float quoteFadeDuration = 4f;
    [Range(0f, 10f)] public float titleHoldDuration = 3.5f;
    [Range(0f, 10f)] public float titleFadeDuration = 3.5f;

    [Header("Debug")]
    public bool SkipOpeningCards = false;

    Coroutine introCoroutine = null;
    private float spaceAlpha;

    protected override void Start()
    {
        base.Start();
        Initialize();

        introCoroutine = StartCoroutine(IntroRoutine());
    }

    private void Initialize()
    {
        TitleParent.SetActive(true);
        SeriousQuoteParent.SetActive(true);
        GoofQuoteParent.SetActive(true);

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
        
        articleWriter.ApplyPopeArticle();
        yield return ShowNewspaper(Newspaper);

        articleWriter.ApplyDickArticle();
        yield return ShowNewspaper(Newspaper);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(1);
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
