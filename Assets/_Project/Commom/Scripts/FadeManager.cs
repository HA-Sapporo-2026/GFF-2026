// Last assigned by 武藤
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    [SerializeField] Image fadeImage; // フェード用UI Image
    [SerializeField] float fadeDuration = 1f; // フェード時間（秒）


    public void FadeToScene()
    {
        StartCoroutine(FadeOut());
    }
    //画面を透明化
    public IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(1f - Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }
    }
    public IEnumerator FadeOut()
    {
        //画面を暗くする
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }
        yield return null; // 1フレーム待つ
        StartCoroutine(FadeIn());
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}
