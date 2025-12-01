using System.Collections;
using TMPro;
using UnityEngine;

public class ScenePlayer : MonoBehaviour
{
    [Header("Newspaper")]
    public Newspaper Newspaper;
    public TMP_Text SpaceButtonIndicator;
    [Range(0f, 10f)] public float ShowIntroAndOutroDuration = 2f;
    [Range(0f, 10f)] public float SpaceIndicatorFadeTime = 1f;

    private float spaceAlpha;

    protected virtual void Start()
    {
        spaceAlpha = SpaceButtonIndicator.color.a;
        SpaceButtonIndicator.gameObject.SetActive(false);
        Newspaper.Hide();
    }

    protected IEnumerator ShowNewspaper(Newspaper newspaper)
    {
        newspaper.SetPositionAndRotation(0f);
        newspaper.Show();

        yield return NewspaperDisplay(newspaper, 0f, 1f, ShowIntroAndOutroDuration);

        yield return new WaitForSeconds(.25f);

        yield return ShowSpaceButton(SpaceButtonIndicator, SpaceIndicatorFadeTime, 0f, spaceAlpha);
        yield return newspaper.WaitForInputRoutine();
        yield return ShowSpaceButton(SpaceButtonIndicator, SpaceIndicatorFadeTime / 2f, spaceAlpha, 0f);

        yield return new WaitForSeconds(.25f);

        yield return NewspaperDisplay(newspaper, 1f, 0f, ShowIntroAndOutroDuration);

        newspaper.Hide();

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ShowSpaceButton(TMP_Text text, float duration, float startAlpha, float endAlpha)
    {
        var color = text.color;

        text.gameObject.SetActive(true);
        color.a = startAlpha;
        text.color = color;

        var startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            var t = (Time.time - startTime) / duration;

            var alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            color.a = alpha;
            text.color = color;

            yield return null;
        }

        color.a = endAlpha;
        text.color = color;
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
}