using UnityEngine;
namespace Sandbox.Kamada
{
    public class TestTitleManager : MonoBehaviour
    {
        /// <summary>
        /// スタートボタンが押された時の処理
        /// </summary>
        public void OnStart()
        {
            SoundManager.Instance.PlaySe("Start");
            StartCoroutine(FadeManager.Instance.FadeOut());
        }
    }
}
