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
    private static int _curPageIndex = -1;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        HideAllPages();

        if (_curPageIndex == -1) {
            if (WindowManager.Instance != null) {
                WindowManager.Instance.CloseWindow(Define.WIDNOW_CUT_SCENE).DoNotAwait();
            }
        }
        else {
            if (WindowManager.Instance != null) {
                WindowManager.Instance.CloseWindow(Define.WIDNOW_LOADING).DoNotAwait();
            }
        }

        if (_curPageIndex == -1) {
            ChangeToPage((UIPage) _initPageIndex, false).DoNotAwait();
        }
        else {
            ChangeToPage((UIPage) _curPageIndex, false).DoNotAwait();
        }        
    }
    #endregion

    #region APIs
    public async Task ChangeToPage(UIPage page, bool useCutScene = true) {
        if (useCutScene && WindowManager.Instance != null) {
            await WindowManager.Instance.OpenWindow(Define.WIDNOW_CUT_SCENE);
        }

        if (_curPageIndex != -1) {
            await _uiPages[_curPageIndex].PlayFadeOut();
            _uiPages[_curPageIndex].OnFadeOutDone();
            _uiPages[_curPageIndex].gameObject.SetActive(false);
        }

        _curPageIndex = (int) page;
        _uiPages[_curPageIndex].gameObject.SetActive(true);
        if (useCutScene && WindowManager.Instance != null) {
            WindowManager.Instance.CloseWindow(Define.WIDNOW_CUT_SCENE).DoNotAwait();
        }

        await _uiPages[_curPageIndex].PlayFadeIn();
        _uiPages[_curPageIndex].OnFadeInDone();
    }

    public async Task TrackSelectionFinished() {
        if (WindowManager.Instance != null) {
            await WindowManager.Instance.OpenWindow(Define.WIDNOW_LOADING);
        }
        SceneManager.LoadScene(Define.SCENE_GAME, LoadSceneMode.Single);
    }
    #endregion

    #region Internal Methods
    private void HideAllPages() {
        for (int i = 0; i < _uiPages.Length; i++) {
            _uiPages[i].gameObject.SetActive(false);
        }
    }
    #endregion
}
