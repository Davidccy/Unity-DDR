using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMain : SceneBase {
    public enum UIPage { 
        Entry,
        TrackSelection,
    }

    #region Serialized Fields
    [SerializeField] private UIMainPageBase[] _uiPages = null;
    [SerializeField] private int _initPageIndex = 0;
    #endregion

    #region Internal Fields
    private int _curPageIndex = -1;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        ChangeToPage((UIPage) _initPageIndex, false);
    }
    #endregion

    #region APIs
    public async Task ChangeToPage(UIPage page, bool cutSceneFadeIn = true) {
        if (cutSceneFadeIn) {
            await CommonWindowManager.Instance.FadeIn(CommonWindowManager.Type.CutScene);
        }

        if (_curPageIndex != -1) {
            await _uiPages[_curPageIndex].PlayFadeOut();
            _uiPages[_curPageIndex].gameObject.SetActive(false);
        }

        _curPageIndex = (int) page;
        _uiPages[_curPageIndex].gameObject.SetActive(true);
        await _uiPages[_curPageIndex].PlayFadeIn();
        await CommonWindowManager.Instance.FadeOut();
    }

    public async Task TrackSelectionFinished() {
        await CommonWindowManager.Instance.FadeIn(CommonWindowManager.Type.Loading);
        SceneManager.LoadScene(Define.SCENE_GAME, LoadSceneMode.Single);
    }
    #endregion

    #region Internal Methods
    #endregion
}
