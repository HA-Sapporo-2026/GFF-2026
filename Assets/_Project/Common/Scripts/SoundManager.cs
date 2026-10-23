// Last assigned by 丹羽
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GFF2026.Common;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    //BGMを再生するAudioSource
    [SerializeField] AudioSource bgmSources;
    //SEを再生するAudioSource
    [SerializeField] AudioSource seSources;

    //BGMの名前、AudioClipを管理するstruct(構造体)
    [Serializable]
    struct bgmData
    {
        public string bgmName;
        public AudioClip bgmAudio;
    }
    [SerializeField] List<bgmData> bgmList;
    Dictionary<string, AudioClip> bgmDictionary;

    //SEの名前、AudioClipを管理するstruct(構造体)
    [Serializable]
    struct seData
    {
        public string seName;
        public AudioClip seAudio;
    }
    [SerializeField] List<seData> seList;
    Dictionary<string, AudioClip> seDictionary;

    protected override void OnAwake()
    {
        //BGMの名前とBGMのクリップを繋ぐ処理
        bgmDictionary = new Dictionary<string, AudioClip>();
        foreach (var bgm in bgmList)
        {
            bgmDictionary[bgm.bgmName] = bgm.bgmAudio;
        }

        //SEの名前とSEのクリップを繋ぐ処理
        seDictionary = new Dictionary<string, AudioClip>();
        foreach (var se in seList)
        {
            seDictionary[se.seName] = se.seAudio;
        }
    }

    /// <summary>
    /// BGMを再生する関数。
    /// BGMのClipが存在しない場合、再生されず警告が表示されます。
    /// </summary>
    /// <param name="bgmName">BGM名の引数。必須</param>
    /// <param name="volume">BGM音量の引数。指定しない場合1.0f</param>
    /// /// <param name ="isLoop">BGMがループするか。指定しない場合true</param>
    public void PlayBgm(string bgmName, float volume = 1.0f, bool isLoop = true)
    {
        //引数bgmNameと一致するキーを探す
        if (bgmDictionary.TryGetValue(bgmName, out AudioClip bgmAudio))
        {
            //指定したBGMをオーディオソースに設定
            bgmSources.clip = bgmAudio;
            //指定した音量に設定
            bgmSources.volume = volume;
            //ループ設定
            bgmSources.loop = isLoop;

            //一致したBGMを再生
            bgmSources.Play();
        }
        else
        {
            //キーに対応するBGMクリップが存在しない場合の警告
            Debug.LogWarning($"PlayBgm:指定されたキー({bgmName})に対応するBGMが存在しません");
        }
    }

    /// <summary>
    /// BGMの音量をフェードインしながらBGMを再生するコルーチン
    /// </summary>
    /// <param name="bgmName">BGM名の引数。必須</param>
    /// <param name="fadeTime">BGM音量が最大になるまでの時間。必須</param>
    /// <param name="volume">BGM音量の引数。指定しない場合1.0f</param>
    /// /// <param name ="isLoop">BGMがループするか。指定しない場合true</param>
    public IEnumerator PlayBgmWithFade(string bgmName, float fadeTime, float volume = 1.0f, bool isLoop = true)
    {
        //引数bgmNameと一致するキーを探す
        if (bgmDictionary.TryGetValue(bgmName, out AudioClip bgmAudio))
        {
            //指定したBGMをオーディオソースに設定
            bgmSources.clip = bgmAudio;
            //ループ設定
            bgmSources.loop = isLoop;

            bgmSources.volume = 0.0f;
            float t = 0.0f;

            //一致したBGMを再生
            bgmSources.Play();

            //BGM音量のフェードを開始
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                bgmSources.volume = Mathf.Lerp(0.0f, volume, t / fadeTime);
                yield return null;
            }

            //指定された音量に設定
            bgmSources.volume = volume;
        }
        else
        {
            //キーに対応するBGMクリップが存在しない場合、警告
            Debug.LogWarning($"PlayBgmWithFade:指定されたキー({bgmName})に対応するBGMが存在しません");
        }
    }

    /// <summary>
    /// BGMの再生を停止する関数
    /// </summary>
    /// <param name="bgmName">BGM名の引数。必須</param>
    public void StopBgm(string bgmName)
    {
        //引数bgmNameと一致するキーを探す
        if (bgmDictionary.TryGetValue(bgmName, out AudioClip bgmAudio))
        {
            //一致したBGMを停止
            bgmSources.Stop();
        }
        else
        {
            //キーに対応するBGMクリップが存在しない場合、警告
            Debug.LogWarning($"StopBgm:指定されたキー({bgmName})に対応するBGMが存在しません");
        }
    }

    /// <summary>
    /// BGMの音量をフェードアウトしながらBGMの再生を停止するコルーチン
    /// </summary>
    /// <param name="bgmName">BGM名の引数。必須</param>
    /// <param name="fadeTime">BGM音量が最大になるまでの時間。必須</param>
    public IEnumerator StopBgmWithFade(string bgmName, float fadeTime)
    {
        //引数bgmNameと一致するキーを探す
        if (bgmDictionary.TryGetValue(bgmName, out AudioClip bgmAudio))
        {
            float startVolume = bgmSources.volume;
            float t = 0.0f;

            //フェードを開始
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                bgmSources.volume = Mathf.Lerp(startVolume, 0.0f, t / fadeTime);
                Debug.Log(bgmSources.volume);
                yield return null;
            }

            //一致したBGMを停止
            bgmSources.volume = 0.0f;
            bgmSources.Stop();
        }
        else
        {
            //キーに対応するBGMクリップが存在しない場合、警告
            Debug.LogWarning($"StopBgmWithFade:指定されたキー({bgmName})に対応するBGMが存在しません");
        }
    }

    /// <summary>
    /// SEを再生する関数。
    /// SEのClipが存在しない場合、再生されず警告が表示されます。
    /// </summary>
    /// <param name="seName">SE名の引数</param>
    /// /// <param name="volume">SE音量の引数。指定しない場合1.0f</param>
    public void PlaySe(string seName, float volume = 1.0f)
    {
        //引数seNameと一致するキーを探す
        if (seDictionary.TryGetValue(seName, out AudioClip seAudio))
        {
            //指定した音量に設定
            seSources.volume = volume;

            //一致したSEを再生
            seSources.PlayOneShot(seAudio);
        }
        else
        {
            //キーに対応するSEクリップが存在しない場合、警告
            Debug.LogWarning($"PlaySe:指定されたキー({seName})に対応するSEが存在しません");
        }
    }
}
