using UnityEngine;

public class UICircleEffectManager : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private UICircleEffect[] _effects = null;
    #endregion

    #region Internal Fields
    private Canvas _canvas = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        InitCanvas();
        SetBound();
    }
    #endregion

    #region Internal Methods
    private void InitCanvas() {
        _canvas = Utility.GetNearestCanvas(this.transform);
    }

    private void SetBound() {
        if (_effects == null || _effects.Length <= 0) {
            return;
        }

        if (_canvas == null) {
            return;
        }

        RectTransform rectCanvas = _canvas.transform as RectTransform;
        for (int i = 0; i < _effects.Length; i++) {
            _effects[i].SetBound(rectCanvas.sizeDelta.x, rectCanvas.sizeDelta.y);
        }
    }
    #endregion
}
