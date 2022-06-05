using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGame : SceneBase {
    #region Serialized Fields
    [SerializeField] private UIWallpaperResizer _uiWallpaperResizer = null;
    [SerializeField] private float _startWaiting = 0;
    [SerializeField] private float _finishedWaiting = 0;

    [SerializeField] private UIAchievementEffect _uiAchvEffectAP = null;
    [SerializeField] private UIAchievementEffect _uiAchvEffectFC = null;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        GameEventManager.Instance.Register(GameEventTypes.NODE_GENERATED, OnNodeGenerated);
        GameEventManager.Instance.Register(GameEventTypes.TRACK_ACHIEVEMENT, OnTrackAchievement);

        LoadTrack();
    }

    protected override void OnSceneDestroy() {
        if (GameEventManager.Instance != null) {
            GameEventManager.Instance.Unregister(GameEventTypes.NODE_GENERATED, OnNodeGenerated);
            GameEventManager.Instance.Unregister(GameEventTypes.TRACK_ACHIEVEMENT, OnTrackAchievement);
        }
    }
    #endregion

    #region Internal Methods
    private async void LoadTrack() {
        await Task.Delay(100);

        TrackManager.Instance.LoadTrackData();
    }

    private async void OnNodeGenerated(BaseGameEventArgs args) {
        _uiWallpaperResizer.SetSprite(TrackManager.Instance.TrackData.Wallpaper);

        await Task.Delay((int) _startWaiting * 1000);

        // Start playing track
        TrackManager.Instance.PlayTrack();
        // Start playing track

        // NOTE:
        // Fade out after starting
        if (WindowManager.Instance != null) {
            await WindowManager.Instance.CloseWindow(Define.WIDNOW_LOADING);
        }
    }

    private async void OnTrackAchievement(BaseGameEventArgs args) {
        TrackAchievementGameEventArgs taArgs = args as TrackAchievementGameEventArgs;
        if (taArgs.IsAllPerfect) {
            await _uiAchvEffectAP.Play();
        }
        else if (taArgs.IsFullCombo) {
            await _uiAchvEffectFC.Play();
        }

        await Task.Delay((int) _finishedWaiting * 1000);

        if (WindowManager.Instance != null) {
            await WindowManager.Instance.OpenWindow(Define.WIDNOW_LOADING);
        }

        TrackManager.Instance.Stop();

        SceneManager.LoadScene(Define.SCENE_RESULT, LoadSceneMode.Single);
    }
    #endregion
}
