using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITrackSelectInfo : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Image _imageThumbnail = null;
    [SerializeField] private CanvasGroup _cgUnselectedMask = null;
    [SerializeField] private TextMeshProUGUI _textBPM = null;
    [SerializeField] private TextMeshProUGUI _textTrackName = null;
    [SerializeField] private TextMeshProUGUI _textHighScore = null;
    [SerializeField] private RectTransform _rectHighScoreRank = null;
    [SerializeField] private TextMeshProUGUI _textHighScoreRank = null;
    [SerializeField] private RectTransform _rectFullCombo = null;
    [SerializeField] private RectTransform _rectAllPerfect = null;
    [SerializeField] private CustomPlayableDirector _cpdConfirm = null;
    [SerializeField] private CustomPlayableDirector _cpdCancel = null;
    #endregion

    #region Internal Fields
    private SelectInfo _sInfo;
    #endregion

    #region APIs
    public void SetTrackSelectInfo(SelectInfo sInfo) {
        _sInfo = sInfo;
        Refresh();
    }

    public void SetUnselectedMaskAlpha(float alpha) {
        _cgUnselectedMask.alpha = alpha;
    }
    #endregion

    #region Internal Methods
    private void Refresh() {
        _imageThumbnail.sprite = _sInfo.TrackData.Thumbnail;
        _textBPM.text = string.Format("{0}", _sInfo.TrackData.BPM);
        _textTrackName.text = string.Format("{0}", _sInfo.TrackData.TrackName);

        TrackAchievement achv = Utility.LoadTrackAchievement(_sInfo.TrackID);
        _textHighScore.text = achv != null ? string.Format("{0:0000000}", achv.Score) : "---";
        _rectHighScoreRank.gameObject.SetActive(achv != null);
        _textHighScoreRank.text = achv != null ? string.Format("{0}", Utility.GetScoreRankText(achv.Score)) : string.Empty;
        _rectFullCombo.gameObject.SetActive(achv != null && achv.IsFullCombo);
        _rectAllPerfect.gameObject.SetActive(achv != null && achv.IsAllPerfect);
    }
    #endregion

    #region APIs
    public void PlayFadeIn() {
        _cpdCancel.Stop();

        _cpdConfirm.Play().DoNotAwait();
    }

    public void PlayFadeOut() {
        _cpdConfirm.Stop();

        _cpdCancel.Play().DoNotAwait();
    }
    #endregion
}
