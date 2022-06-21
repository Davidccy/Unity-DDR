using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainPageTrackSelection : UIMainPageBase {
    #region Serialized Fields
    [Header("Sub Content")]
    [SerializeField] private GameObject _goSelectedRoot = null;
    [SerializeField] private GameObject _goContentRoot = null;

    [SerializeField] private Button _btnBack = null;

    [SerializeField] private Button _btnPrevious = null;
    [SerializeField] private Button _btnNext = null;
    [SerializeField] private Button _btnConfirm = null;

    [SerializeField] private Button _btnStart = null;
    [SerializeField] private Button _btnOption = null;

    [SerializeField] private Button _btnConfirmCancel = null;

    [SerializeField] private List<UITrackSelectInfo> _trackSelectInfoList = new List<UITrackSelectInfo>(); // Count should be odd number

    [SerializeField] private CustomPlayableDirector _cpdTrackConfirm = null;
    [SerializeField] private CustomPlayableDirector _cpdTrackCancel = null;

    [Header("Sub Content - Performance Settings")]
    [SerializeField] private float _space = 0;
    [SerializeField] private float _unselectedScale = 0;
    [SerializeField] private float _performcanceDuration = 0;
    [SerializeField] private bool _switchWhenPerformance = false;
    [SerializeField] private AudioClip _acTrackSelection = null;
    [SerializeField] private float _alphaThreshold = 0;
    [SerializeField] private float _alphaMaxDistance = 0; // Not include threshold
    #endregion

    #region Internal Fields
    private SelectData _selectData = null;

    private int _centerTrackDataIndex = 0;
    private int _centerUIIndex = 0;
    private bool _isPerforming = false;
    private Coroutine _coPerformance = null;
    #endregion

    #region Properties
    private int TotalSelectDataCount {
        get {
            return _selectData.SelectInfos.Length;
        }
    }

    private int TotalUICount {
        get {
            return _trackSelectInfoList.Count;
        }
    }
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btnBack.onClick.AddListener(ButtonBackOnClick);
        _btnPrevious.onClick.AddListener(ButtonPreviousOnClick);
        _btnNext.onClick.AddListener(ButtonNextOnClick);
        _btnConfirm.onClick.AddListener(ButtonConfirmOnClick);
        _btnStart.onClick.AddListener(ButtonStartOnClick);
        _btnOption.onClick.AddListener(ButtonOptionOnClick);
        _btnConfirmCancel.onClick.AddListener(ButtonConfirmCancelOnClick);
    }

    private void OnEnable() {
        InitSelectData();
        Refresh(true);
    }

    private void OnDestroy() {
        _btnBack.onClick.RemoveListener(ButtonBackOnClick);
        _btnPrevious.onClick.RemoveListener(ButtonPreviousOnClick);
        _btnNext.onClick.RemoveListener(ButtonNextOnClick);
        _btnConfirm.onClick.RemoveListener(ButtonConfirmOnClick);
        _btnStart.onClick.RemoveListener(ButtonStartOnClick);
        _btnOption.onClick.RemoveListener(ButtonOptionOnClick);
        _btnConfirmCancel.onClick.RemoveListener(ButtonConfirmCancelOnClick);
    }
    #endregion

    #region Button Handlings
    private async void ButtonBackOnClick() {
        AudioManager.Instance.StopBGM().DoNotAwait();

        await _sceneMain.ChangeToPage(SceneMain.UIPage.Entry);
    }

    private void ButtonPreviousOnClick() {
        if (_isPerforming) {
            return;
        }

        ToPreviousTrack();
    }

    private void ButtonNextOnClick() {
        if (_isPerforming) {
            return;
        }

        ToNextTrack();
    }

    private void ButtonConfirmOnClick() {
        if (_isPerforming) {
            return;
        }

        _trackSelectInfoList[_centerUIIndex].transform.SetParent(_goSelectedRoot.transform);
        _trackSelectInfoList[_centerUIIndex].PlayFadeIn();

        _cpdTrackConfirm.Play().DoNotAwait();
    }

    private void ButtonStartOnClick() {
        SelectInfo sInfo = _selectData.SelectInfos[_centerTrackDataIndex];
        TempDataManager.SaveData(Define.TEMP_GAME_DATA_KEY_SELECTED_TRACK_ID, sInfo.TrackID);
        _sceneMain.TrackSelectionFinished().DoNotAwait();
    }

    private async void ButtonOptionOnClick() {
        if (WindowManager.Instance != null) {
            await WindowManager.Instance.OpenWindow(Define.WIDNOW_TRACK_OPTION);
        }
    }

    private void ButtonConfirmCancelOnClick() {
        _trackSelectInfoList[_centerUIIndex].PlayFadeOut();

        _cpdTrackCancel.Play(() => {
            _trackSelectInfoList[_centerUIIndex].transform.SetParent(_goContentRoot.transform);
            _trackSelectInfoList[_centerUIIndex].transform.SetSiblingIndex(_centerUIIndex);
        }).DoNotAwait();
    }
    #endregion

    #region Internal Methods
    private void InitSelectData() {
        if (_selectData == null) {
            _selectData = Utility.SelectData;
        }
    }

    private void Refresh(bool resetCenterUI = false) {
        int totalUICount = TotalUICount;
        int totalSelectDataCount = TotalSelectDataCount;
        if (resetCenterUI) {
            _centerUIIndex = totalUICount / 2;
        }

        for (int uiIndex = 0; uiIndex < _trackSelectInfoList.Count; uiIndex++) {
            int positionIndex = (uiIndex - _centerUIIndex);
            if (positionIndex < -(totalUICount / 2)) {
                positionIndex += TotalUICount;
            }
            else if (positionIndex > (totalUICount / 2)) {
                positionIndex -= totalUICount;
            }

            int trackDataIndex = _centerTrackDataIndex + positionIndex;
            if (trackDataIndex < 0) {
                trackDataIndex += totalSelectDataCount;
            }
            else if (trackDataIndex >= totalSelectDataCount) {
                trackDataIndex -= totalSelectDataCount;
            }

            float xPos = _space * positionIndex;
            float alpha = Mathf.Abs(xPos) <= _alphaThreshold ? 1 : 1 - (Mathf.Abs(xPos) - _alphaThreshold) / _alphaMaxDistance;

            UITrackSelectInfo info = _trackSelectInfoList[uiIndex];
            info.SetTrackSelectInfo(_selectData.SelectInfos[trackDataIndex]);
            info.transform.localPosition = new Vector3(xPos, 0, 0);
            info.transform.localScale = uiIndex == _centerUIIndex ? Vector3.one : Vector3.one * _unselectedScale;            
            info.SetAlpha(alpha);
        }

        AudioClip acShort = _selectData.SelectInfos[_centerTrackDataIndex].TrackData.AudioTrackShort;
        AudioManager.Instance.PlayBGM(acShort).DoNotAwait();
    }

    private void ToPreviousTrack() {
        if (_coPerformance != null) {
            StopCoroutine(_coPerformance);
        }
        _coPerformance = StartCoroutine(CoPlayPerformance(false));

        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySE(_acTrackSelection);
        }
    }

    private void ToNextTrack() {
        if (_coPerformance != null) {
            StopCoroutine(_coPerformance);
        }
        _coPerformance = StartCoroutine(CoPlayPerformance(true));

        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySE(_acTrackSelection);
        }
    }

    private void SetCenterTrackDataIndex(int index) {
        _centerTrackDataIndex = index;

        if (_centerTrackDataIndex < 0) {
            _centerTrackDataIndex += TotalSelectDataCount;
        }
        else if (_centerTrackDataIndex >= TotalSelectDataCount) {
            _centerTrackDataIndex -= TotalSelectDataCount;
        }
    }

    private void SetCenterUIIndex(int index) {
        _centerUIIndex = index;

        if (_centerUIIndex < 0) {
            _centerUIIndex += TotalUICount;
        }
        else if (_centerUIIndex >= TotalUICount) {
            _centerUIIndex -= TotalUICount;
        }
    }

    private IEnumerator CoPlayPerformance(bool toNext) {
        if (!_switchWhenPerformance && _isPerforming) {
            yield break;
        }

        _isPerforming = true;

        AudioManager.Instance.StopBGM().DoNotAwait();

        float startTime = Time.realtimeSinceStartup;
        float passedTime = 0;
        float progress = 0;

        int totalUICount = TotalUICount;
        int nextCenterUIIndex = toNext ? _centerUIIndex + 1 : _centerUIIndex - 1;
        if (nextCenterUIIndex < 0) {
            nextCenterUIIndex += totalUICount;
        }
        else if (nextCenterUIIndex >= totalUICount) {
            nextCenterUIIndex -= totalUICount;
        }

        while (passedTime < _performcanceDuration) {
            yield return new WaitForEndOfFrame();

            passedTime = Time.realtimeSinceStartup - startTime;
            progress = passedTime / _performcanceDuration;
            for (int uiIndex = 0; uiIndex < _trackSelectInfoList.Count; uiIndex++) {
                int curPositionIndex = (uiIndex - _centerUIIndex);
                if (curPositionIndex < -(totalUICount / 2)) {
                    curPositionIndex += TotalUICount;
                }
                else if (curPositionIndex > (totalUICount / 2)) {
                    curPositionIndex -= TotalUICount;
                }

                int goalPositionIndex = toNext ? curPositionIndex - 1 : curPositionIndex + 1;

                UITrackSelectInfo info = _trackSelectInfoList[uiIndex];

                // Position
                Vector3 startPos = new Vector3(_space * curPositionIndex, 0, 0);
                Vector3 goalPos = new Vector3(_space * goalPositionIndex, 0, 0);
                Vector3 localPos = Vector3.Lerp(startPos, goalPos, progress);
                info.transform.localPosition = localPos;

                // Scale
                Vector3 startScale = uiIndex == _centerUIIndex ? Vector3.one : Vector3.one * _unselectedScale;
                Vector3 goalScale = uiIndex == nextCenterUIIndex ? Vector3.one : Vector3.one * _unselectedScale;
                info.transform.localScale = Vector3.Lerp(startScale, goalScale, progress);

                // Alpha
                float xPos = localPos.x;
                float alpha = Mathf.Abs(xPos) <= _alphaThreshold ? 1 : 1 - (Mathf.Abs(xPos) - _alphaThreshold) / _alphaMaxDistance;
                info.SetAlpha(alpha);

                // Unselected mask alpha
                float startAlpha = uiIndex == _centerUIIndex ? 0 : 1;
                float goalAlpha = uiIndex == nextCenterUIIndex ? 0 : 1;
                float maskAlpha = Mathf.Lerp(startAlpha, goalAlpha, progress);
                info.SetUnselectedMaskAlpha(maskAlpha);
            }
        }

        if (toNext) {
            SetCenterTrackDataIndex(_centerTrackDataIndex + 1);
            SetCenterUIIndex(_centerUIIndex + 1);
        }
        else {
            SetCenterTrackDataIndex(_centerTrackDataIndex - 1);
            SetCenterUIIndex(_centerUIIndex - 1);
        }

        Refresh();

        _isPerforming = false;
    }
    #endregion
}
