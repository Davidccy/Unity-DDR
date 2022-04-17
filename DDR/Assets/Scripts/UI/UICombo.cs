using UnityEngine;
using TMPro;

public class UICombo : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private RectTransform _rectRoot = null;
    [SerializeField] private TextMeshProUGUI _textCombo = null;
    [SerializeField] private CustomPlayableDirector _cpdCombo = null;
    [SerializeField] private int _displayThreshold = 0;
    #endregion

    #region Internal Fields
    private int _comboCount = 0;
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
        EventManager.Instance.Register(EventTypes.TAP_RESULT, OnTapResult);
    }

    private void OnEnable() {
        _comboCount = 0;

        Refresh();
    }

    private void OnDestroy() {
        if (EventManager.Instance != null) {
            EventManager.Instance.Unregister(EventTypes.TAP_RESULT, OnTapResult);
        }
    }
    #endregion

    #region Event Handlings
    private void OnTapResult(BaseEventArgs args) {
        TapResultEventArgs trArgs = args as TapResultEventArgs;

        if (trArgs.TR != (int) TapResult.Miss) {
            _comboCount += 1;
        }
        else {
            _comboCount = 0;
        }

        Refresh();
    }
    #endregion

    #region Internal Methods
    private void Refresh() {
        _rectRoot.gameObject.SetActive(_comboCount >= DisplayThreshold);
        if (_comboCount < DisplayThreshold) {
            return;
        }

        _textCombo.text = string.Format("{0}", _comboCount);
        _cpdCombo.Play().DoNotAwait();
    }
    #endregion
}
