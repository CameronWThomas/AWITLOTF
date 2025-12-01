using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InBetweenScene : ScenePlayer
{
    public TMP_Text TenYearsLaterText;

    private GlobalStateManager _globalStateManager;

    protected override void Start()
    {
        base.Start();

        _globalStateManager = GetGlobalStateManager();
        SetTenYearsLaterAlpha(0f);
        TenYearsLaterText.gameObject.SetActive(true);

        StartCoroutine(InBetweenRoutine());
    }

    private IEnumerator InBetweenRoutine()
    {
        yield return FadeTenYearsLater(3f, 5f);

        yield return new WaitForSeconds(3f);

        var articleWriter = Newspaper.GetComponent<ArticleWriter>();
        articleWriter.ApplyRandomArticle(_globalStateManager);
        yield return ShowNewspaper(Newspaper);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(1);
    }

    private IEnumerator FadeTenYearsLater(float fadeDuration, float holdDuration)
    {
        yield return FadeTenYearsLater(fadeDuration, 0f, 1f);
        yield return new WaitForSeconds(holdDuration);
        yield return FadeTenYearsLater(fadeDuration, 1f, 0f);
    }

    private IEnumerator FadeTenYearsLater(float duration, float startAlpha, float endAlpha)
    {
        var startTime = Time.time;
        while (Time.time - startTime < duration)
        {

            var t = (Time.time - startTime) / duration;
            var alpha = Mathf.SmoothStep(startAlpha, endAlpha, t);

            SetTenYearsLaterAlpha(alpha);

            yield return null;
        }

        SetTenYearsLaterAlpha(endAlpha);
    }

    private void SetTenYearsLaterAlpha(float alpha)
    {
        TenYearsLaterText.alpha = alpha;
    }

    private GlobalStateManager GetGlobalStateManager()
    {
        var globalStateManager = FindFirstObjectByType<GlobalStateManager>();
        if (globalStateManager != null)
            return globalStateManager;

        var newGameObject = new GameObject("GLOBAL STATE MANAGER INSTANCE");
        globalStateManager = newGameObject.AddComponent<GlobalStateManager>();

        return globalStateManager;
    }
}
