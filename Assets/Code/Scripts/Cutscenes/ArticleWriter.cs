using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class ArticleWriter : MonoBehaviour
{
    public TextMeshPro MainTitle;
    public TextMeshPro MainText1;
    public TextMeshPro MainText2;

    public TextMeshPro LeftTitle;
    public TextMeshPro LeftText;

    public TextMeshPro RightTitle;
    public TextMeshPro RightText;

    [Header("Articles")]
    public TextAsset PopeArticle;
    public TextAsset DickArticle;
    public TextAsset[] MindArticles;
    public TextAsset[] BodyArticles;
    public TextAsset[] SpiritArticles;
    public TextAsset[] SideArticles;

    public void ApplyPopeArticle() => ApplyMainArticle(PopeArticle);
    public void ApplyDickArticle() => ApplyMainArticle(DickArticle);

    public void ApplyRandomArticle(GlobalStateManager globalStateManager)
    {
        //TODO write some base articles so if its not high enough, there is still something
        var articles = new List<TextAsset>();
        if (globalStateManager.BodyPurity <= 2.5f)
            articles.AddRange(BodyArticles);
        if (globalStateManager.MindPurity <= 2.5f)
            articles.AddRange(MindArticles);
        if (globalStateManager.SoulPurity <= 2.5f)
            articles.AddRange(SpiritArticles);

        var article = articles.Randomize().FirstOrDefault();
        if (article != null)
            ApplyMainArticle(article);
        else
            Debug.LogWarning("No article found. Uh oh.");
    }

    private void ApplyMainArticle(TextAsset article)
    {
        GetArticleContent(article, out var title, out var text1, out var text2);

        MainTitle.text = title;
        MainText1.text = text1;
        MainText2.text = text2;

        ApplyRandomSideArticles();
    }

    private void ApplyRandomSideArticles()
    {
        var articles = SideArticles.Randomize().Take(2);
        ApplyLeftArticle(articles.First());
        ApplyRightArticle(articles.Last());
    }

    private void ApplyLeftArticle(TextAsset article)
    {
        GetArticleContent(article, out var title, out var text1, out _);

        LeftTitle.text = title;
        LeftText.text = text1;
    }

    private void ApplyRightArticle(TextAsset article)
    {
        GetArticleContent(article, out var title, out var text1, out _);

        RightTitle.text = title;
        RightText.text = text1;
    }

    private static void GetArticleContent(TextAsset article, out string title, out string text1, out string text2)
    {
        title = string.Empty;
        text1 = string.Empty;
        text2 = string.Empty;

        try
        {

            var articleLines = article.text.Split(Environment.NewLine);

            title = articleLines.FirstOrDefault();

            if (articleLines.Length < 1)
            {
                text1 = string.Empty;
                text2 = string.Empty;
                return;
            }

            var text1Applied = false;
            var textBuilder = new StringBuilder();
            foreach (var line in articleLines.Skip(1))
            {
                if (line.ToLower().Contains("<newcolumn>"))
                {
                    text1 = textBuilder.ToString();
                    textBuilder.Clear();
                    text1Applied = true;
                    continue;
                }

                textBuilder.AppendLine(line);
            }

            if (text1Applied)
                text2 = textBuilder.ToString();
            else
                text1 = textBuilder.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}