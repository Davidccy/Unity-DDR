using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITrackInfo : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _textTrackProgress = null;
    [SerializeField] private TextMeshProUGUI _textTrackProgressReverse = null;
    [SerializeField] private TextMeshProUGUI _textCurMeasure = null;
    [SerializeField] private Image _imageProgress = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Update() {
        Refresh();
    }
    #endregion

    #region Internal Methods
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
