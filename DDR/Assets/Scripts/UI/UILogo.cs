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
        // Do nothing
    }
    #endregion

    #region APIs
    public async Task Play() {
        _cpdFadeIn.Stop();
        _cpdFadeOut.Stop();

        await _cpdFadeIn.Play();
        await Task.Delay(2000);
        await _cpdFadeOut.Play();
    }
    #endregion
}
