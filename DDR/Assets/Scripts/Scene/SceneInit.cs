using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInit : SceneBase {
    #region Serialized Fields
    [SerializeField] private UILogo _uiLogo = null;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        Init();
        PlayPerformance();
    }
    #endregion

    #region Internal Methods
    private void Init() {
        SceneManager.LoadScene(Define.SCENE_COMMON, LoadSceneMode.Additive);
    }

    private async void PlayPerformance() {
        await ShowLogo();
        await ShowCutScene();

        SceneManager.LoadScene(Define.SCENE_MAIN, LoadSceneMode.Single);
    }

    private async Task ShowLogo() {
        await _uiLogo.Play();
    }

    private async Task ShowCutScene() {
        if (CommonWindowManager.Instance != null) {
            CommonWindowManager.Instance.SetCutSceneColor(Color.black);
            await CommonWindowManager.Instance.FadeIn(CommonWindowManager.Type.CutScene);
        }        
    }
    #endregion
}
