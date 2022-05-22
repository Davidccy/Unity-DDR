using UnityEngine;
using Newtonsoft.Json;

public static class Utility {
    #region Node Related Handlings     
    public static string GetNodeResultText(NodeResult nr) {
        string text = string.Empty;

        if (nr == NodeResult.Miss) {
            text = "Miss ..";
        }
        else if (nr == NodeResult.Good) {
            text = "Good";
        }
        else if (nr == NodeResult.Great) {
            text = "Great !";
        }
        else if (nr == NodeResult.Perfect) {
            text = "PERFECT !!!";
        }

        return text;
    }

    public static Color GetNodeResultColor(NodeResult nr) {
        Color c = Color.white;

        if (nr == NodeResult.Miss) {
            c = Color.gray;
        }
        else if (nr == NodeResult.Good) {
            c = Color.green;
        }
        else if (nr == NodeResult.Great) {
            c = Color.red;
        }
        else if (nr == NodeResult.Perfect) {
            c = Color.yellow;
        }

        return c;
    }
    #endregion

    #region Game Config Handlings
    private static GameConfigData _gameConfigData = null;
    public static GameConfigData GameConfigData {
        get {
            if (_gameConfigData == null) {
                _gameConfigData = Resources.Load<GameConfigData>("Data/GameConfigData");
            }

            return _gameConfigData;
        }
    }

    public static AudioClip GetSEReady() {
        if (GameConfigData == null) {
            return null;
        }

        return GameConfigData.SoundEffectReady;
    }

    public static AudioClip GetSEPerfect() {
        if (GameConfigData == null) {
            return null;
        }

        int seIndex = GameDataManager.LoadInt(Define.GAME_DATA_KEY_SE_PERFECT);
        if (seIndex >= GameConfigData.SoundEffectPerfect.Length) {
            return GameConfigData.SoundEffectPerfect[0];
        }

        return GameConfigData.SoundEffectPerfect[seIndex];
    }

    public static AudioClip GetSENormal() {
        if (GameConfigData == null) {
            return null;
        }

        int seIndex = GameDataManager.LoadInt(Define.GAME_DATA_KEY_SE_NORMAL);
        if (seIndex >= GameConfigData.SoundEffectNormal.Length) {
            return GameConfigData.SoundEffectNormal[0];
        }

        return GameConfigData.SoundEffectNormal[seIndex];
    }
    #endregion

    #region Select Data Handlings
    private static SelectData _selectData = null;
    public static SelectData SelectData {
        get {
            if (_selectData == null) {
                _selectData = Resources.Load<SelectData>("Data/SelectData");
            }

            return _selectData;
        }
    }

    public static SelectInfo GetSelectInfo(int trackID) {
        if (SelectData == null) {
            return null;
        }

        for (int i = 0; i < SelectData.SelectInfos.Length; i++) {
            if (SelectData.SelectInfos[i].TrackID == trackID) {
                return SelectData.SelectInfos[i];
            }
        }

        return null;
    }
    #endregion

    #region Track Handlings
    public static TrackData GetTrackData(int trackID) {
        TrackData td = Resources.Load<TrackData>(string.Format("Data/TrackData/TrackData_{0:000}", trackID));
        return td;
    }
    #endregion

    #region Track Speed Handlings
    public static float GetTrackSpeed() {
        int speedLevel = GameDataManager.LoadInt(Define.GAME_DATA_KEY_SPEED_LEVEL);
        return speedLevel * Define.SPEED_PER_LEVEL + Define.SPEED_BASE;
    }
    #endregion

    #region Score Rank Handlings
    public static ScoreRank GetScoreRank(int score) {
        if (score >= Define.SCORE_MIN_RANK_S) {
            return ScoreRank.S;
        }
        else if (score >= Define.SCORE_MIN_RANK_A) {
            return ScoreRank.A;
        }
        else if (score >= Define.SCORE_MIN_RANK_B) {
            return ScoreRank.B;
        }
        else if (score >= Define.SCORE_MIN_RANK_C) {
            return ScoreRank.C;
        }
        else if (score >= Define.SCORE_MIN_RANK_D) {
            return ScoreRank.D;
        }

        return ScoreRank.E;
    }

    public static string GetScoreRankText(int score) {
        ScoreRank rank = GetScoreRank(score);
        return GetScoreRankText(rank);
    }

    public static string GetScoreRankText(ScoreRank rank) {
        if (rank == ScoreRank.S) {
            return "S";
        }
        else if (rank == ScoreRank.A) {
            return "A";
        }
        else if (rank == ScoreRank.B) {
            return "B";
        }
        else if (rank == ScoreRank.C) {
            return "C";
        }
        else if (rank == ScoreRank.D) {
            return "D";
        }

        return "E";
    }
    #endregion

    #region Track Achievement Handlings
    public static void SaveTrackAchievement(int trackID, TrackAchievement achv) {
        string jsonString = JsonConvert.SerializeObject(achv);
        string saveKey = string.Format(Define.GAME_DATA_KEY_TRACK_ACHV, trackID);
        GameDataManager.SaveString(saveKey, jsonString);
    }

    public static TrackAchievement LoadTrackAchievement(int trackID) {
        string saveKey = string.Format(Define.GAME_DATA_KEY_TRACK_ACHV, trackID);
        string jsonString = GameDataManager.LoadString(saveKey);
        if (string.IsNullOrEmpty(jsonString)) {
            return null;
        }

        TrackAchievement achv = JsonConvert.DeserializeObject<TrackAchievement>(jsonString);
        return achv;
    }
    #endregion

    #region Misc
    public static Canvas GetNearestCanvas(Transform tf) {
        Canvas c = tf.GetComponent<Canvas>();
        if (c != null) {
            return c;
        }

        if (tf.parent != null) {
            return GetNearestCanvas(tf.parent);
        }

        return null;
    }
    #endregion
}
