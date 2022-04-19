using System;
using System.Collections;
using UnityEngine;

public class TrackManager : ISingleton<TrackManager> {
    #region Internal Fields
    private AudioSource _asTrack;
    private AudioSource _asSE;

    private AudioClip _acTrack;
    private AudioClip _acSEReady;
    private AudioClip _acSEPerfect;
    private AudioClip _acSENormal;

    private TrackData _trackData = null;

    private bool _isPlaying = false;
    private bool _isTrackEnd = false;
    private float _trackTime = 0;
    private float _delayBeforeReady = 2.0f;
    private float _pre = 0;
    #endregion

    #region Properties
    public TrackData TrackData {
        get {
            return _trackData;
        }
    }

    public AudioSource AsTrack {
        get {
            if (_asTrack == null) {
                GameObject newGo = new GameObject();
                newGo.name = "AudioSourceTrack";
                newGo.transform.SetParent(this.transform);

                _asTrack = newGo.AddComponent<AudioSource>();
            }

            return _asTrack;
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

    public bool IsPlaying {
        get {
            return _isPlaying;
        }
    }

    public bool IsTrackEnd {
        get {
            return _isTrackEnd;
        }
    }

    public float TrackLength {
        get {
            if (_acTrack == null) {
                return 0;
            }

            return _acTrack.length;
        }
    }

    public int BPM {
        get {
            return _trackData.BPM;
        }
    }

    public int CurMeasure {
        get {
            if (_trackData == null) {
                return -99;
            }

            float a = TrackProgress - _trackData.FirstMeasure;
            if (a < 0) {
                return 0;
            }

            float spm = (float) 60 / _trackData.BPM * 4; // Second per measure

            return Mathf.FloorToInt(a / spm) + 1;
        }
    }

    public float TrackProgress {
        get {
            if (IsTrackEnd) {
                return TrackLength;
            }

            return _pre + (_asTrack != null ? _asTrack.time : 0);
        }
    }
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        Stop();
    }

    private void OnDisable() {
        Stop();
    }

    private void FixedUpdate() {
        if (_isPlaying) {
            _trackTime += Time.fixedDeltaTime;
        }
    }
    #endregion

    #region APIs
    public void LoadTrackData() {
        int trackID = PlayerPrefs.HasKey(Utility.PLAYER_PREF_TRACK_ID) ? PlayerPrefs.GetInt(Utility.PLAYER_PREF_TRACK_ID) : 0;

        // Load data
        _trackData = Utility.GetTrackData(trackID);

        // Load Track
        _acTrack = Utility.GetTrack(trackID);

        // Load sound effect
        _acSEReady = Utility.GetSEReady();
        _acSEPerfect = Utility.GetSEPerfect();
        _acSENormal = Utility.GetSENormal();


        TrackLoadedEventArgs args = new TrackLoadedEventArgs();
        args.Dispatch();
    }

    public void PlayTrack(bool playReadyBump = false) {
        Stop();

        StartCoroutine(StartTrack());
    }

    //public void StopTrack() {
    //    Stop();
    //}

    public void PlaySE(bool isPerfect) {
        AsSE.clip = isPerfect ? _acSEPerfect : _acSENormal;
        AsSE.Play();
    }

    public void PlayReady() {
        AsSE.clip = _acSEReady;
        AsSE.Play();
    }

    public void Stop() {
        StopAllCoroutines();

        AsTrack.Stop();

        _isPlaying = false;
        _isTrackEnd = false;
    }
    #endregion

    #region Internal Methods
    private IEnumerator StartTrack() {
        if (_isPlaying) {
            yield break;
        }

        _isPlaying = true;
        _isTrackEnd = false;

        AsTrack.clip = _acTrack;
        AsTrack.time = 0;

        // BPM calculation
        float bps = (float) _trackData.BPM / 60;    // Bump per second
        float spb = 1 / bps;                        // Seconds per bump
        // BPM calculation

        _pre = -(spb * _trackData.ReadyCount - _trackData.StartDelay + _delayBeforeReady);
        float nextBumpTime = -(spb * _trackData.ReadyCount - _trackData.StartDelay); // First bump
        while (_pre < 0) {
            yield return new WaitForEndOfFrame();

            _pre += Time.deltaTime;
            if (_pre > nextBumpTime) {
                PlayReady();
                new BumpEventArgs().Dispatch();
                nextBumpTime += spb;
            }
        }
        _pre = 0;

        AsTrack.Play();

        while (AsTrack.isPlaying) {
            yield return new WaitForEndOfFrame();

            if (AsTrack.time >= nextBumpTime) {
                new BumpEventArgs().Dispatch();
                nextBumpTime += spb;
            }
        }

        _isTrackEnd = true;
    }
    #endregion
}
