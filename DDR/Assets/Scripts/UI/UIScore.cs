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
    private int _currentScore = 0;
    private int _performanceScore = 0;
    private CancellationTokenSource _cts = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        GameEventManager.Instance.Register(GameEventTypes.SCORE_CHANGED, OnScoreChanged);
    }

    private void OnDestroy() {
        if (GameEventManager.Instance != null) {
            GameEventManager.Instance.Unregister(GameEventTypes.SCORE_CHANGED, OnScoreChanged);
        }
    }
    #endregion

    #region Event Handlings
    private void OnScoreChanged(BaseGameEventArgs args) {
        ScoreChangedGameEventArgs scArgs = args as ScoreChangedGameEventArgs;
        _currentScore = scArgs.CurrentScore;

        StopPerformance();
        PlayPerformance();
    }
    #endregion

    #region Internal Methods
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
