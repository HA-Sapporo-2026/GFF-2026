using UnityEngine;

namespace Sandbox.Chiba
{
    public class StartFade : MonoBehaviour
    {
        public void StartFadeOut()
        {
            StartCoroutine(FadeManager.Instance.FadeOut());
        }
    }
}