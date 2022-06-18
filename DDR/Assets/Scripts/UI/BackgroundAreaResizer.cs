using UnityEngine;

public class BackgroundAreaResizer : MonoBehaviour {
    #region Internal Fields
    private RectTransform _rt = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _rt = GetComponent<RectTransform>();

        GameEventManager.Instance.Register(GameEventTypes.RESOLUTION_CHANGED, OnResolutionChanged);
    }

    private void OnEnable() {
        Refresh();
    }

    private void OnDestroy() {
        if (GameEventManager.Instance != null) {
            GameEventManager.Instance.Unregister(GameEventTypes.RESOLUTION_CHANGED, OnResolutionChanged);
        }
    }
    #endregion

    #region Event Handlings
    private void OnResolutionChanged(BaseGameEventArgs args) {
        Refresh();
    }
    #endregion

    #region Internal Methods
    private void Refresh() {
        if (_rt == null) {
            return;
        }

#if UNITY_ANDROID
        Rect scaledSafeArea = Utility.GetScaledSafeArea();
        _rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, scaledSafeArea.x, scaledSafeArea.width);
        _rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, scaledSafeArea.y, scaledSafeArea.height);
#else
        Canvas c = Utility.GetNearestCanvas(this.transform);
        RectTransform rectCanvas = (RectTransform) c.transform;
        _rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rectCanvas.rect.width);
        _rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, rectCanvas.rect.height);
#endif
    }
    #endregion
}
