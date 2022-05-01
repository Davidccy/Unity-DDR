using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITrackInfo : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _textTrackName = null;

    [SerializeField] private TextMeshProUGUI _textTrackProgress = null;
    [SerializeField] private TextMeshProUGUI _textTrackProgressReverse = null;
    [SerializeField] private Image _imageProgress = null;
    [SerializeField] private TextMeshProUGUI _textCurMeasure = null;
    #endregion

    #region Internal Fields
    private SelectInfo _selectInfo = null;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        Init();
        RefreshTrackName();
    }

    private void Update() {
        Refresh();
    }
    #endregion

    #region Internal Methods
    private void Init() {
        int trackID = TempDataManager.LoadData<int>(Define.TEMP_GAME_DATA_KEY_SELECTED_TRACK_ID);
        _selectInfo = Utility.GetSelectInfo(trackID);
    }

    private void RefreshTrackName() {
        _textTrackName.text = string.Format("{0}", _selectInfo.TrackName);
    }

    private void Refresh() {
        float trackProgress = TrackManager.Instance.TrackProgress;
        int minutes = Mathf.Max(0, (int) trackProgress / 60);
        int seconds = Mathf.Max(0, (int) trackProgress % 60);
        _textTrackProgress.text = string.Format("{0}:{1:00}", minutes, seconds);

        float trackProgressReverse = TrackManager.Instance.TrackLength - trackProgress;
        minutes = Mathf.Max(0, (int) trackProgressReverse / 60);
        seconds = Mathf.Max(0, (int) trackProgressReverse % 60);
        _textTrackProgressReverse.text = string.Format("{0}:{1:00}", minutes, seconds);

        _textCurMeasure.text = string.Format("{0}", TrackManager.Instance.CurMeasure);

        bool isTrackEnd = TrackManager.Instance.IsTrackEnd;
        float trackLength = TrackManager.Instance.TrackLength;
        float fillAmount = trackLength != 0 ? trackProgress / trackLength : 0;
        _imageProgress.fillAmount = fillAmount;
    }
    #endregion
}
