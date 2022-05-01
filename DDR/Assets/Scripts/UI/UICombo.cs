using UnityEngine;
using TMPro;

public class UICombo : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private RectTransform _rectRoot = null;
    [SerializeField] private TextMeshProUGUI _textCombo = null;
    [SerializeField] private CustomPlayableDirector _cpdCombo = null;
    [SerializeField] private int _displayThreshold = 0;
    #endregion

    #region Properties
    private int DisplayThreshold {
        get {
            return Mathf.Max(1, _displayThreshold);
        }
    }
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        GameEventManager.Instance.Register(GameEventTypes.COMBO_CHANGED, OnComboChanged);
    }

    private void OnDestroy() {
        if (GameEventManager.Instance != null) {
            GameEventManager.Instance.Unregister(GameEventTypes.COMBO_CHANGED, OnComboChanged);
        }
    }
    #endregion

    #region Event Handlings
    private void OnComboChanged(BaseGameEventArgs args) {
        ComboChangedGameEventArgs ccArgs = args as ComboChangedGameEventArgs;
        int curCombo = ccArgs.CurrentCombo;
        _rectRoot.gameObject.SetActive(curCombo >= DisplayThreshold);
        if (curCombo < DisplayThreshold) {
            return;
        }

        _textCombo.text = string.Format("{0}", curCombo);
        _cpdCombo.Play().DoNotAwait();
    }
    #endregion
}
