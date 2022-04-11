using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneGame : SceneBase {
    #region Serialized Fields
    [SerializeField] private UINodeHandler _uiNodeHandler = null;
    [SerializeField] private float _startWaiting = 0;
    [SerializeField] private float _finishedWaiting = 0;
    [SerializeField] private Button _btn = null;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        Init();
        LoadTrack();

        _btn.onClick.AddListener(ButtononClick);
    }

    protected override void OnSceneDestroy() {
        UnInit();

        _btn.onClick.RemoveListener(ButtononClick);
    }
    #endregion

    private void ButtononClick() {
        TrackManager.Instance.LoadTrackData();
    }

    #region Internal Methods
    private void Init() {
        _uiNodeHandler.onNodeGenerated += OnNodeGenerated;
        _uiNodeHandler.onFinalNodeFinished += OnFinalNodeFinished;
    }

    private void UnInit() {
        _uiNodeHandler.onNodeGenerated -= OnNodeGenerated;
        _uiNodeHandler.onFinalNodeFinished -= OnFinalNodeFinished;
    }

    private void LoadTrack() {
        TrackManager.Instance.LoadTrackData();
    }

    private async void OnNodeGenerated() {
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

    private async void OnFinalNodeFinished() {
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
