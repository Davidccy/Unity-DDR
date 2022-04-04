using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGame : SceneBase {
    #region Serialized Fields
    [SerializeField] private UISample _uiSample = null;
    [SerializeField] private UINodeHandler _uiNodeHandler = null;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        Init();
        OnTrackNodeReady();
    }

    protected override void OnSceneDestroy() {
        UnInit();
    }
    #endregion

    #region Internal Methods
    private void Init() {
        _uiNodeHandler.onFinalNodeFinished += OnFinalNodeFinished;
    }

    private void UnInit() {
        _uiNodeHandler.onFinalNodeFinished -= OnFinalNodeFinished;
    }

    private async void OnTrackNodeReady() {
        // TODO
        if (CommonWindowManager.Instance != null) {
            await CommonWindowManager.Instance.FadeOut();
        }

        _uiSample.ButtonOnClick();
    }

    private async void OnFinalNodeFinished() {
        if (CommonWindowManager.Instance != null) {
            await CommonWindowManager.Instance.FadeIn(CommonWindowManager.Type.Loading);
        }        

        SceneManager.LoadScene(Define.SCENE_RESULT, LoadSceneMode.Single);
    }
    #endregion

}
