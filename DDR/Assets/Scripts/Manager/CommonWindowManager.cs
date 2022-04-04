using System.Threading.Tasks;
using UnityEngine;

public class CommonWindowManager : ISingleton<CommonWindowManager> {
    public enum Type { 
        CutScene,
        Loading,
    }

    #region Serialized Fields
    [SerializeField] private UIWindowCutScene _uiWindowCutScene = null;
    [SerializeField] private UIWindowLoading _uiWindowLoading = null;
    #endregion

    #region Internal Fields
    private Type _windowType = Type.CutScene;
    #endregion

    #region Override Methods
    protected override void DoAwake() {
        _uiWindowCutScene.gameObject.SetActive(false);
        _uiWindowLoading.gameObject.SetActive(false);
    }
    #endregion

    #region APIs
    public void SetCutSceneColor(Color c) {
        _uiWindowCutScene.SetColor(c);
    }

    public async Task FadeIn(Type type) {
        _windowType = type;

        if (_windowType == Type.CutScene) {
            _uiWindowCutScene.gameObject.SetActive(true);
            await _uiWindowCutScene.PlayFadeIn();
        }
        else if (_windowType == Type.Loading) {
            _uiWindowLoading.gameObject.SetActive(true);
            await _uiWindowLoading.PlayFadeIn();
        }
    }

    public async Task FadeOut() {
        if (_windowType == Type.CutScene) {
            await _uiWindowCutScene.PlayFadeOut();
            _uiWindowCutScene.gameObject.SetActive(false);
        }
        else if (_windowType == Type.Loading) {
            await _uiWindowLoading.PlayFadeOut();
            _uiWindowLoading.gameObject.SetActive(false);
        }
    }
    #endregion
}
