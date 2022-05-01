using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResultValueRolling : MonoBehaviour {
    public enum ValueType { 
        Value,
        Ratio,
    }

    #region Serialized Fields
    [SerializeField] private Image _imageBarValue = null;
    [SerializeField] private TextMeshProUGUI _textValue = null;
    [SerializeField] private ValueType _pType = ValueType.Value;
    [SerializeField] private float _interval = 1;
    #endregion

    #region Internal Fields
    private CancellationTokenSource _cts = null;

    private int _score;
    private int _scoreMax;
    #endregion

    #region Mono Behaviour Hooks
    private void OnDestroy() {
        Stop();
    }
    #endregion

    #region APIs
    public void SetScore(int score, int scoreMax, bool refresh = false) {
        _score = score;
        _scoreMax = scoreMax;

        if (refresh) {
            Refresh();
        }        
    }

    public async Task Play(float delay) {
        _cts = new CancellationTokenSource();

        await PlayTween(_cts.Token, delay);
    }

    public void SetFinish() {
        Stop();

        Refresh();
    }

    public void Stop() {
        if (_cts != null) {
            _cts.Cancel();
        }

        _cts = null;
    }
    #endregion

    #region Internal Methods
    private async Task PlayTween(CancellationToken ct, float delay) {
        float delayTimer = 0;
        while (delayTimer < delay) {
            await Task.Delay(1);
            if (ct.IsCancellationRequested) {
                return;
            }

            delayTimer += Time.deltaTime;
        }

        int score = 0;

        float internalValue = 0;

        while (_score > 0 && score < _score) {
            if (_imageBarValue != null) {
                _imageBarValue.fillAmount = (float) score / _scoreMax;
            }
            _textValue.text = string.Format(string.Format("{0}", score));

            await Task.Delay(1);
            if (ct.IsCancellationRequested) {
                return;
            }

            if (_pType == ValueType.Value) {
                internalValue += _interval;
                score = (int) internalValue;
            }
            else {
                internalValue += _interval;
                score = (int) (_score * internalValue);
            }
        }

        Refresh();
    }

    private void Refresh() {
        if (_imageBarValue != null) {
            _imageBarValue.fillAmount = (float) _score / _scoreMax;
        }
        _textValue.text = string.Format(string.Format("{0}", _score));
    }
    #endregion
}
