﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class GameScoreHandler : MonoBehaviour {
    #region Internal Fields
    // Score
    private int _totalNode = 0;
    private int _curScore = 0;
    private Dictionary<NodeResult, int> _nodeResultTable = new Dictionary<NodeResult, int>();

    // Combo
    private int _curCombo = 0;
    private int _maxCombo = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        GameEventManager.Instance.Register(GameEventTypes.TRACK_LOADED, OnTrackLoaded);
        GameEventManager.Instance.Register(GameEventTypes.NODE_RESULT, ONodeResult);
        GameEventManager.Instance.Register(GameEventTypes.FINAL_NODE_FINISHED, OnFinalNodeFinished);
    }

    private void OnDestroy() {
        if (GameEventManager.Instance != null) {
            GameEventManager.Instance.Unregister(GameEventTypes.TRACK_LOADED, OnTrackLoaded);
            GameEventManager.Instance.Unregister(GameEventTypes.NODE_RESULT, ONodeResult);
            GameEventManager.Instance.Unregister(GameEventTypes.FINAL_NODE_FINISHED, OnFinalNodeFinished);
        }
    }
    #endregion

    #region Event Handlings
    private void OnTrackLoaded(BaseGameEventArgs args) {
        InitScoreData();
        InitComboData();
    }

    private void ONodeResult(BaseGameEventArgs args) {
        NodeResultGameEventArgs nrArgs = args as NodeResultGameEventArgs;

        UpdateScore(nrArgs.NR);
        UpdateCombo(nrArgs.NR);
    }

    private void OnFinalNodeFinished(BaseGameEventArgs args) {
        TempResultData trd = new TempResultData();
        trd.Score = _curScore;
        trd.TotalNodeCount = _totalNode;
        trd.MaxCombo = _maxCombo;
        trd.NodeResultTable = _nodeResultTable;

        TempDataManager.SaveData(Define.TEMP_GAME_DATA_KEY_RESULT, trd);
    }
    #endregion

    #region Internal Methods
    private void InitScoreData() {
        TrackData td = TrackManager.Instance.TrackData;

        // Track's node
        _curScore = 0;
        _totalNode = 0;
        for (int i = 0; i < td.Nodes.Length; i++) {
            NodeData nd = td.Nodes[i];
            _totalNode += nd.NodeInfoList.Length;
        }

        // Init table
        _nodeResultTable.Clear();
        foreach (NodeResult nr in Enum.GetValues(typeof(NodeResult))) {
            _nodeResultTable.Add(nr, 0);
        }

        ScoreChangedGameEventArgs args = new ScoreChangedGameEventArgs(0, 0);
        args.Dispatch();
    }

    private void InitComboData() {
        _curCombo = 0;
        _maxCombo = 0;

        ComboChangedGameEventArgs args = new ComboChangedGameEventArgs(0, 0);
        args.Dispatch();
    }

    private void UpdateScore(NodeResult nr) {
        int preScore = _curScore;

        _nodeResultTable[nr] += 1;

        float totalGainedPoint = 0;
        totalGainedPoint += _nodeResultTable[NodeResult.Miss] * 0.0f;
        totalGainedPoint += _nodeResultTable[NodeResult.Good] * 0.5f;
        totalGainedPoint += _nodeResultTable[NodeResult.Great] * 0.8f;
        totalGainedPoint += _nodeResultTable[NodeResult.Perfect] * 1.0f;

        _curScore = (int) (Define.TOTAL_SCORE * ((float) totalGainedPoint / _totalNode));

        if (preScore != _curScore) {
            ScoreChangedGameEventArgs args = new ScoreChangedGameEventArgs(preScore, _curScore);
            args.Dispatch();
        }
    }

    private void UpdateCombo(NodeResult nr) {
        int preCombo = _curCombo;
        if (nr == NodeResult.Miss) {
            _curCombo = 0;
        }
        else {
            _curCombo += 1;
        }

        if (_curCombo > _maxCombo) {
            _maxCombo = _curCombo;
        }

        ComboChangedGameEventArgs args = new ComboChangedGameEventArgs(preCombo, _curCombo);
        args.Dispatch();
    }
    #endregion
}