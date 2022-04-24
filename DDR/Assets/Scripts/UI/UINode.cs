using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINode : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _textRemainTiming = null;
    [SerializeField] private CanvasGroup _cgRoot = null;
    [SerializeField] private GameObject _goIconRoot = null;

    [SerializeField] private GameObject _goArrow = null;
    [SerializeField] private Image _imageArrow = null;

    [SerializeField] private GameObject _goRect = null;
    [SerializeField] private Image _imageRect = null;
    #endregion

    #region Internal Fields
    private UINodeRoot _uiNodeRoot = null;
    private NodeDisplayInfo _ndInfo;
    private float _nodeHandlingThreshold;
    private Action<UINode> _onNodeMissedAction;
    private Action<UINode> _onNodeOutOfBoundAction;
    private bool _isDone = false;
    #endregion

    #region Properties
    public NodeDisplayInfo NDInfo {
        get {
            return _ndInfo;
        }
    }

    public bool IsDone {
        get {
            return _isDone;
        }
        set {
            _isDone = value;
            OnStatusChanged();
        }
    }
    #endregion

    #region APIs
    public void SetInfo(UINodeRoot uiNodeRoot, NodeDisplayInfo ndInfo, float nodeHandlingThreshold, Action<UINode> onNodeMissedAction, Action<UINode> onNodeOutOfBoundAction) {
        _uiNodeRoot = uiNodeRoot;
        _ndInfo = ndInfo;
        _nodeHandlingThreshold = nodeHandlingThreshold;
        _onNodeMissedAction = onNodeMissedAction;
        _onNodeOutOfBoundAction = onNodeOutOfBoundAction;

        RefreshIcon();
        RefreshDirection();
        RefreshColor();
    }
    #endregion

    #region Mono Behaviour Hooks
    private void FixedUpdate() {
        RefreshPosition();

        CheckIsMissed();
        CheckIsOutOfBound();
    }
    #endregion

    #region Internal Fields


    private void RefreshIcon() {
        if (_ndInfo == null) {
            return;
        }

        _goArrow.SetActive(_ndInfo.Position != NodePosition.Space);
        _goRect.SetActive(_ndInfo.Position == NodePosition.Space);
    }

    private void RefreshDirection() {
        if (_ndInfo == null) {
            return;
        }

        switch (_ndInfo.Position) {
            case NodePosition.Left:
                _goIconRoot.transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case NodePosition.Up:
                _goIconRoot.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case NodePosition.Down:
                _goIconRoot.transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
            case NodePosition.Right:
                _goIconRoot.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case NodePosition.Space:
            default:
                _goIconRoot.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
        }
    }

    private void RefreshColor() {
        _cgRoot.alpha = IsDone ? 0.5f : 1.0f;

        _imageArrow.color = IsDone ? Color.gray : Color.red;
        _imageRect.color = IsDone ? Color.gray : Color.red;
    }

    private void RefreshPosition() {
        if (_ndInfo == null) {
            return;
        }

        float curTiming = TrackManager.Instance.TrackProgress;
        float timingDiff = _ndInfo.Timing - curTiming; // If 'timingDiff' > 0, means the node not passed yet

        _textRemainTiming.text = string.Format("{0:F4}", timingDiff);

        float height = timingDiff * _ndInfo.Speed * 100;
        height = _ndInfo.MovingType == NodeMovingType.Raising ? -height : height;

        RectTransform rt = this.transform as RectTransform;
        rt.anchoredPosition = new Vector3(0, height, 0);
    }

    private void CheckIsMissed() {
        if (IsDone) {
            return;
        }

        float curTiming = TrackManager.Instance.TrackProgress;
        float timingDiff = _ndInfo.Timing - curTiming; // If 'timingDiff' > 0, means the node not passed yet

        if (timingDiff < -_nodeHandlingThreshold) {
            _onNodeMissedAction(this);
        }
    }

    private void CheckIsOutOfBound() {
        float curTiming = TrackManager.Instance.TrackProgress;
        float timingDiff = _ndInfo.Timing - curTiming; // If 'timingDiff' > 0, means the node not passed yet

        if (timingDiff < -2) {
            _onNodeOutOfBoundAction(this);
        }
    }

    private void OnStatusChanged() {
        RefreshColor();
    }
    #endregion
}
