using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINode : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _textRemainTiming = null;
    [SerializeField] private GameObject _goIconRoot = null;
    [SerializeField] private GameObject _goArrow = null;
    [SerializeField] private GameObject _goRect = null;
    #endregion

    #region Internal Fields
    private UINodeRoot _uiNodeRoot = null;
    private NodeInfoTest _nInfo;
    #endregion

    #region Properties
    public NodeInfoTest NInfo {
        get {
            return _nInfo;
        }
    }
    #endregion

    #region APIs
    public void SetInfo(UINodeRoot uiNodeRoot, NodeInfoTest nInfo) {
        _uiNodeRoot = uiNodeRoot;
        _nInfo = nInfo;

        RefreshIcon();
        RefreshDirection();
    }
    #endregion

    #region Mono Behaviour Hooks
    private void FixedUpdate() {
        RefreshPosition();
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
    #endregion
}
