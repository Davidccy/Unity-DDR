﻿using System.Threading.Tasks;
using System.Collections.Generic;
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
        if (WindowManager.Instance != null) {
            await WindowManager.Instance.CloseWindow(Define.WIDNOW_LOADING);
        }

        _uiResult.PlayResultPerformance();
    }

    private async void OnResultFinished() {
        if (WindowManager.Instance != null) {
            await WindowManager.Instance.OpenWindow(Define.WIDNOW_LOADING);
        }

        SceneManager.LoadScene(Define.SCENE_MAIN, LoadSceneMode.Single);
    }
    #endregion
}
