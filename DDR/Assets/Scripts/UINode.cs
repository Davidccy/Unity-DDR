using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINode : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _textRemainTiming = null;
    [SerializeField] private Image _imageArrow = null;
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

        RefreshDirection();
    }
    #endregion

    #region Mono Behaviour Hooks
    private void FixedUpdate() {
        RefreshPosition();
    }
    #endregion

    #region Internal Fields
    private void RefreshDirection() {
        if (_nInfo == null) {
            return;
        }

        switch (_nInfo.Position) {
            case NodePosition.Left:
                _imageArrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case NodePosition.Up:
                _imageArrow.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case NodePosition.Down:
                _imageArrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
            case NodePosition.Right:
                _imageArrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            default:
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
