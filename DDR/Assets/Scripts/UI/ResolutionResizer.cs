using System;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionResizer : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private CanvasScaler _cs = null;
    #endregion

    #region Exposed Fields
    public Action onChanged;
    #endregion

    #region Internal Fields
    private ScreenOrientation _lastScreenOri;
    private Vector2 _lastResolution;
    #endregion

    #region Mono Behaviours Hooks
    private void Update() {
        Refresh();
    }
    #endregion

    #region Internal Methods
    private void Refresh() {
        if (_cs == null) {
            return;
        }

        if (!IsScreenOrientationChanged() && !IsResolutionChanged()) {
            return;
        }

        _lastScreenOri = Screen.orientation;
        _lastResolution = new Vector2(Screen.width, Screen.height);

        _cs.referenceResolution = new Vector2(Define.RESOLUTION_WIDTH, Define.RESOLUTION_HEIGHT);
        _cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        _cs.matchWidthOrHeight = Utility.GetMatchWidthOrHeight();

        ResolutionChangedGameEventArgs args = new ResolutionChangedGameEventArgs();
        args.Dispatch();
    }

    private bool IsScreenOrientationChanged() {
        return _lastScreenOri != Screen.orientation;
    }

    private bool IsResolutionChanged() {
        return _lastResolution != new Vector2(Screen.width, Screen.height);
    }
    #endregion
}
