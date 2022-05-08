using System.Threading.Tasks;
using UnityEngine;

public class AudioManager : ISingleton<AudioManager> {
    #region Internal Fields
    private AudioSource _asBGM;
    private AudioSource _asSE;

    private float _volumeBGM;
    private float _volumeSE;
    #endregion

    #region Override Methods
    protected override void DoAwake() {
        InitSettings();
    }
    #endregion

    #region Properties
    public AudioSource AsBGM {
        get {
            if (_asBGM == null) {
                GameObject newGo = new GameObject();
                newGo.name = "AudioSourceBGM";
                newGo.transform.SetParent(this.transform);

                _asBGM = newGo.AddComponent<AudioSource>();
            }

            return _asBGM;
        }
    }

    public AudioSource AsSE {
        get {
            if (_asSE == null) {
                GameObject newGo = new GameObject();
                newGo.name = "AudioSourceSE";
                newGo.transform.SetParent(this.transform);

                _asSE = newGo.AddComponent<AudioSource>();
            }

            return _asSE;
        }
    }

    public float VolumeBGM {
        get {
            return _volumeBGM;
        }
        set {
            GameDataManager.SaveFloat(Define.GAME_DATA_KEY_VOLUME_BGM, value);
            _volumeBGM = value;
            _asBGM.volume = value;
        }
    }

    public float VolumeSE {
        get {
            return _volumeSE;
        }
        set {
            GameDataManager.SaveFloat(Define.GAME_DATA_KEY_VOLUME_SE, value);
            _volumeSE = value;
            _asSE.volume = value;
        }
    }
    #endregion

    #region APIs
    public async Task PlayBGM(AudioClip acBGM, bool loop = true, bool fadeInFadeOut = true) {
        if (fadeInFadeOut) {
            await FadeOutBGMValue();
        }

        AsBGM.clip = acBGM;
        AsBGM.loop = loop;
        AsBGM.Play();
        if (fadeInFadeOut) {
            await FadeInBGMValue();
        }
    }

    public async Task StopBGM(bool fadeInFadeOut = true) {
        if (fadeInFadeOut) {
            await FadeOutBGMValue();
        }
    }

    public void PlaySE(AudioClip acSE) {
        AsSE.clip = acSE;
        AsSE.Play();
    }
    #endregion

    #region Internal Methods
    private void InitSettings() {
        _volumeBGM = GameDataManager.LoadFloat(Define.GAME_DATA_KEY_VOLUME_BGM, Define.AUDIO_DEFAULT_BGM_VOLUME);
        _volumeSE = GameDataManager.LoadFloat(Define.GAME_DATA_KEY_VOLUME_BGM, Define.AUDIO_DEFAULT_SE_VOLUME);
    }

    private async Task FadeInBGMValue() {
        float targetVolume = VolumeBGM;
        float passedTime = 0;
        float fadeInDuration = Define.AUDIO_BGM_FADE_IN_DURATION;

        while (passedTime < fadeInDuration) {
            AsBGM.volume = Mathf.Lerp(0, targetVolume, (passedTime / fadeInDuration));

            await Task.Delay(1);

            passedTime += Time.deltaTime;
        }

        AsBGM.volume = targetVolume;
    }

    private async Task FadeOutBGMValue() {
        float startVolume = AsBGM.volume;
        float passedTime = 0;
        float fadeOutDuration = Define.AUDIO_BGM_FADE_OUT_DURATION;

        while (passedTime < fadeOutDuration) {
            AsBGM.volume = Mathf.Lerp(startVolume, 0, (passedTime / fadeOutDuration));

            await Task.Delay(1);

            passedTime += Time.deltaTime;
        }

        AsBGM.volume = 0;
    }
    #endregion
}
