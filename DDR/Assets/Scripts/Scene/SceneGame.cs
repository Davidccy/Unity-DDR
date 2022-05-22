using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneGame : SceneBase {
    #region Serialized Fields
    [SerializeField] private UINodeHandler _uiNodeHandler = null;
    [SerializeField] private float _startWaiting = 0;
    [SerializeField] private float _finishedWaiting = 0;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        GameEventManager.Instance.Register(GameEventTypes.NODE_GENERATED, OnNodeGenerated);
        GameEventManager.Instance.Register(GameEventTypes.FINAL_NODE_FINISHED, OnFinalNodeFinished);

        LoadTrack();
    }

    protected override void OnSceneDestroy() {
        if (GameEventManager.Instance != null) {
            GameEventManager.Instance.Unregister(GameEventTypes.NODE_GENERATED, OnNodeGenerated);
            GameEventManager.Instance.Unregister(GameEventTypes.FINAL_NODE_FINISHED, OnFinalNodeFinished);
        }
    }
    #endregion

    #region Internal Methods
    private async void LoadTrack() {
        await Task.Delay(100);

        TrackManager.Instance.LoadTrackData();
    }

    private async void OnNodeGenerated(BaseGameEventArgs args) {
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

    private async void OnFinalNodeFinished(BaseGameEventArgs args) {
        if (TrackManager.Instance.IsEditorMode) {
            return;
        }

        await Task.Delay((int) _finishedWaiting * 1000);

        if (WindowManager.Instance != null) {
            await WindowManager.Instance.OpenWindow(Define.WIDNOW_LOADING);
        }

        TrackManager.Instance.Stop();

        SceneManager.LoadScene(Define.SCENE_RESULT, LoadSceneMode.Single);
    }

    private bool IsAllperfect() {
        // TODO
        return false;
    }
    #endregion
}
