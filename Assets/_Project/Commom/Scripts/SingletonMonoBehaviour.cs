// Last assigned by 武藤
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour
    where T : SingletonMonoBehaviour<T>

{
    static T instance;

    [SerializeField] bool dontDestroyOnLoad;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        //インスタンスが既に存在する場合、
        //既存のインスタンスを優先する為自身を破棄する
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        //まだインスタンスが存在しないので
        //自身を唯一のインスタンスとして設定する
        instance = (T)this;

        //DontDestroyOnLoadが有効な場合は
        //自身をシーン遷移しても破棄されないようにする
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
