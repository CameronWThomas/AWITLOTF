using System;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject TutorialContainer;

    public bool IsTutorialOpen { get; private set; } = false;

    bool _hasFoundGlobalStateManager = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CloseTutorial();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            ToggleTutorial();

        if (_hasFoundGlobalStateManager)
            return;

        var globalStateManager = FindFirstObjectByType<GlobalStateManager>();
        if (globalStateManager != null && globalStateManager.CurrentRunCount == 1)
        {
            _hasFoundGlobalStateManager = true;
            OpenTutorial();
        }
    }

    private void ToggleTutorial()
    {
        if (IsTutorialOpen)
            CloseTutorial();
        else
            OpenTutorial();
    }

    private void OpenTutorial()
    {
        IsTutorialOpen = true;
        TutorialContainer.SetActive(true);
    }

    private void CloseTutorial()
    {
        IsTutorialOpen = false;
        TutorialContainer.SetActive(false);
    }
}