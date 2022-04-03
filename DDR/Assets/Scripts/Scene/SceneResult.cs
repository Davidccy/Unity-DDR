using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResult : SceneBase {
    #region Serialized Fields
    [SerializeField] private UIResult _uiResult = null;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        Init();
        PlayPerformance();
    }

    protected override void OnSceneDestroy() {
        UnInit();
    }
    #endregion

    #region Internal Methods
    private void Init() {
        _uiResult.onResultFinished += OnResultFinished;
    }

    private void UnInit() {
        _uiResult.onResultFinished -= OnResultFinished;
    }

    private async void PlayPerformance() {
        if (CommonWindowManager.Instance != null) {
            await CommonWindowManager.Instance.CutSceneFadeOut();
        }

        _uiResult.PlayResultPerformance();
    }

    private async void OnResultFinished() {
        Debug.LogErrorFormat("OnResulteFinished");

        CommonWindowManager.Instance.SetCutSceneColor(Color.white);
        Debug.LogErrorFormat("fade in");
        await CommonWindowManager.Instance.CutSceneFadeIn();
        Debug.LogErrorFormat("fade in done");

        SceneManager.LoadScene(Define.SCENE_MAIN, LoadSceneMode.Single);
    }
    #endregion
}
