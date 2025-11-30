using System.Collections;
using TMPro;
using UnityEngine;

public class CreditsText : MonoBehaviour
{

    public string[] creditsLines = new string[]
    {
        "Another Waver in Time, Love of the Future",
        "Created by: Connor and Cameron Thomas",
        "Press \"Q\" at any time to quit the game.",
    };
    public TextMeshPro text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        StartCoroutine(PlayCredits());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator PlayCredits()
    {
        foreach (var line in creditsLines)
        {
            text.text = line;
            yield return new WaitForSeconds(10f);
        }
    }
}
