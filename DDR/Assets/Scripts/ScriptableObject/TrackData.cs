using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Data", menuName = "TrackData", order = 1)]
public class TrackData : ScriptableObject {
    public string TrackName;
    public int BPM;
    public int ReadyCount;
    public float StartDelay;
    public float FirstMeasure;
    public NodeData[] Nodes;
}
