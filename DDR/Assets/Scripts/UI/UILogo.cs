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
    private bool _canSkipWaiting = false;
    private bool _skipWaiting = false;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btn.onClick.AddListener(ButtonOnClick);
    }

    private void OnDestroy() {
        _btn.onClick.RemoveListener(ButtonOnClick);
    }
    #endregion

    #region Button Handlings
    private void ButtonOnClick() {
        if (_canSkipWaiting) {
            _skipWaiting = true;
        }
    }
    #endregion

    #region APIs
    public async Task Play() {
        _canSkipWaiting = false;
        _skipWaiting = false;

        _cpdFadeIn.Stop();
        _cpdFadeOut.Stop();

        await _cpdFadeIn.Play();
        _canSkipWaiting = true;

        await Wait();
        await _cpdFadeOut.Play();
    }
    #endregion

    #region Internal Methods
    private async Task Wait() {
        int milliSeconds = 0;
        while (!_skipWaiting && milliSeconds < _waitSeconds * 1000) {
            await Task.Delay(10);
            milliSeconds += 10;
        }
    }
    #endregion
}
