// Last assigned by 武藤
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GFF2026.Common;

public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    [SerializeField] Image fadeImage; // フェード用UI Image
    [SerializeField] GraphicRaycaster graphicRaycaster;

    protected override void OnAwake()
    {
        if (fadeImage == null)
        {
            Debug.LogError("フェードイメージが設定されていません。");
        }
        if (graphicRaycaster == null)
        {
            Debug.LogError("グラフィックレイキャスターが設定されていません。");
        }
    }


    /// <summary>
    /// 画面を指定時間で明るくする
    /// 引数のfadeDurationで時間を設定できる
    /// </summary>
    /// <param name="fadeDuration"></param>
    /// <returns></returns>
    public IEnumerator FadeIn(float fadeDuration = 1.0f)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(1f - Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }
        graphicRaycaster.enabled = false;
    }


    /// <summary>
    /// 画面を指定時間で暗くする
    /// 引数のfadeDurationで時間を設定できる
    /// </summary>
    /// <param name="fadeDuration"></param>
    /// <returns></returns>
    public IEnumerator FadeOut(float fadeDuration = 1.0f)
    {
        graphicRaycaster.enabled = true;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Clamp01(t / fadeDuration));//時間の経過に合わせて、画面の暗さ（透明度）を変える処理
            yield return null;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
