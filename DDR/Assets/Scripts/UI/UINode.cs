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
    private NodeInfoTest _nInfo;
    private float _nodeHandlingThreshold;
    private Action<UINode> _onNodeMissedAction;
    private Action<UINode> _onNodeOutOfBoundAction;
    private bool _isDone = false;
    #endregion

    #region Properties
    public NodeInfoTest NInfo {
        get {
            return _nInfo;
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
    public void SetInfo(UINodeRoot uiNodeRoot, NodeInfoTest nInfo, float nodeHandlingThreshold, Action<UINode> onNodeMissedAction, Action<UINode> onNodeOutOfBoundAction) {
        _uiNodeRoot = uiNodeRoot;
        _nInfo = nInfo;
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
        if (_nInfo == null) {
            return;
        }

        _goArrow.SetActive(_nInfo.Position != NodePosition.Space);
        _goRect.SetActive(_nInfo.Position == NodePosition.Space);
    }

    private void RefreshDirection() {
        if (_nInfo == null) {
            return;
        }

        switch (_nInfo.Position) {
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
        if (_nInfo == null) {
            return;
        }

        float curTiming = TrackManager.Instance.TrackProgress;
        float timingDiff = _nInfo.Timing - curTiming; // If 'timingDiff' > 0, means the node not passed yet

        _textRemainTiming.text = string.Format("{0:F4}", timingDiff);

        RectTransform rt = this.transform as RectTransform;
        rt.anchoredPosition = new Vector3(0, -(timingDiff * _nInfo.Speed * 100), 0);
    }

    private void CheckIsMissed() {
        if (IsDone) {
            return;
        }

        float curTiming = TrackManager.Instance.TrackProgress;
        float timingDiff = _nInfo.Timing - curTiming; // If 'timingDiff' > 0, means the node not passed yet

        if (timingDiff < -_nodeHandlingThreshold) {
            _onNodeMissedAction(this);
        }
    }

    private void CheckIsOutOfBound() {
        float curTiming = TrackManager.Instance.TrackProgress;
        float timingDiff = _nInfo.Timing - curTiming; // If 'timingDiff' > 0, means the node not passed yet

        if (timingDiff < -2) {
            _onNodeOutOfBoundAction(this);
        }
    }

    private void OnStatusChanged() {
        RefreshColor();
    }
    #endregion
}
