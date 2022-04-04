using UnityEngine;

public class UICircleEffectManager : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Canvas _canvas = null;
    [SerializeField] private UICircleEffect[] _effects = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        SetBound();
    }
    #endregion

    #region APIs

    #endregion

    #region Internal Methods
    private void SetBound() {
        if (_effects == null || _effects.Length <= 0) {
            return;
        }

        RectTransform rectCanvas = _canvas.transform as RectTransform;
        for (int i = 0; i < _effects.Length; i++) {
            _effects[i].SetBound(rectCanvas.sizeDelta.x, rectCanvas.sizeDelta.y);
        }
    }
    #endregion
}
