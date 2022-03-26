using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class UIMainMenu : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private GameObject _goSelectedRoot = null;
    [SerializeField] private GameObject _goContentRoot = null;

    [SerializeField] private Button _btnPrevious = null;
    [SerializeField] private Button _btnNext = null;
    [SerializeField] private Button _btnConfirm = null;
    [SerializeField] private Button _btnCancel = null;
    [SerializeField] private SelectData _selectData = null;

    [SerializeField] private List<UITrackSelectInfo> _trackSelectInfoList = new List<UITrackSelectInfo>(); // Count should be odd number

    [SerializeField] private PlayableDirector _pdTrackConfirm = null;
    [SerializeField] private PlayableDirector _pdTrackCancel = null;

    [Header("Performance Settings")]
    [SerializeField] private float _space = 0;
    [SerializeField] private float _unselectedScale = 0;
    [SerializeField] private float _performcanceDuration = 0;
    [SerializeField] private bool _switchWhenPerformance = false;
    #endregion

    #region Internal Fields
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
        _btnPrevious.onClick.AddListener(ButtonPreviousOnClick);
        _btnNext.onClick.AddListener(ButtonNextOnClick);
        _btnConfirm.onClick.AddListener(ButtonConfirmOnClick);
        _btnCancel.onClick.AddListener(ButtonCancelOnClick);
    }

    private void OnEnable() {
        Refresh(true);
    }

    private void OnDestroy() {
        _btnPrevious.onClick.RemoveListener(ButtonPreviousOnClick);
        _btnNext.onClick.RemoveListener(ButtonNextOnClick);
        _btnConfirm.onClick.RemoveListener(ButtonConfirmOnClick);
        _btnCancel.onClick.AddListener(ButtonCancelOnClick);
    }
    #endregion

    #region Button On Click Handlings
    private void ButtonPreviousOnClick() {
        ToPreviousTrack();
    }

    private void ButtonNextOnClick() {
        ToNextTrack();
    }

    private void ButtonConfirmOnClick() {
        _trackSelectInfoList[_centerUIIndex].transform.SetParent(_goSelectedRoot.transform);
        _trackSelectInfoList[_centerUIIndex].PlayFadeIn();

        _pdTrackConfirm.Play();
    }

    private void ButtonCancelOnClick() {
        _trackSelectInfoList[_centerUIIndex].PlayFadeOut();

        _pdTrackCancel.stopped -= OnFadeInFinished;
        _pdTrackCancel.stopped += OnFadeInFinished;

        _pdTrackCancel.Play();
    }

    private void OnFadeInFinished(PlayableDirector pd) {
        _trackSelectInfoList[_centerUIIndex].transform.SetParent(_goContentRoot.transform);
        _trackSelectInfoList[_centerUIIndex].transform.SetSiblingIndex(_centerUIIndex);
    }
    #endregion

    #region Internal Methods
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

            UITrackSelectInfo info = _trackSelectInfoList[uiIndex];
            info.SetTrackSelectInfo(_selectData.SelectInfos[trackDataIndex]);
            info.transform.localPosition = new Vector3(_space * positionIndex, 0, 0);
            info.transform.localScale = uiIndex == _centerUIIndex ? Vector3.one : Vector3.one * _unselectedScale;
        }
    }

    private void ToPreviousTrack() {
        if (_coPerformance != null) {
            StopCoroutine(_coPerformance);
        }
        _coPerformance = StartCoroutine(CoPlayPerformance(false));
    }

    private void ToNextTrack() {
        if (_coPerformance != null) {
            StopCoroutine(_coPerformance);
        }
        _coPerformance = StartCoroutine(CoPlayPerformance(true));
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
                info.transform.localPosition = Vector3.Lerp(startPos, goalPos, progress);

                // Scale
                Vector3 startScale = uiIndex == _centerUIIndex ? Vector3.one : Vector3.one * _unselectedScale;
                Vector3 goalScale = uiIndex == nextCenterUIIndex ? Vector3.one : Vector3.one * _unselectedScale;

                info.transform.localScale = Vector3.Lerp(startScale, goalScale, progress);
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
