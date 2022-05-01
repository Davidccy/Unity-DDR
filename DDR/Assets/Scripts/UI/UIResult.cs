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
        Node,
        Rank,
        Continue,
        Finish,
    }

    #region Serialized Fields
    [Header("Track Info")]
    [SerializeField] private TextMeshProUGUI _textTrackName = null;
    [SerializeField] private Image _imageTrackThumbnail = null;
    [SerializeField] private CustomPlayableDirector _cpdTrackInfo = null;

    [Header("Node & Score")]
    [SerializeField] private UIResultValueRolling _resultPerfect = null;
    [SerializeField] private UIResultValueRolling _resultGreat = null;
    [SerializeField] private UIResultValueRolling _resultGood = null;
    [SerializeField] private UIResultValueRolling _resultMiss = null;
    [SerializeField] private UIResultValueRolling _resultCombo = null;
    [SerializeField] private UIResultValueRolling _resultScore = null;
    [SerializeField] private TextMeshProUGUI _textMyBestScore = null;
    [SerializeField] private CustomPlayableDirector _cpdNode = null;

    [Header("Rank")]
    [SerializeField] private TextMeshProUGUI _textScoreRank = null;
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
        await PlayResultNode();
        await PlayResultRank();
        await PlayTapToContinue();

        _rStep = ResultStep.Finish;
    }

    private void InitPerformance() {
        _resultFInishedTriggered = false;

        int trackID = TempDataManager.LoadData<int>(Define.TEMP_GAME_DATA_KEY_SELECTED_TRACK_ID);
        SelectInfo sInfo = Utility.GetSelectInfo(trackID);
        _textTrackName.text = sInfo.TrackName;
        _imageTrackThumbnail.sprite = sInfo.Thumbnail;

        TempResultData trd = TempDataManager.LoadData<TempResultData>(Define.TEMP_GAME_DATA_KEY_RESULT);
        _resultPerfect.SetScore(trd.NodeResultTable[NodeResult.Perfect], trd.TotalNodeCount);
        _resultGreat.SetScore(trd.NodeResultTable[NodeResult.Great], trd.TotalNodeCount);
        _resultGood.SetScore(trd.NodeResultTable[NodeResult.Good], trd.TotalNodeCount);
        _resultMiss.SetScore(trd.NodeResultTable[NodeResult.Miss], trd.TotalNodeCount);
        _resultCombo.SetScore(trd.MaxCombo, trd.TotalNodeCount);
        _resultScore.SetScore(trd.Score, trd.TotalNodeCount);
        _textMyBestScore.text = "1234567"; // TODO

        string rank = Utility.GetScoreRankText(trd.Score);
        _textScoreRank.text = rank;
    }

    private async Task PlayTitle() {
        _rStep = ResultStep.Title;

        await _cpdTrackInfo.Play();
    }

    private async Task PlayResultNode() {
        _rStep = ResultStep.Node;

        List<Task> tasks = new List<Task>();
        tasks.Add(_cpdNode.Play());
        tasks.Add(_resultPerfect.Play((float) 0 / 60));
        tasks.Add(_resultGreat.Play((float) 2 / 60));
        tasks.Add(_resultGood.Play((float) 4 / 60));
        tasks.Add(_resultMiss.Play((float) 6 / 60));
        tasks.Add(_resultCombo.Play((float) 28 / 60));
        tasks.Add(_resultScore.Play((float) 30 / 60));
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
        else if (_rStep == ResultStep.Node) {
            _cpdNode.SetFinish();
            _resultPerfect.SetFinish();
            _resultGreat.SetFinish();
            _resultGood.SetFinish();
            _resultMiss.SetFinish();
            _resultCombo.SetFinish();
            _resultScore.SetFinish();
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
