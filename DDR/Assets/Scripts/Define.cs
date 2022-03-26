using UnityEngine;
using System.Collections.Generic;

public enum NodePosition {
    Left,
    Up,
    Down,
    Right,
    Space,
}

public enum TapResult {
    Miss,
    Good,
    Great,
    Perfect,
}

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
    public static string PLAYER_PREF_KEY_TRACK_NAME = "Track_Name";
    public static string PLAYER_PREF_KEY_SE_TYPE = "SE_Type";
}
