using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISample : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btn = null;
    [SerializeField] private TextMeshProUGUI _textTrackProgress = null;
    [SerializeField] private TextMeshProUGUI _textCurMeasure = null;
    [SerializeField] private Image _imageProgress = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btn.onClick.AddListener(ButtonOnClick);
    }

    private void Update() {
        Refresh();
    }

    private void OnDestroy() {
        _btn.onClick.RemoveListener(ButtonOnClick);
    }
    #endregion

    #region Button Handlings
    private void ButtonOnClick() {
        // Load notes
        PlayerPrefs.SetInt(Utility.PLAYER_PREF_TRACK_ID, 1);
        TrackManager.Instance.LoadTrackData();
    }
    #endregion

    #region Internal Methods
    private void Refresh() {
        float trackProgress = TrackManager.Instance.TrackProgress;
        int minutes = Mathf.Max(0, (int) trackProgress / 60);
        int seconds = Mathf.Max(0, (int) trackProgress % 60);
        _textTrackProgress.text = string.Format("{0}:{1:00}", minutes, seconds);
        _textCurMeasure.text = string.Format("{0}", TrackManager.Instance.CurMeasure);

        float trackLength = TrackManager.Instance.TrackLength;
        float fillAmount = trackLength != 0 ? trackProgress / trackLength : 0;
        _imageProgress.fillAmount = fillAmount;
    }
    #endregion
}
