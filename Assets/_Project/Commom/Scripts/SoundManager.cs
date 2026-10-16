// Last assigned by 丹羽
using System;
using System.Collections.Generic;
using System.Linq;
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
    Dictionary<string, AudioClip> bgmDic;

    //SEの名前、AudioClipを管理するstruct(構造体)
    [Serializable]
    struct seData
    {
        public string seName;
        public AudioClip seAudio;
    }
    [SerializeField] List<seData> seList;
    Dictionary<string, AudioClip> seDic;

    protected override void Awake()
    {
        base.Awake();

        //BGMの名前とBGMのクリップを繋ぐ処理
        bgmDic = new Dictionary<string, AudioClip>();
        foreach (var bgm in bgmList)
        {
            bgmDic[bgm.bgmName] = bgm.bgmAudio;
        }

        //SEの名前とSEのクリップを繋ぐ処理
        seDic = new Dictionary<string, AudioClip>();
        foreach (var se in seList)
        {
            seDic[se.seName] = se.seAudio;
        }
    }

    /// <summary>
    /// BGMを再生する関数。
    /// BGMのClipが存在しない場合、再生されずエラーは出ません。
    /// </summary>
    /// <param name="bgmName">BGM名の引数</param>
    public void PlayBGM(string bgmName)
    {
        //引数bgmNameと一致するキーを探す
        if(bgmDic.TryGetValue(bgmName, out AudioClip bgmAudio))
        {
            bgmSources.clip = bgmAudio;

            //一致したBGMを再生
            bgmSources.Play();
        }
    }

    /// <summary>
    /// SEを再生する関数。
    /// SEのClipが存在しない場合、再生されずエラーは出ません。
    /// </summary>
    /// <param name="seName">SE名の引数</param>
    public void PlaySE(string seName)
    {
        //引数seNameと一致するキーを探す
        if (seDic.TryGetValue(seName, out AudioClip seAudio))
        {
            //一致したSEを再生
            seSources.PlayOneShot(seAudio);
        }
    }
}
