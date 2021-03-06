using UnityEngine;
using System.Collections.Generic;

#region Enum
public enum NodePosition {
    Left,
    Up,
    Down,
    Right,
    Space,
}

public enum NodeType { 
    Tap,
    Hold,
}

public enum NodeResult {
    Perfect,
    Great,
    Good,
    Miss,
}

public enum ScoreRank { 
    S,
    A,
    B,
    C,
    D,
    E,
}

public enum NodeMovingType {
    Raising,
    Falling,    
}

public enum ControlType { 
    Keyboard,
    Touching,
}
#endregion

#region Temp Data
public class TempResultData {
    public int Score;
    public int TotalNodeCount;
    public int MaxCombo;
    public Dictionary<NodeResult, int> NodeResultTable;
    public bool IsAllPerfect;
    public bool IsFullCombo;
    public bool IsNewRecord;
    public int MyBestScore;
}
#endregion

#region Scriptable Object Data
[System.Serializable]
public class NodeData {
    public int Measure;
    public NodeInfo[] NodeInfoList;
}

[System.Serializable]
public class NodeInfo {
    public float Timing;
    public float Timing2;
    public NodeType NodeType;
    public NodePosition NodePosition;
}

[System.Serializable]
public class SelectInfo {
    public int TrackID;
    public TrackData TrackData;
    //public string TrackName;
    //public Sprite Thumbnail;
    //public int BPM;
}
#endregion

public class NodeDisplayInfo {
    public NodePosition Position;
    public NodeMovingType MovingType;
    public float Speed;
    public float Timing;
}

public class TrackAchievement {
    public int Score;
    public int Combo;
    public bool IsAllPerfect;
    public bool IsFullCombo;
}

public static class Define {
    #region Resolution
    public static int RESOLUTION_WIDTH = 800;
    public static int RESOLUTION_HEIGHT = 600;
    #endregion

    #region Editor Related Settings
    public static string EDITOR_SCENE_PATH = "Assets/Scenes/Game.unity";
    public static string EDITOR_ASSET_PATH = "Assets/Resources/Data/TrackData";
    public static string EDITOR_ASSET_DEFAULT_NAME = "TrackData_000";
    #endregion

    #region Scene Names
    public static string SCENE_COMMON = "Common";
    public static string SCENE_GAME = "Game";
    public static string SCENE_INIT = "Init";
    public static string SCENE_MAIN = "Main";
    public static string SCENE_RESULT = "Result";    
    #endregion

    #region Window Names
    public static string WIDNOW_CREDITS = "UIWindowCredits";
    public static string WIDNOW_CUT_SCENE = "UIWindowCutScene";
    public static string WIDNOW_GAME_PAUSE = "UIWindowGamePause";
    public static string WIDNOW_LOADING = "UIWindowLoading";
    public static string WIDNOW_TRACK_OPTION = "UIWindowTrackOption";
    public static string WIDNOW_TRACK_RESUME = "UIWindowTrackResume";
    #endregion

    #region Audio Settings
    public static float AUDIO_DEFAULT_BGM_VOLUME = 0.5f;
    public static float AUDIO_DEFAULT_SE_VOLUME = 0.5f;
    public static float AUDIO_BGM_FADE_IN_DURATION = 0.25f;
    public static float AUDIO_BGM_FADE_OUT_DURATION = 0.25f;
    #endregion

    #region Track Speed
    public static int SPEED_LEVEL_MIN = 1;
    public static int SPEED_LEVEL_MAX = 19;
    public static float SPEED_PER_LEVEL = 0.5f;
    public static float SPEED_BASE = 0.5f;
    #endregion

    #region Track Score
    public static int TOTAL_SCORE = 1000000;
    public static int SCORE_MIN_RANK_S = 900000;
    public static int SCORE_MIN_RANK_A = 800000;
    public static int SCORE_MIN_RANK_B = 700000;
    public static int SCORE_MIN_RANK_C = 600000;
    public static int SCORE_MIN_RANK_D = 500000;
    #endregion

    #region Default Settings
    public static int DEFAULT_AUDIO_SE_PERFECT = 0;
    public static int DEFAULT_AUDIO_SE_NORMAL = 1;
    public static int DEFAULT_SPEED_LEVEL = 6; // Level 6, value = 3
    public static int DEFAULT_NODE_MOVING_TYPE = (int) NodeMovingType.Raising;
    public static int DEFAULT_CONTROL_TYPE = (int) ControlType.Keyboard;
    #endregion

    #region Game Data Keys
    // Options
    public static string GAME_DATA_KEY_SE_PERFECT = "Game_Data_SE_Perfect";
    public static string GAME_DATA_KEY_SE_NORMAL = "Game_Data_SE_Normal";
    public static string GAME_DATA_KEY_VOLUME_BGM = "Game_Data_Volume_BGM";
    public static string GAME_DATA_KEY_VOLUME_SE = "Game_Data_Volume_SE";
    public static string GAME_DATA_KEY_SPEED_LEVEL = "Game_Data_Speed_Level";
    public static string GAME_DATA_KEY_NODE_MOVING_TYPE = "Game_Data_Node_Moving_Type";
    public static string GAME_DATA_KEY_CONTROL_TYPE = "Game_Data_Control_Type";

    // Track achievements
    public static string GAME_DATA_KEY_TRACK_ACHV = "Game_Data_Track_Achv_{0:000}"; // Fill track ID as parameter
    #endregion

    #region Temporary Game Data Keys
    public static string TEMP_GAME_DATA_KEY_SELECTED_TRACK_ID = "Temp_Selected_Track_ID";
    public static string TEMP_GAME_DATA_KEY_RESULT = "Temp_Result";
    #endregion
}
