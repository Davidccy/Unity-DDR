using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UITweenCanvasGroupAlpha : MonoBehaviour {
    public enum LoopType {
        None,
        Yoyo,
    }

    #region Serialized Fields
    [SerializeField] private CanvasGroup _cg = null;
    [SerializeField] private float _alphaMax = 0;
    [SerializeField] private float _alphaMin = 0;
    [SerializeField] private float _duration = 0;
    [SerializeField] private AnimationCurve _aniCurve = null;
    [SerializeField] private LoopType _loopType = LoopType.None;
    #endregion

    #region Internal Fields
    private CancellationTokenSource _cts = null;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        Play().DoNotAwait();
    }

    private void OnDestroy() {
        Stop();
    }
    #endregion

    #region APIs
    public void SetAlphaRange(float max, float min) {
        _alphaMax = max;
        _alphaMin = min;
    }

    public void SetDuration(float duration) {
        _duration = duration;
    }

    public async Task Play() {
        Stop();

        _cts = new CancellationTokenSource();

        await PlayTween(_cts.Token);
    }

    public void Stop() {
        if (_cts != null) {
            _cts.Cancel();
        }

        _cts = null;
    }
    #endregion

    #region Internal Methods
    private async Task PlayTween(CancellationToken ct) {
        float progress = 0;
        float value = 0;
        float passedTime = 0.0f;

        bool toward = true;
        bool tweenFinish = false;
        
        while (!tweenFinish) {
            if (ct.IsCancellationRequested) {
                return;
            }

            progress = toward ? (passedTime / _duration) : 1 - (passedTime / _duration);
            value = _aniCurve.Evaluate(progress);
            _cg.alpha = value;

            await Task.Delay(1);

            passedTime += Time.deltaTime;
            if (passedTime > _duration) {
                if (_loopType == LoopType.None) {
                    tweenFinish = true;
                }
                else if (_loopType == LoopType.Yoyo) {
                    passedTime -= _duration;
                    toward = !toward;
                }
            }
        }

        progress = 1;
        value = _aniCurve.Evaluate(progress);
        _cg.alpha = value;
    }
    #endregion
}
