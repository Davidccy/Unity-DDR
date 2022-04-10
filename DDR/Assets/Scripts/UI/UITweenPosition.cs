using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UITweenPosition : MonoBehaviour {
    public enum LoopType {
        None,
        Yoyo,
    }

    #region Serialized Fields
    [SerializeField] private RectTransform _rectTarget = null;
    [SerializeField] private Vector2 _posStart = Vector2.zero;
    [SerializeField] private Vector2 _posEnd = Vector2.zero;
    [SerializeField] private float _duration = 0;
    [SerializeField] private AnimationCurve _aniCurve = null;
    [SerializeField] private LoopType _loopType = LoopType.None;
    #endregion

    #region Internal Fields
    private CancellationTokenSource _cts = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        PlayTween().DoNotAwait();
    }

    private void OnDestroy() {
        StopTween();
    }
    #endregion

    #region APIs
    public void SetPosition(Vector2 posStart, Vector2 posEnd) {
        _posStart = posStart;
        _posEnd = posEnd;
    }

    public void SetDuration(float duration) {
        _duration = duration;
    }

    public async Task RestartTween() {
        await PlayTween();
    }

    public void StopTween() {
        if (_cts != null) {
            _cts.Cancel();
        }

        _cts = null;
    }
    #endregion

    #region Internal Methods
    private async Task PlayTween() {
        if (_cts != null) {
            _cts.Cancel();
        }

        _cts = new CancellationTokenSource();
        await DoTween(_cts.Token);
    }

    private async Task DoTween(CancellationToken ct) {
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
            _rectTarget.anchoredPosition = Vector2.Lerp(_posStart, _posEnd, value);

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
        _rectTarget.anchoredPosition = Vector2.Lerp(_posStart, _posEnd, value);
    }
    #endregion
}
