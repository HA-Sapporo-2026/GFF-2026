// Last assigned by 福野
using UnityEngine;

namespace GFF2026.Common
{
    /// <summary>
    /// MonoBehaviourを継承したシングルトンMonoBehaviourクラスです。
    /// マネージャークラスなど、どこからでもアクセスしたいコンポーネントクラスに継承させます。
    /// </summary>
    /// <remarks>
    /// 継承先のコンポーネントクラスでAwake、OnDestroy相当の処理を実装したい場合は
    /// OnAwake、OnDestroyedをオーバーライドしてください。
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        /// <summary>
        /// 静的なコンポーネントのインスタンス。
        /// </summary>
        static T s_instance;

        /// <summary>
        /// シーン遷移しても自身をシーン遷移しても破棄されないようにします。
        /// </summary>
        [SerializeField] bool dontDestroyOnLoad;

        public static T Instance => s_instance;

        void Awake()
        {
            // インスタンスが既に存在する場合、
            // 既存のインスタンスを優先する為自身を破棄します。
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            // まだインスタンスが存在しないので、
            // 自身を唯一のインスタンスとして設定します。
            s_instance = (T)this;

            // DontDestroyOnLoadが有効な場合は、
            // 自身をシーン遷移しても破棄されないようにします。
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
            
            OnAwake();
        }

        void OnDestroy()
        {
            if (s_instance != this) { return; }
            
            s_instance = null;
            OnDestroyed();
        }
        
        /// <summary>
        /// Awake時に呼ばれる処理。
        /// </summary>
        protected virtual void OnAwake() { }
        
        /// <summary>
        /// OnDestroy時に呼ばれる処理。
        /// </summary>
        protected virtual void OnDestroyed() { }
    }
}