using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneInit : SceneBase {
    #region Serialized Fields
    [SerializeField] private UILogo _uiLogo = null;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        Init();
    }
    #endregion

    #region Internal Methods
    private async void Init() {
        SceneManager.LoadScene(Define.SCENE_COMMON, LoadSceneMode.Additive);

        await ShowLogo();
        await ShowCutScene();

        SceneManager.LoadScene(Define.SCENE_MAIN, LoadSceneMode.Single);
    }

    private async Task ShowLogo() {
        await _uiLogo.Play();
    }

    private async Task ShowCutScene() {
        CommonWindowManager.Instance.SetCutSceneColor(Color.black);
        await CommonWindowManager.Instance.CutSceneFadeIn();
    }
    #endregion
}
