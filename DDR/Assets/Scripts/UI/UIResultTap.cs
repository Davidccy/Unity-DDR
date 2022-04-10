using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResultTap : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Image _imageBarValue = null;
    [SerializeField] private TextMeshProUGUI _textValue = null;
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
    public void SetScore(int score, int scoreMax) {
        _score = score;
        _scoreMax = scoreMax;

        Refresh();
    }

    public async Task Play(int score, int scoreMax) {
        _score = score;
        _scoreMax = scoreMax;

        _cts = new CancellationTokenSource();

        await PlayTween(_cts.Token);
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
    private async Task PlayTween(CancellationToken ct) {
        int score = 0;

        while (_score > 0 && score < _score) {
            _imageBarValue.fillAmount = (float) score / _scoreMax;
            _textValue.text = string.Format(string.Format("{0}", score));

            await Task.Delay(1);
            if (ct.IsCancellationRequested) {
                return;
            }

            score += 3;            
        }

        Refresh();
    }

    private void Refresh() {
        _imageBarValue.fillAmount = (float) _score / _scoreMax;
        _textValue.text = string.Format(string.Format("{0}", _score));
    }
    #endregion
}
