using UnityEngine;

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

    #region Select Data Handlings
    public static SelectData GetSelectData() {
        SelectData td = Resources.Load<SelectData>("Data/SelectData");
        return td;
    }
    #endregion

    #region Track Handlings
    public static TrackData GetTrackData(int trackID) {
        TrackData td = Resources.Load<TrackData>(string.Format("Data/TrackData/TrackData_{0:000}", trackID));
        return td;
    }
    #endregion

    #region Audio Handlings
    public static AudioClip GetTrack(int trackID) {
        AudioClip ac = Resources.Load<AudioClip>(string.Format("Audio/Track/Track_{0:000}", trackID));
        return ac;
    }

    public static AudioClip GetSEReady() {
        AudioClip ac = Resources.Load<AudioClip>("Audio/SE/SoundEffect_001");
        return ac;
    }

    public static AudioClip GetSEPerfect() {
        AudioClip ac = Resources.Load<AudioClip>("Audio/SE/SoundEffect_001");
        return ac;
    }

    public static AudioClip GetSENormal() {
        AudioClip ac = Resources.Load<AudioClip>("Audio/SE/SoundEffect_002");
        return ac;
    }
    #endregion

    #region Track Speed Handlings
    public static float GetTrackSpeed() {
        int speedLevel = GameDataManager.LoadInt(Define.GAME_DATA_KEY_SPEED_LEVEL);
        return speedLevel * Define.SPEED_PER_LEVEL + Define.SPEED_BASE;
    }
    #endregion

    #region Score rank Handlings
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
}
