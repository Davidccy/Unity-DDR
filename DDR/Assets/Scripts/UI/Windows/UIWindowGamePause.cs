using UnityEngine;
using UnityEngine.UI;

public class UIWindowGamePause : UIGenericWindow {
    #region Serialized Fields
    [SerializeField] private Button _btnRetry = null;
    [SerializeField] private Button _btnSelect = null;
    [SerializeField] private Button _btnResume = null;

    [SerializeField] private CustomPlayableDirector _cpdCountDown = null;
    #endregion

    #region Exposed Fields
    public override string WindowName => Define.WIDNOW_GAME_PAUSE;
    #endregion

    #region Override Methods
    protected override void OnWindowAwake() {
        _btnRetry.onClick.AddListener(ButtonRetryOnClick);
        _btnSelect.onClick.AddListener(ButtonSelectOnClick);
        _btnResume.onClick.AddListener(ButtonResumeOnClick);
    }

    protected override void OnWindowDestroy() {
        _btnRetry.onClick.RemoveListener(ButtonRetryOnClick);
        _btnSelect.onClick.RemoveListener(ButtonSelectOnClick);
        _btnResume.onClick.RemoveListener(ButtonResumeOnClick);
    }
    #endregion

    #region Button Handlings
    private async void ButtonRetryOnClick() {
        if (WindowManager.Instance != null) {
            await WindowManager.Instance.CloseWindow(Define.WIDNOW_GAME_PAUSE);
        }        

        TrackRetryGameEventArgs args = new TrackRetryGameEventArgs();
        args.Dispatch();
    }

    private async void ButtonSelectOnClick() {
        if (WindowManager.Instance != null) {
            await WindowManager.Instance.CloseWindow(Define.WIDNOW_GAME_PAUSE);
        }

        TrackAbortGameEventArgs args = new TrackAbortGameEventArgs();
        args.Dispatch();
    }

    private async void ButtonResumeOnClick() {
        //if (WindowManager.Instance != null) {
        //    await WindowManager.Instance.CloseWindow(Define.WIDNOW_GAME_PAUSE);
        //}

        //TrackResumeGameEventArgs args = new TrackResumeGameEventArgs();
        //args.Dispatch();

        await _cpdCountDown.Play();

        if (WindowManager.Instance != null) {
            await WindowManager.Instance.CloseWindow(Define.WIDNOW_GAME_PAUSE, false, true);
        }

        TrackResumeGameEventArgs args = new TrackResumeGameEventArgs();
        args.Dispatch();
    }
    #endregion
}
