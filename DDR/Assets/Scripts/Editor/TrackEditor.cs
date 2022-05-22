using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class TrackEditor : EditorWindow {
    [MenuItem("PnP/TrackEditor")]
    public static void OpenWindow() {
        EditorSceneManager.OpenScene(Define.EDITOR_SCENE_PATH);

        var window = GetWindow<TrackEditor>();
        window.Show();
    }

    private AudioClip _acTrack = null;
    private string _trackName = string.Empty;
    private int _bgm = 0;
    private int _readyCount = 0;
    private float _startDelay = 0;
    private float _firstMeasure = 0;
    private NodeData[] _nodeDataArray;

    private readonly int _MEASURE_PER_LINE = 8;

    private int _selectingMeasure = -1;
    private int _selectedCopyMeasure = -1;
    private TrackManager _trackManager = null;

    public void OnEnable() {
        GameObject goTrackManager = GameObject.Find("TrackManager");
        _trackManager = goTrackManager.GetComponent<TrackManager>();
        if (_trackManager == null) {
            _trackManager = goTrackManager.AddComponent<TrackManager>();
        }

        if (_trackManager == null) {
            Debug.LogErrorFormat("TrackManager not found");
        }
    }

    public void OnGUI() {
        AudioClip acTrack = (AudioClip) EditorGUILayout.ObjectField("Select Audio Clip", _acTrack, typeof(AudioClip), false);
        if (acTrack != _acTrack) {
            _acTrack = acTrack;
        }

        string trackName = EditorGUILayout.TextField("Track Name", _trackName);
        if (trackName != _trackName) {
            _trackName = trackName;
        }

        int bgm = EditorGUILayout.IntField("BGM", _bgm);
        if (bgm != _bgm) {
            _bgm = bgm;
        }

        int readyCount = EditorGUILayout.IntField("Ready Count", _readyCount);
        if (readyCount != _readyCount) {
            _readyCount = readyCount;
        }

        float startDelay = EditorGUILayout.FloatField("Start Delay", _startDelay);
        if (startDelay != _startDelay) {
            _startDelay = startDelay;
        }

        float firstMeasure = EditorGUILayout.FloatField("First Measure", _firstMeasure);
        if (firstMeasure != _firstMeasure) {
            _firstMeasure = firstMeasure;
        }

        if (GUILayout.Button("**Load Track Data**")) {
            LoadTrackData();            
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Measures:");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Measure", GUILayout.Width(150))) {
            CreateMeasure();
            _selectedCopyMeasure = -1;
        }

        if (_selectingMeasure != -1 && _selectingMeasure == _nodeDataArray.Length - 1) {
            if (GUILayout.Button("Remove Measure", GUILayout.Width(150))) {
                RemoveMeasure();
                _selectedCopyMeasure = -1;
            }
        }        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (_selectingMeasure != -1) {
            if (GUILayout.Button("Copy Measure", GUILayout.Width(150))) {
                _selectedCopyMeasure = _selectingMeasure;
            }
        }

        if (_selectedCopyMeasure != -1 && _selectingMeasure != _selectedCopyMeasure) {
            if (GUILayout.Button("Paste Copied Measure", GUILayout.Width(150))) {
                CopyMeasure(_selectedCopyMeasure, _selectingMeasure);
                _selectedCopyMeasure = -1;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (_nodeDataArray != null) {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < _nodeDataArray.Length; i++) {
                int measure = _nodeDataArray[i].Measure;
                if (measure != 0 && measure % _MEASURE_PER_LINE == 1) {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                if (GUILayout.Button(measure.ToString(), GUILayout.Width(50))) {
                    _selectingMeasure = measure;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (_selectingMeasure != -1) {
            EditorGUILayout.LabelField(string.Format("Now Selecting Measure: {0}", _selectingMeasure));
            if (GUILayout.Button("Create New Node", GUILayout.Width(150))) {
                CreateNodeInfo(_selectingMeasure);
            }
            EditorGUILayout.Space(20, false);

            EditorGUILayout.LabelField("Nodes in Measures:");

            string[] nTypeNames = Enum.GetNames(typeof(NodeType));
            Array nTypeValues = Enum.GetValues(typeof(NodeType));

            string[] nPositionNames = Enum.GetNames(typeof(NodePosition));
            Array nPositionValues = Enum.GetValues(typeof(NodePosition));

            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            NodeInfo[] nodeInfoArray = _nodeDataArray[_selectingMeasure].NodeInfoList;
            if (nodeInfoArray == null) {
                _nodeDataArray[_selectingMeasure].NodeInfoList = new NodeInfo[0];
                nodeInfoArray = _nodeDataArray[_selectingMeasure].NodeInfoList;
            }

            for (int i = 0; i < nodeInfoArray.Length; i++) {
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 50;

                // Order changing
                if (GUILayout.Button("^", GUILayout.Width(20))) {
                    if (i != 0) {
                        SwapNodeInfo(nodeInfoArray, i, i - 1);
                    }
                }

                if (GUILayout.Button("v", GUILayout.Width(20))) {
                    if (i != nodeInfoArray.Length - 1) {
                        SwapNodeInfo(nodeInfoArray, i, i + 1);
                    }
                }
                EditorGUILayout.Space(20, false);

                NodeInfo nInfo = nodeInfoArray[i];

                // Timing
                float timing = EditorGUILayout.FloatField("Timing", nInfo.Timing, GUILayout.Width(120));
                if (timing != nInfo.Timing) {
                    nInfo.Timing = timing;
                }
                EditorGUILayout.Space(20, false);

                // Timing2
                float timing2 = EditorGUILayout.FloatField("Timing2", nInfo.Timing2, GUILayout.Width(120));
                if (timing2 != nInfo.Timing2) {
                    nInfo.Timing2 = timing2;
                }
                EditorGUILayout.Space(20, false);

                // Node type
                EditorGUILayout.LabelField("Node Type", GUILayout.Width(70));
                NodeType nType = nInfo.NodeType;
                if (EditorGUILayout.DropdownButton(new GUIContent(nType.ToString()), FocusType.Keyboard, GUILayout.Width(80))) {
                    GenericMenu menu = new GenericMenu();
                    for (int j = 0; j < nTypeNames.Length; j++) {
                        string name = nTypeNames[j];
                        menu.AddItem(new GUIContent(nTypeNames[j]), name == nType.ToString(), (v) => OnNodeTypeSelected(v, nInfo), (int) nTypeValues.GetValue(j));
                    }

                    menu.ShowAsContext();
                }
                EditorGUILayout.Space(20, false);

                // Node position
                EditorGUILayout.LabelField("Node Position", GUILayout.Width(90));
                NodePosition nPos = nInfo.NodePosition;
                if (EditorGUILayout.DropdownButton(new GUIContent(nPos.ToString()), FocusType.Keyboard, GUILayout.Width(80))) {
                    GenericMenu menu = new GenericMenu();
                    for (int j = 0; j < nPositionNames.Length; j++) {
                        string name = nPositionNames[j];
                        menu.AddItem(new GUIContent(nPositionNames[j]), name == nPos.ToString(), (v) => OnNodePositionSelected(v, nInfo), (int) nPositionValues.GetValue(j));
                    }

                    menu.ShowAsContext();
                }
                EditorGUILayout.Space(20, false);

                if (GUILayout.Button("Remove", GUILayout.Width(60))) {
                    RemoveNodeInfo(_selectingMeasure, i);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("**Save Track Data**")) {
            SaveTrackData();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Play !!")) {
            PlayerAudio();
        }
    }

    #region Internal Methods
    private void LoadTrackData() {
        // NOTE:
        // Ex:  path = D:/UnityProjects/Unity-DDR/DDR/Assets/Resources/Data/TrackData/TrackData_001.asset
        //      Application.dataPath = D:/UnityProjects/Unity-DDR/DDR/Assets
        //

        string path = EditorUtility.OpenFilePanel("Load Track Data", Define.EDITOR_ASSET_PATH, "asset");
        path = path.Replace(Application.dataPath, "Assets");
        TrackData tData = AssetDatabase.LoadAssetAtPath<TrackData>(path);
        ImportTrackData(tData);
    }

    private void SaveTrackData() {
        // NOTE:
        // Ex:  path = D:/UnityProjects/Unity-DDR/DDR/Assets/Resources/Data/TrackData/TrackData_001.asset
        //      Application.dataPath = D:/UnityProjects/Unity-DDR/DDR/Assets
        //

        string path = EditorUtility.SaveFilePanel("Save Track Data", Define.EDITOR_ASSET_PATH, Define.EDITOR_ASSET_DEFAULT_NAME, "asset");
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        path = path.Replace(Application.dataPath, "Assets");

        TrackData tData = ExportTrackData();
        AssetDatabase.CreateAsset(tData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ImportTrackData(TrackData tData) {
        if (tData == null) {
            Debug.LogErrorFormat("Null track data");
            return;
        }

        _acTrack = tData.AudioTrack;
        _trackName = tData.TrackName;
        _bgm = tData.BPM;
        _readyCount = tData.ReadyCount;
        _startDelay = tData.StartDelay;
        _firstMeasure = tData.FirstMeasure;
        _nodeDataArray = tData.Nodes;

        _selectingMeasure = -1;
    }

    private TrackData ExportTrackData() {
        TrackData tData = ScriptableObject.CreateInstance<TrackData>();
        tData.AudioTrack = _acTrack;
        tData.TrackName = _trackName;
        tData.BPM = _bgm;
        tData.ReadyCount = _readyCount;
        tData.StartDelay = _startDelay;
        tData.FirstMeasure = _firstMeasure;
        tData.Nodes = _nodeDataArray;

        return tData;
    }

    private void CreateMeasure() {
        // NOTE:
        // Add from measure value = 0

        int curMeasureCount = _nodeDataArray != null ? _nodeDataArray.Length : 0;
        int newMeasureCount = curMeasureCount + 1;

        // Copy
        NodeData[] newNodeDataList = new NodeData[newMeasureCount];
        for (int i = 0; i < curMeasureCount; i++) {
            newNodeDataList[i] = _nodeDataArray[i];
        }

        // Add new measure
        NodeData newNodeData = new NodeData();
        newNodeData.Measure = newMeasureCount - 1;
        newNodeData.NodeInfoList = new NodeInfo[0];
        newNodeDataList[newMeasureCount - 1] = newNodeData;

        _nodeDataArray = newNodeDataList;
    }

    private void RemoveMeasure() {
        // NOTE:
        // Currently, only final measure can be removed

        int curMeasureCount = _nodeDataArray.Length;
        int newMeasureCount = curMeasureCount - 1;

        NodeData[] newNodeDataList = new NodeData[newMeasureCount];
        for (int i = 0; i < newMeasureCount; i++) {
            ;
            newNodeDataList[i] = _nodeDataArray[i];
        }

        _nodeDataArray = newNodeDataList;
        _selectingMeasure = -1;
    }

    private void CopyMeasure(int fromMeasure, int toMeasure) {
        if (fromMeasure < 0 || fromMeasure >= _nodeDataArray.Length) {
            Debug.LogErrorFormat("Invalid 'From measure' {0} to copy content", fromMeasure);
            return;
        }

        if (toMeasure < 0 || toMeasure >= _nodeDataArray.Length) {
            Debug.LogErrorFormat("Invalid 'From measure' {0} to copy content", toMeasure);
            return;
        }

        NodeInfo[] copiedContent = _nodeDataArray[fromMeasure].NodeInfoList;
        NodeInfo[] newContent = new NodeInfo[copiedContent.Length];
        for (int i = 0; i < copiedContent.Length; i++) {
            newContent[i] = new NodeInfo();
            newContent[i].Timing = copiedContent[i].Timing;
            newContent[i].Timing2 = copiedContent[i].Timing2;
            newContent[i].NodeType = copiedContent[i].NodeType;
            newContent[i].NodePosition = copiedContent[i].NodePosition;
        }
        _nodeDataArray[toMeasure].NodeInfoList = newContent;
    }

    private void CreateNodeInfo(int measure) {
        NodeInfo[] oldNodeInfoList = _nodeDataArray[measure].NodeInfoList;
        int oldCount = oldNodeInfoList != null ? oldNodeInfoList.Length : 0;
        int newCount = oldCount + 1;

        NodeInfo[] newNodeInfoList = new NodeInfo[newCount];
        for (int i = 0; i < oldCount; i++) {
            newNodeInfoList[i] = oldNodeInfoList[i];
        }
        newNodeInfoList[newCount - 1] = new NodeInfo();

        _nodeDataArray[measure].NodeInfoList = newNodeInfoList;
    }

    private void RemoveNodeInfo(int measure, int nodeInfoIdx) {
        NodeInfo[] oldNodeInfoList = _nodeDataArray[measure].NodeInfoList;
        int oldCount = oldNodeInfoList.Length;
        int newCount = oldCount - 1;

        NodeInfo[] newNodeInfoList = new NodeInfo[newCount];
        int newNodeInfoIdx = 0;
        for (int i = 0; i < oldCount; i++) {
            if (i == nodeInfoIdx) {
                continue;
            }

            newNodeInfoList[newNodeInfoIdx] = oldNodeInfoList[i];
            newNodeInfoIdx += 1;
        }

        _nodeDataArray[measure].NodeInfoList = newNodeInfoList;
    }

    private void SwapNodeInfo(NodeInfo[] nodeInfoArray, int idxA, int idxB) {
        if (nodeInfoArray == null) {
            Debug.LogErrorFormat("Null node info array to swap");
            return;
        }

        if (idxA < 0 || idxA >= nodeInfoArray.Length) {
            Debug.LogErrorFormat("Invalid index A '{0}' to swap", idxA);
            return;
        }

        if (idxB < 0 || idxB >= nodeInfoArray.Length) {
            Debug.LogErrorFormat("Invalid index B '{0}' to swap", idxB);
            return;
        }

        NodeInfo oldNodeInfoA = nodeInfoArray[idxA];
        NodeInfo oldNodeInfoB = nodeInfoArray[idxB];

        nodeInfoArray[idxA] = oldNodeInfoB;
        nodeInfoArray[idxB] = oldNodeInfoA;
    }

    private void PlayerAudio() {
        if (_acTrack == null) {
            return;
        }

        TrackData tData = ExportTrackData();

        TrackManager.Instance.LoadTrackDataEditor(tData);
    }
    
    private void OnNodeTypeSelected(object value, NodeInfo nodeInfo) {
        NodeType nType = (NodeType) value;
        nodeInfo.NodeType = nType;
    }

    private void OnNodePositionSelected(object value, NodeInfo nodeInfo) {
        NodePosition nPos = (NodePosition) value;
        nodeInfo.NodePosition = nPos;
    }
    #endregion
}
