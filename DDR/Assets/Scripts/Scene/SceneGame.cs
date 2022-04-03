using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneGame : SceneBase {
    #region Serialized Fields
    [SerializeField] private UISample _uiSample = null;
    [SerializeField] private UINodeHandler _uiNodeHandler = null;
    #endregion

    #region Internal Fields
    private int _curPageIndex = -1;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        Init();
    }

    protected override void OnSceneDestroy() {
        UnInit();
    }
    #endregion

    #region Internal Methods
    private async void Init() {
        _uiNodeHandler.onFinalNodeFinished += OnFinalNodeFinished;

        await CommonWindowManager.Instance.CutSceneFadeOut();
        _uiSample.ButtonOnClick();
    }

    private void UnInit() {
        _uiNodeHandler.onFinalNodeFinished -= OnFinalNodeFinished;
    }

    private async void OnFinalNodeFinished() {
        await CommonWindowManager.Instance.CutSceneFadeIn();

        SceneManager.LoadScene(Define.SCENE_RESULT, LoadSceneMode.Single);
    }
    #endregion

}
