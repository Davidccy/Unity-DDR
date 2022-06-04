using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Data", menuName = "TrackData", order = 1)]
public class TrackData : ScriptableObject {
    public AudioClip AudioTrack;
    public AudioClip AudioTrackShort;
    public Sprite Thumbnail;
    public Sprite Wallpaper;
    public string TrackName;
    public int BPM;
    public int ReadyCount;
    public float StartDelay;
    public float FirstMeasure;
    public int BumpPerMeasure;
    public NodeData[] Nodes;
}
