using Game.Common.Const;
using Game.Common.Utils;
using Game.Config;
using Game.Core.Net;
using UnityEngine;

namespace Game.Core.Manager
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        //private static GameObject _gameObject;

        public AudioSource bgmSource;
        public AudioSource clickSoundSource;

        private string playingAudioName;

        protected override void Awake()
        {
            base.Awake(); // 调用父类 Awake 初始化单例

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                clickSoundSource = gameObject.AddComponent<AudioSource>();
                clickSoundSource.playOnAwake = false;
                bgmSource.playOnAwake = false; // 避免一创建就播放
            }
        }

        public void Init()
        {
            bgmSource.loop = true;
        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            bgmSource.clip = audioClip;
            bgmSource.Play();
        }

        public void PlayBGM(string audioName)
        {
            if (playingAudioName == audioName) return;
            var path = ConfigManager.pathConfig.GetPath(GameConst.PathCategory.Sound, audioName);
            ResourceLoader.Instance.LoadRemoteAudio(path, audioClip =>
            {
                if (audioClip == null)
                {
                    Debug.LogError("AudioClip is null!");
                    return;
                }

                PlayAudioClip(audioClip);
            });
        }

        public void PlayClickSound(string audioName)
        {
            var path = GameConst.SoundPath.UI_SOUND_PATH+audioName;
            ResourceLoader.Instance.LoadRemoteAudio(path, audioClip =>
            {
                if (audioClip == null)
                {
                    Debug.LogError("AudioClip is null!");
                    return;
                }

                clickSoundSource.clip = audioClip;
                clickSoundSource.Play();
            });

        }

        public async void PlaySFX(string audioName, float volume = 1f)
        {
            AudioClip audioClip = await ResourceUtils.GetSoundEffect(audioName);
            if (audioClip == null)
            {
                Debug.LogError($"SFX '{audioName}' is null!");
                return;
            }

            var tempAudioSource = gameObject.AddComponent<AudioSource>();
            tempAudioSource.clip = audioClip;
            tempAudioSource.loop = false;
            tempAudioSource.volume = volume;
            tempAudioSource.Play();

            Destroy(tempAudioSource, audioClip.length);
        }
    }
}