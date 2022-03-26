using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Select Data", menuName = "SelectData", order = 1)]
public class SelectData : ScriptableObject {
    public SelectInfo[] SelectInfos;
}
