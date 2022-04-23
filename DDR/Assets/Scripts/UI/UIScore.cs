using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class UIScore : MonoBehaviour {    
    #region Serialized Fields
    [SerializeField] private List<TextMeshProUGUI> _textScoreList = null;
    [SerializeField] private float _performanceDuration = 0.5f;
    #endregion

    #region Internal Fields
    private readonly int _TOTAL_SCORE = 1000000;
    private int _totalNode = 0;
    private Dictionary<TapResult, int> _tapResultData = new Dictionary<TapResult, int>();
    private int _currentScore = 0;
    private int _performanceScore = 0;
    private CancellationTokenSource _cts = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        EventManager.Instance.Register(EventTypes.TRACK_LOADED, OnTrackLoaded);
        EventManager.Instance.Register(EventTypes.TAP_RESULT, OnTapResult);
        EventManager.Instance.Register(EventTypes.FINAL_NODE_FINISHED, OnFinalNodeFinished);
    }

    private void OnDestroy() {
        if (EventManager.Instance != null) {
            EventManager.Instance.Unregister(EventTypes.TRACK_LOADED, OnTrackLoaded);
            EventManager.Instance.Unregister(EventTypes.TAP_RESULT, OnTapResult);
            EventManager.Instance.Unregister(EventTypes.FINAL_NODE_FINISHED, OnFinalNodeFinished);
        }
    }
    #endregion

    #region Event Handlings
    private void OnTrackLoaded(BaseEventArgs args) {
        RebuildScoreData();
        UpdateScore();
    }

    private void OnTapResult(BaseEventArgs args) {
        TapResultEventArgs trArgs = args as TapResultEventArgs;

        _tapResultData[trArgs.TR] += 1;

        UpdateScore();
    }

    private void OnFinalNodeFinished(BaseEventArgs args) {
        TempResultData rd = new TempResultData();
        rd.Score = _currentScore;
        rd.TotalTaps = _totalNode;
        rd.Taps = _tapResultData;

        TempDataManager.SaveData(Define.TEMP_GAME_DATA_KEY_RESULT, rd);
    }
    #endregion

    #region Internal Methods
    private void RebuildScoreData() {
        TrackData td = TrackManager.Instance.TrackData;

        // Track's node
        _totalNode = 0;
        for (int i = 0; i < td.Nodes.Length; i++) {
            NodeData nd = td.Nodes[i];
            _totalNode += nd.NodeInfoList.Length;
        }

        // Tap result data
        _tapResultData.Clear();
        foreach (TapResult tr in Enum.GetValues(typeof(TapResult))) {
            _tapResultData.Add(tr, 0);
        }

        // Performance
        _currentScore = 0;
        _performanceScore = 0;
    }

    private void UpdateScore() {
        float totalGainedPoint = 0;
        totalGainedPoint += _tapResultData[TapResult.Miss] * 0.0f;
        totalGainedPoint += _tapResultData[TapResult.Good] * 0.5f;
        totalGainedPoint += _tapResultData[TapResult.Great] * 0.8f;
        totalGainedPoint += _tapResultData[TapResult.Perfect] * 1.0f;

        _currentScore = (int) (_TOTAL_SCORE * ((float) totalGainedPoint / _totalNode));

        StopPerformance();
        PlayPerformance();
    }

    private void PlayPerformance() {
        StopPerformance();

        _cts = new CancellationTokenSource();

        Performance(_cts.Token, _currentScore);
    }

    private void StopPerformance() {
        if (_cts != null) {
            _cts.Cancel();
        }

        _cts = null;
    }

    private async void Performance(CancellationToken ct, int goalValue) {
        float passedTime = 0;
        float progress = 0;
        int startValue = _performanceScore;

        while (passedTime < _performanceDuration) {
            if (ct.IsCancellationRequested) {
                return;
            }

            progress = passedTime / _performanceDuration;
            _performanceScore = (int) Mathf.Lerp(startValue, goalValue, progress);
            ShowScore(string.Format("{0:0000000}", _performanceScore));

            await Task.Delay(1);

            passedTime += Time.deltaTime;
        }

        _performanceScore = goalValue;
        ShowScore(string.Format("{0:0000000}", _performanceScore));
    }

    private void ShowScore(string scoreStr) {
        int scoreTextCount = _textScoreList.Count;
        for (int i = 0; i < _textScoreList.Count; i++) {
            string subScoreStr = scoreStr.Substring(scoreTextCount - i - 1, 1);
            _textScoreList[i].text = string.Format("{0}", subScoreStr);
        }
    }
    #endregion
}
