using UnityEngine;

namespace Sandbox.Masuda
{
    public class TitleManager : MonoBehaviour
    {
        // 鳴らすSEの名前をここで入力する
        // (もしかしたら必要ないかも)
        [SerializeField] string titleSeName;

        // スタートボタンが押されたら、
        public void OnClickStartButton()
        {
            // SoundManagerのPlaySeを呼び出して、
            // 決めたSEを再生する
            SoundManager.Instance.PlaySe(titleSeName);

            // FadeManagerのFadeOutを呼び出して、
            // 画面を暗転させる
            StartCoroutine(FadeManager.Instance.FadeOut());
        }
    }
}

