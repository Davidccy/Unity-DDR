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
        CommonWindowManager.Instance.SetCutSceneColor(Color.white);
        await CommonWindowManager.Instance.CutSceneFadeIn();

        SceneManager.LoadScene(Define.SCENE_MAIN, LoadSceneMode.Single);
    }
    #endregion
}
