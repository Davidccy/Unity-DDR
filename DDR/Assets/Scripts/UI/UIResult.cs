using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResult : MonoBehaviour {
    public enum ResultStep { 
        None,
        Title,
        Tap,
        Rank,
        Continue,
        Finish,
    }

    public class TapScore {
        public int Score;
        public int ScoreMax;
    }

    #region Serialized Fields
    [Header("Track Info")]
    [SerializeField] private TextMeshProUGUI _textTrackInfo = null;
    [SerializeField] private CustomPlayableDirector _cpdTrackInfo = null;

    [Header("Tap")]
    [SerializeField] private UIResultTap _resultTapPerfect = null;
    [SerializeField] private UIResultTap _resultTapGreat = null;
    [SerializeField] private UIResultTap _resultTapGood = null;
    [SerializeField] private UIResultTap _resultTapMiss = null;
    [SerializeField] private CustomPlayableDirector _cpdTap = null;

    [Header("Rank")]
    [SerializeField] private CustomPlayableDirector _cpdRank = null;

    [Header("Continue")]
    [SerializeField] private CustomPlayableDirector _cpdContinue = null;

    [Header("Other Settings")]
    [SerializeField] private Button _btnNext = null;
    #endregion

    #region Internal Fields
    private ResultStep _rStep = ResultStep.None;
    private bool _resultFInishedTriggered = false;
    #endregion

    #region Exposed Fields
    public Action onResultFinished;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btnNext.onClick.AddListener(ButtonNextOnClick);
    }

    private void OnDestroy() {
        _btnNext.onClick.RemoveListener(ButtonNextOnClick);
    }
    #endregion

    #region Button Handlings
    private void ButtonNextOnClick() {
        ToNextStep();
    }
    #endregion

    #region Internal Methods
    public async void PlayResultPerformance() {
        InitPerformance();

        await PlayTitle();
        await PlayResultTap();
        await PlayResultRank();
        await PlayTapToContinue();

        _rStep = ResultStep.Finish;
    }

    private void InitPerformance() {
        _resultFInishedTriggered = false;

        _resultTapPerfect.SetScore(0, 0);
        _resultTapGreat.SetScore(0, 0);
        _resultTapGood.SetScore(0, 0);
        _resultTapMiss.SetScore(0, 0);
    }

    private async Task PlayTitle() {
        _rStep = ResultStep.Title;

        await _cpdTrackInfo.Play();
    }

    private async Task PlayResultTap() {
        TempResultData rd = TempDataManager.LoadData<TempResultData>(Define.TEMP_GAME_DATA_KEY_RESULT);

        _rStep = ResultStep.Tap;

        List<Task> tasks = new List<Task>();
        tasks.Add(_cpdTap.Play());
        tasks.Add(_resultTapPerfect.Play(rd.Taps[TapResult.Perfect], rd.TotalTaps));
        tasks.Add(_resultTapGreat.Play(rd.Taps[TapResult.Great], rd.TotalTaps));
        tasks.Add(_resultTapGood.Play(rd.Taps[TapResult.Good], rd.TotalTaps));
        tasks.Add(_resultTapMiss.Play(rd.Taps[TapResult.Miss], rd.TotalTaps));
        await Task.WhenAll(tasks.ToArray());
    }

    private async Task PlayResultRank() {
        _rStep = ResultStep.Rank;

        await _cpdRank.Play();
    }

    private async Task PlayTapToContinue() {
        _rStep = ResultStep.Continue;

        await _cpdContinue.Play();
    }

    private void ToNextStep() {
        if (_rStep == ResultStep.Title) {
            _cpdTrackInfo.SetFinish();
        }
        else if (_rStep == ResultStep.Tap) {
            _cpdTap.SetFinish();
            _resultTapPerfect.SetFinish();
            _resultTapGreat.SetFinish();
            _resultTapGood.SetFinish();
            _resultTapMiss.SetFinish();
        }
        else if(_rStep == ResultStep.Rank) {
            _cpdRank.SetFinish();
        }
        else if(_rStep == ResultStep.Continue) {
            _cpdContinue.SetFinish();
        }
        else if(_rStep == ResultStep.Finish) {
            ResultFinish();
        }
    }

    private void ResultFinish() {
        if (_resultFInishedTriggered) {
            return;
        }

        _resultFInishedTriggered = true;

        if (onResultFinished != null) {
            onResultFinished();
        }
    }
    #endregion
}
