using System.Threading.Tasks;
using UnityEngine;

public class CommonWindowManager : ISingleton<CommonWindowManager> {
    #region Serialized Fields
    [SerializeField] private UIWindowCutScene _uiWindowCutScene = null;
    #endregion

    #region APIs
    public void SetCutSceneColor(Color c) {
        _uiWindowCutScene.SetColor(c);
    }

    public async Task CutSceneFadeIn() {
        await _uiWindowCutScene.PlayFadeIn();
    }

    public async Task CutSceneFadeOut() {
        await _uiWindowCutScene.PlayFadeOut();
    }
    #endregion
}
