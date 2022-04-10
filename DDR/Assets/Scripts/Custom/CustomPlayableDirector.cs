using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class CustomPlayableDirector : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private PlayableDirector _pd = null;
    #endregion

    #region Exposed Fields
    public Action<PlayableDirector> onStopped = null;
    public Action<PlayableDirector> onPlayed = null;
    public Action<PlayableDirector> onPaused = null;
    //public Action<PlayableDirector> onFinished = null;
    #endregion

    #region Internal Fields
    private CancellationTokenSource _cts = null;
    private bool _isPlaying = false;
    #endregion

    #region Properties
    public bool IsPlaying {
        get {
            return _isPlaying;
        }
    }
    #endregion

    #region Mono Behaviou Hooks
    private void Awake() {
        if (_pd != null) {
            _pd.stopped += InternalStopped;
            _pd.played += InternalPlayed;
            _pd.paused += InternalPaused;
        }
    }

    private void OnDestroy() {
        if (_pd != null) {
            _pd.stopped -= InternalStopped;
            _pd.played -= InternalPlayed;
            _pd.paused -= InternalPaused;
        }

        Stop();
    }
    #endregion

    #region APIs
    public async Task Play(Action cb = null) {
        Stop();

        _cts = new CancellationTokenSource();

        _pd.Play();
        _isPlaying = true;

        await Timing(cb, _cts.Token);
    }

    public void SetFinish() {
        //Stop();

        _pd.time = _pd.duration;
    }

    public void Stop() {
        if (_cts != null) {
            _cts.Cancel();
        }

        _cts = null;

        _pd.Stop();
        _isPlaying = false;
    }
    #endregion

    #region Internal Methods
    private async Task Timing(Action cb, CancellationToken ct) {
        float startTime = Time.realtimeSinceStartup;
        float passedTime = 0;
        double duration = _pd.duration;

        while (passedTime < duration) {
            await Task.Delay(1);
            if (ct.IsCancellationRequested) {
                return;
            }

            passedTime = Time.realtimeSinceStartup - startTime;
        }

        // Finished
        if (cb != null) {
            cb();
        }
    }

    private void InternalStopped(PlayableDirector pd) {
        _isPlaying = false;

        if (onStopped != null) {
            onStopped(pd);
        }
    }

    private void InternalPlayed(PlayableDirector pd) {
        _isPlaying = true;

        if (onPlayed != null) {
            onPlayed(pd);
        }
    }

    private void InternalPaused(PlayableDirector pd) {
        _isPlaying = false;

        if (onPaused != null) {
            onPaused(pd);
        }
    }
    #endregion


}
