using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace KazukiTrumpGame
{
    /// <summary>
    /// サウンド関連を管理するシングルトンパターンのクラス
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        //オーディオミキサーグループ（Master,BGM,SE）
        public AudioMixerGroup bgmMixerGroup, seMixerGroup;

        // 同時再生可能なオーディオソースの最大数
        public int maxAudioSources = 8;

        //BGMとSEのオーディオソース
        [SerializeField]
        AudioSource audioSources_BGM;

        private List<AudioSource> audioSources_SE;

        //全ボタンに反映させるSEのクリップ(クリック、エンター)
        public AudioClip buttonClickSE;

        //トランプめくるSE
        public AudioClip trumpOpenSE;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            //BGM用のオーディオミキサーグループを指定
            audioSources_BGM.outputAudioMixerGroup = bgmMixerGroup;

            // SEListの初期化
            audioSources_SE = new List<AudioSource>();
            // 最初に最大数分のSEのオーディオソースを生成
            for (int i = 0; i < maxAudioSources; i++)
            {
                audioSources_SE.Add(gameObject.AddComponent<AudioSource>());
            }

            foreach (AudioSource source in audioSources_SE)
            {
                source.outputAudioMixerGroup = seMixerGroup; // SE用のオーディオミキサーグループを指定
                source.playOnAwake = false;//PlayOnAwakeをオフ
            }
        }


        /// <summary>
        /// SEを鳴らすメソッド
        /// </summary>
        public void PlaySound_SE(AudioClip clip, float volume = 1f)
        {
            // 使用可能なオーディオソースを探す
            AudioSource availableSource = GetAvailableAudioSource();
            if (availableSource != null)
            {
                availableSource.clip = clip;
                availableSource.volume = volume;
                availableSource.Play();
            }
            else
            {
                // 新しいオーディオソースを作成して再生
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                newSource.clip = clip;
                newSource.volume = volume;
                newSource.Play();
                audioSources_SE.Add(newSource);
            }
        }

        /// <summary>
        /// 使用可能なオーディオソースを探してすメソッド
        /// </summary>
        /// <returns></returns>
        private AudioSource GetAvailableAudioSource()
        {
            foreach (AudioSource source in audioSources_SE)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            return null;
        }



        //MasterのBGMをセットするメソッド
        public void SetBGMVolume(float volume)
        {
            bgmMixerGroup.audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        }

        //MasterのSEをセットするメソッド
        public void SetSEVolume(float volume)
        {
            seMixerGroup.audioMixer.SetFloat("SE", Mathf.Log10(volume) * 20);
        }

        //BGMのミキサーグループの値を返すメソッド
        public float GetBGMVolume()
        {
            float value;
            if (bgmMixerGroup.audioMixer.GetFloat("BGM", out value))
            {
                return Mathf.Pow(10, value / 20);
            }
            return 0.0001f;
        }

        //SEのミキサーグループの値を返すメソッド
        public float GetSEVolume()
        {
            float value;
            if (seMixerGroup.audioMixer.GetFloat("SE", out value))
            {
                return Mathf.Pow(10, value / 20);
            }
            return 0.0001f;
        }
    }
}
