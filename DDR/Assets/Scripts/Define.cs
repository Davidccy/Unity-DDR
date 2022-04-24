﻿using UnityEngine;
using System.Collections.Generic;

#region Enum
public enum NodePosition {
    Left,
    Up,
    Down,
    Right,
    Space,
}

public enum TapResult {
    Perfect,
    Great,
    Good,
    Miss,
}

public enum ResultRank { 
    SSS,
    SS,
    S,
    A,
    B,
    C,
}

public enum NodeMovingType { 
    Falling,
    Raising,
}

public enum ControlType { 
    Keyboard,
    Touching,
}
#endregion

#region Temp Data
public class TempResultData {
    public int Score;
    public int TotalTaps;
    public Dictionary<TapResult, int> Taps;
}
#endregion

[System.Serializable]
public class NodeData {
    public int Measure;             // Start from 1
    public NodeInfo[] NodeInfoList;
}

[System.Serializable]
public class NodeInfo {
    public float Timing;
    public NodePosition NodePosition;
}


public class NodeInfoTest {
    public NodePosition Position;
    public float Speed;
    public float Timing;
}

[System.Serializable]
public class SelectInfo {
    public int TrackID;
    public string TrackName;
    public Sprite Thumbnail;
    public int BPM;
}

public static class Define {
    #region Track Speed
    public static int SPEED_LEVEL_MIN = 1;
    public static int SPEED_LEVEL_MAX = 19;
    public static float SPEED_PER_LEVEL = 0.5f;
    public static float SPEED_BASE = 0.5f;
    #endregion

    #region Scene Names
    public static string SCENE_COMMON = "Common";
    public static string SCENE_GAME = "Game";
    public static string SCENE_INIT = "Init";
    public static string SCENE_MAIN = "Main";
    public static string SCENE_RESULT = "Result";
    #endregion

    #region Game Data Keys
    public static string GAME_DATA_KEY_SE_TYPE = "Game_Data_SE_Type"; // TODO
    public static string GAME_DATA_KEY_SPEED_LEVEL = "Game_Data_Speed_Level";
    public static string GAME_DATA_KEY_NODE_MOVING_TYPE = "Game_Data_Node_Moving_Type";
    public static string GAME_DATA_KEY_CONTROL_TYPE = "Game_Data_Control_Type";
    #endregion

    #region Temporary Game Data Keys
    public static string TEMP_GAME_DATA_KEY_SELECTED_TRACK_ID = "Temp_Selected_Track_ID";
    public static string TEMP_GAME_DATA_KEY_RESULT = "Temp_Result";
    #endregion
}
