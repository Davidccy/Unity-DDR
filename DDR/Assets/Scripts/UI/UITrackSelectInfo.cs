using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;

public class UITrackSelectInfo : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Image _imageThumbnail = null;
    [SerializeField] private TextMeshProUGUI _textBPM = null;
    [SerializeField] private PlayableDirector _pdConfirm = null;
    [SerializeField] private PlayableDirector _pdCancel = null;
    #endregion

    #region Internal Fields
    private SelectInfo _sInfo;
    #endregion

    #region APIs
    public void SetTrackSelectInfo(SelectInfo sInfo) {
        _sInfo = sInfo;
        Refresh();
    }
    #endregion

    #region Internal Methods
    private void Refresh() {
        _imageThumbnail.sprite = _sInfo.Thumbnail;
        _textBPM.text = string.Format("BPM : {0}", _sInfo.BPM);
    }
    #endregion

    #region APIs
    public void PlayFadeIn() {
        _pdCancel.Stop();

        _pdConfirm.Play();
    }

    public void PlayFadeOut() {
        _pdConfirm.Stop();

        _pdCancel.Play();
    }
    #endregion
}
