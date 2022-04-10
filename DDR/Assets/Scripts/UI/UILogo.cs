using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UILogo : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btn = null;
    [SerializeField] private CustomPlayableDirector _cpdFadeIn = null;
    [SerializeField] private CustomPlayableDirector _cpdFadeOut = null;
    [SerializeField] private float _waitSeconds = 0;
    #endregion

    #region Internal Fields
    private CancellationTokenSource _cts = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btn.onClick.AddListener(ButtonOnClick);
    }

    private void OnDestroy() {
        _btn.onClick.RemoveListener(ButtonOnClick);

        Stop();
    }
    #endregion

    #region Button Handlings
    private void ButtonOnClick() {
        // Do nothing
    }
    #endregion

    #region APIs
    public async Task Play() {
        Stop();

        _cts = new CancellationTokenSource();

        await PlayPerformance(_cts.Token);
    }

    public void Stop() {
        _cpdFadeIn.Stop();
        _cpdFadeOut.Stop();

        if (_cts != null) {
            _cts.Cancel();
        }
        _cts = null;
    }
    #endregion

    #region Internal Methods
    private async Task PlayPerformance(CancellationToken ct) {
        await _cpdFadeIn.Play();
        if (ct.IsCancellationRequested) {
            return;
        }

        await Task.Delay(2000);
        if (ct.IsCancellationRequested) {
            return;
        }

        await _cpdFadeOut.Play();
        if (ct.IsCancellationRequested) {
            return;
        }
    }
    #endregion
}
