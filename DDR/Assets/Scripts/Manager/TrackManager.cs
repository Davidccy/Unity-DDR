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
    private bool _isEditorMode = false;
    private bool _isPausing = false;
    
    private float _pre = 0;
    private float _trackTime = 0;
    private float _bps = 0; // Bump per second
    private float _spb = 0; // Seconds per bump
    private float _nextBumpTime = 0;
    private int _nextBumpIndex = 0;

    private readonly float _DELAY_BEFORE_READY = 2.0f;
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

            float spm = 60.0f / _trackData.BPM * _trackData.BumpPerMeasure; // Second per measure

            return Mathf.FloorToInt(a / spm) + 1;
        }
    }

    public int BumpIndex {
        get {
            return _nextBumpIndex;
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

    public bool IsEditorMode {
        get {
            return _isEditorMode;
        }
    }

    public bool IsPausing {
        get {
            return _isPausing;
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
        ToNextFrame();
        RefreshTrackVolume();
    }
    #endregion

    #region APIs
    public void LoadTrackDataEditor(TrackData td) {
        Stop();

        _isEditorMode = true;

        _trackData = td;
        _acTrack = td.AudioTrack;
        _acSEReady = Utility.GetSEReady();
        _acSEPerfect = Utility.GetSEPerfect();
        _acSENormal = Utility.GetSENormal();

        TrackLoadedGameEventArgs args = new TrackLoadedGameEventArgs();
        args.Dispatch();
    }

    public void LoadTrackData() {
        if (!TempDataManager.HasData(Define.TEMP_GAME_DATA_KEY_SELECTED_TRACK_ID)) {
            Debug.LogErrorFormat("Invalid selected track ID, abort track data loading");
            return;
        }

        Stop();

        _isEditorMode = false;

        int trackID = TempDataManager.LoadData<int>(Define.TEMP_GAME_DATA_KEY_SELECTED_TRACK_ID);

        // Load data
        _trackData = Utility.GetTrackData(trackID);
        _acTrack = _trackData.AudioTrack;
        _acSEReady = Utility.GetSEReady();
        _acSEPerfect = Utility.GetSEPerfect();
        _acSENormal = Utility.GetSENormal();

        // Loading finished
        TrackLoadedGameEventArgs args = new TrackLoadedGameEventArgs();
        args.Dispatch();
    }

    public void PlayTrack() {
        Stop();

        //StartCoroutine(StartTrack());

        StartNew();
    }

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
        _isPausing = false;
    }

    public void Pause() {
        if (_isPausing) {
            return;
        }

        _isPausing = true;
        _asTrack.Pause();
    }

    public void Continue() {
        if (!_isPausing) {
            return;
        }

        _isPausing = false;
        _asTrack.Play();
    }
    #endregion

    #region Internal Methods
    public void StartNew() {
        _isPlaying = true;
        _isTrackEnd = false;

        AsTrack.clip = _acTrack;
        AsTrack.time = 0;

        _bps = (float) _trackData.BPM / 60;
        _spb = 1 / _bps;

        _pre = _trackData.FirstMeasure - (_spb * _trackData.DelayBumpCount + _DELAY_BEFORE_READY);
        _nextBumpTime = GetNextBumpTime(TrackProgress, _spb, out _nextBumpIndex);
        _trackTime = 0;
    }

    private void ToNextFrame() {
        if (!_isPlaying || IsPausing) {
            return;
        }

        if (_pre < 0) {
            _pre += Time.fixedDeltaTime;

            if (_pre > _nextBumpTime) {
                if (_nextBumpIndex < -(_trackData.DelayBumpCount - _trackData.ReadyCount)) {
                    PlayReady();
                }
                
                new BumpGameEventArgs().Dispatch();
                _nextBumpTime = GetNextBumpTime(TrackProgress, _spb, out _nextBumpIndex);
            }

            if (_pre >= 0) {
                AsTrack.Play();
            }
        }
        else {
            if (AsTrack.time >= _nextBumpTime) {
                if (_nextBumpIndex < -(_trackData.DelayBumpCount - _trackData.ReadyCount)) {
                    PlayReady();
                }

                new BumpGameEventArgs().Dispatch();
                _nextBumpTime = GetNextBumpTime(TrackProgress, _spb, out _nextBumpIndex);
            }
        }
    }

    private float GetNextBumpTime(float currentTime, float spb, out int bumpIndex) {
        bumpIndex = 0;
        float firstBumpTime = _trackData.FirstMeasure - (spb * _trackData.DelayBumpCount);
        if (currentTime < firstBumpTime) {
            bumpIndex = -_trackData.DelayBumpCount;
            return firstBumpTime;
        }

        float diffTime = 0;
        if (currentTime < _trackData.FirstMeasure) {
            diffTime = _trackData.FirstMeasure - currentTime;
            int remainedBumpCount = (int) (diffTime / spb);
            bumpIndex = -remainedBumpCount;

            return _trackData.FirstMeasure - remainedBumpCount * spb;
        }

        diffTime = currentTime - _trackData.FirstMeasure;
        int passedBumpCount = (int) (diffTime / spb);
        bumpIndex = passedBumpCount + 1;

        return _trackData.FirstMeasure + (passedBumpCount + 1) * spb;
    }

    //private IEnumerator StartTrack() {
    //    if (_isPlaying) {
    //        yield break;
    //    }

    //    _isPlaying = true;
    //    _isTrackEnd = false;

    //    AsTrack.clip = _acTrack;
    //    AsTrack.time = 0;

    //    // BPM calculation
    //    float bps = (float) _trackData.BPM / 60;    // Bump per second
    //    float spb = 1 / bps;                        // Seconds per bump
    //    // BPM calculation

    //    _pre = -(spb * _trackData.ReadyCount - _trackData.StartDelay + _DELAY_BEFORE_READY);
    //    float nextBumpTime = -(spb * _trackData.ReadyCount - _trackData.StartDelay); // First bump
    //    while (_pre < 0) {
    //        yield return new WaitForEndOfFrame();

    //        _pre += Time.deltaTime;
    //        if (_pre > nextBumpTime) {
    //            PlayReady();
    //            new BumpGameEventArgs().Dispatch();
    //            nextBumpTime += spb;
    //        }
    //    }
    //    _pre = 0;

    //    AsTrack.Play();

    //    while (AsTrack.isPlaying) {
    //        yield return new WaitForEndOfFrame();

    //        if (AsTrack.time >= nextBumpTime) {
    //            new BumpGameEventArgs().Dispatch();
    //            nextBumpTime += spb;
    //        }
    //    }

    //    _isTrackEnd = true;
    //}

    private void RefreshTrackVolume() {
        if (!_isPlaying) {
            return;
        }

        if (_asTrack == null || _asTrack.clip == null) {
            return;
        }

        float fadeOutDuration = 0.2f;
        float curTrackTime = _asTrack.time;
        float fadeOutTiming = TrackLength - fadeOutDuration;
        _asTrack.volume = curTrackTime <= fadeOutTiming ? 1 : Mathf.Lerp(0, 1, (TrackLength - curTrackTime) / fadeOutDuration);
    }
    #endregion
}
