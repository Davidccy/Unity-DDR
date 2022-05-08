using UnityEngine;
using UnityEngine.UI;

public class UIButtonSE : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btn = null;
    [SerializeField] private AudioClip _acSE = null;
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
        if (_acSE != null && AudioManager.Instance != null) {
            AudioManager.Instance.PlaySE(_acSE);
        }
    }
    #endregion
}
