// Last assigned by 武藤
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GFF2026.Common;

public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    [SerializeField] Image fadeImage; // フェード用UI Image
    [SerializeField] float fadeDuration = 1f; // フェード時間（秒）
    [SerializeField] GraphicRaycaster graphicRaycaster;

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
        graphicRaycaster.enabled = false;
    }
    public IEnumerator FadeOut()
    {
        Debug.Log($"raycaster: {graphicRaycaster}");
        graphicRaycaster.enabled = true;
        //画面を暗くする
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(t / fadeDuration));//時間の経過に合わせて、画面の暗さ（透明度）を変える処理
            yield return null;
        }
        yield return null; // 1フレーム待つ
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
