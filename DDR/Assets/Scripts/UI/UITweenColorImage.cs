using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UITweenColorImage : MonoBehaviour {
    public enum LoopType {
        None,
        Yoyo,
    }

    #region Serialized Fields
    [SerializeField] private Image _imageBG = null;

    [SerializeField] private Color _colorFrom = Color.white;
    [SerializeField] private Color _colorTo = Color.white;
    [SerializeField] private float _startDelay = 0;
    [SerializeField] private float _duration = 0;
    [SerializeField] private AnimationCurve _aniCurve = null;

    [SerializeField] private LoopType _loopType = LoopType.None;
    #endregion

    #region Internal Fields
    private CancellationTokenSource _cts = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        Play().DoNotAwait();
    }
    #endregion

    #region APIs
    public void SetColorFrom(Color colorFrom) {
        _colorFrom = colorFrom;
    }

    public void SetColorTo(Color colorTo) {
        _colorTo = colorTo;
    }

    public async Task Play() {
        Stop();

        _cts = new CancellationTokenSource();

        await PlayColorTween(_cts.Token);
    }

    public void Stop() {
        if (_cts != null) {
            _cts.Cancel();
        }

        _cts = null;
    }
    #endregion

    #region Internal Methods
    private async Task PlayColorTween(CancellationToken ct) {
        float startDelayTimer = 0;
        while (startDelayTimer < _startDelay) {
            if (ct.IsCancellationRequested) {
                return;
            }

            await Task.Delay(1);

            startDelayTimer += Time.deltaTime;
        }

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
            _imageBG.color = Color.Lerp(_colorFrom, _colorTo, value);

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
        _imageBG.color = Color.Lerp(_colorFrom, _colorTo, value);
    }
    #endregion
}
