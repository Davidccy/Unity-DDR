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
        EventManager.Instance.Register(EventTypes.NODE_GENERATED, OnNodeGenerated);
        EventManager.Instance.Register(EventTypes.FINAL_NODE_FINISHED, OnFinalNodeFinished);

        LoadTrack();
    }

    protected override void OnSceneDestroy() {
        if (EventManager.Instance != null) {
            EventManager.Instance.Unregister(EventTypes.NODE_GENERATED, OnNodeGenerated);
            EventManager.Instance.Unregister(EventTypes.FINAL_NODE_FINISHED, OnFinalNodeFinished);
        }
    }
    #endregion

    #region Internal Methods
    private async void LoadTrack() {
        await Task.Delay(100);

        TrackManager.Instance.LoadTrackData();
    }

    private async void OnNodeGenerated(BaseEventArgs args) {
        await Task.Delay((int) _startWaiting * 1000);

        // Start playing track
        TrackManager.Instance.PlayTrack();
        // Start playing track

        // NOTE:
        // Fade out after starting
        if (CommonWindowManager.Instance != null) {
            await CommonWindowManager.Instance.FadeOut();
        }
    }

    private async void OnFinalNodeFinished(BaseEventArgs args) {
        await Task.Delay((int) _finishedWaiting * 1000);

        if (CommonWindowManager.Instance != null) {
            await CommonWindowManager.Instance.FadeIn(CommonWindowManager.Type.Loading);
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
