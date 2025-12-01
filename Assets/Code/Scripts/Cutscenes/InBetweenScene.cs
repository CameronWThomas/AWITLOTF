using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InBetweenScene : ScenePlayer
{
    private GlobalStateManager _globalStateManager;

    protected override void Start()
    {
        base.Start();

        _globalStateManager = GetGlobalStateManager();

        StartCoroutine(InBetweenRoutine());
    }

    private IEnumerator InBetweenRoutine()
    {
        yield return new WaitForSeconds(5f);

        var articleWriter = Newspaper.GetComponent<ArticleWriter>();
        articleWriter.ApplyRandomArticle(_globalStateManager);
        yield return ShowNewspaper(Newspaper);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(1);
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
