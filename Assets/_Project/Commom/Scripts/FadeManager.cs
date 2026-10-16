// Last assigned by 武藤
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour/*SingletonMonoBehaviour<FadeManager>*/
{
    public static FadeManager Instance{ get; private set; }//FadeManager のインスタンスを共有する

    [SerializeField] Image fadeImage; // フェード用UI Image
    [SerializeField] GameObject canvas;
    [SerializeField] float fadeDuration = 1f; // フェード時間（秒）

    void Awake()
    {
        // シングルトン化
        if (Instance != null && Instance != this)//別の FadeManager が存在しているかどうかを判定している
        {
            Destroy(gameObject);
            Destroy(canvas);
            return;
        }
        Instance = this;//現在の FadeManager を、共有して使うインスタンスとして登録
        DontDestroyOnLoad(gameObject); // シーン切り替えでも破棄しない
        DontDestroyOnLoad(canvas);

        // 起動時はフェードイン
        if (fadeImage != null)//フェード用画像が設定されているか確認
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
            StartCoroutine(FadeIn());
        }
    }

    // 指定シーンへフェードアウトして遷移
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    //画面を透明化
    private IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(1f - Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }
    }
    private IEnumerator FadeOut(string sceneName)
    {
        //画面を暗くする
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }

        // シーン切り替え後フェードイン
        SceneManager.LoadScene(sceneName);
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

    /// <summary>
    /// 現在のシーン名に応じて次のシーンを決定して遷移
    /// </summary>
    public void NextSceneLoad()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string nextScene = DecideNextScene(currentScene);

        if (!string.IsNullOrEmpty(nextScene))
        {
            StartCoroutine(FadeOut(nextScene));
        }
        else
        {
            Debug.LogWarning("次のシーンが設定されていません: " + currentScene);
        }
    }

    /// <summary>
    /// 現在のシーン名から遷移先を決定するロジック
    /// </summary>
    private string DecideNextScene(string currentScene)
    {
        Debug.Log("フェード");
        // ここに条件分岐を追加
        switch (currentScene)
        {
            
            case "TestScene":
                return "TestSceneA";
            case "TestSceneA":
                return "TestScene";

            default:
                return null; // 該当なし
        }
    }
}
