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

    #region Internal Fields
    private readonly int _MEASURE_PER_LINE = 8;
    private readonly float _NOTIFICATION_DURATION = 3.0f;
    private Color _COLOR_SELECTED_MEASURE = Color.green;
    private Color _COLOR_UNSELECTED_MEASURE = Color.white;

    private AudioClip _acTrack = null;
    private AudioClip _acTrackShort = null;
    private Sprite _thumbnail = null;
    private Sprite _wallpaper = null;
    private string _trackName = string.Empty;
    private int _bpm = 0;
    private int _readyCount = 0;
    private float _firstMeasure = 0;
    private int _delayBumpCount = 0;
    private int _bumpPerMeasure = 0;
    private NodeData[] _nodeDataArray;

    private int _selectingMeasure = -1;
    private int _selectedCopyMeasure = -1;
    private TrackManager _trackManager = null;
    private string _editingFilePath = string.Empty;

    private GUIStyle _selectedMeasureStyle = new GUIStyle();
    #endregion

    #region Editor Window Hooks
    public void OnEnable() {
        GameObject goTrackManager = GameObject.Find("TrackManager");
        _trackManager = goTrackManager.GetComponent<TrackManager>();
        if (_trackManager == null) {
            _trackManager = goTrackManager.AddComponent<TrackManager>();
        }

        if (_trackManager == null) {
            Debug.LogErrorFormat("TrackManager not found");
        }

        _selectedMeasureStyle.fontStyle = FontStyle.Bold;
    }

    public void OnGUI() {
        EditorGUILayout.LabelField(string.Format("Editing file path: {0}", 
            string.IsNullOrEmpty(_editingFilePath) ? "None" : _editingFilePath));

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        DrawColorButton("**Create New Track Data**",
            () => {
                CreateNewTrackData();
            },
            Color.white);

        DrawColorButton("**Load Track Data**",
            () => {
                LoadTrackData();
            },
            Color.white);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        Sprite thumbnail = (Sprite) EditorGUILayout.ObjectField("Thumbnail", _thumbnail, typeof(Sprite), false, GUILayout.Width(220));
        if (_thumbnail != thumbnail) {
            _thumbnail = thumbnail;
        }

        EditorGUILayout.Space(50, false);

        Sprite wallpaper = (Sprite) EditorGUILayout.ObjectField("Wallpaper", _wallpaper, typeof(Sprite), false, GUILayout.Width(220));
        if (_wallpaper != wallpaper) {
            _wallpaper = wallpaper;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        AudioClip acTrack = (AudioClip) EditorGUILayout.ObjectField("Select Audio Clip", _acTrack, typeof(AudioClip), false);
        if (_acTrack != acTrack) {
            _acTrack = acTrack;
        }

        AudioClip acTrackShort = (AudioClip) EditorGUILayout.ObjectField("Select Audio Clip Short", _acTrackShort, typeof(AudioClip), false);
        if (_acTrackShort != acTrackShort) {
            _acTrackShort = acTrackShort;
        }

        string trackName = EditorGUILayout.TextField("Track Name", _trackName);
        if (_trackName != trackName) {
            _trackName = trackName;
        }

        int bpm = EditorGUILayout.IntField("BPM", _bpm);
        if (_bpm != bpm) {
            _bpm = bpm;
        }

        int readyCount = EditorGUILayout.IntField("Ready Count", _readyCount);
        if (_readyCount != readyCount) {
            _readyCount = readyCount;
        }

        float firstMeasure = EditorGUILayout.FloatField("First Measure", _firstMeasure);
        if (_firstMeasure != firstMeasure) {
            _firstMeasure = firstMeasure;
        }

        int delayBumpCount = EditorGUILayout.IntField("Delay Bump Count", _delayBumpCount);
        if (_delayBumpCount != delayBumpCount) {
            _delayBumpCount = delayBumpCount;
        }

        int bumpPerMeasure = EditorGUILayout.IntField("Bump Per Measure", _bumpPerMeasure);
        if (_bumpPerMeasure != bumpPerMeasure) {
            _bumpPerMeasure = bumpPerMeasure;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Measures:");
        EditorGUILayout.BeginHorizontal();
        DrawColorButton("Create Measure", 
            () => {
                CreateMeasure();
                _selectedCopyMeasure = -1;
            }, 
            Color.white, 150);

        if (_selectingMeasure != -1 && _selectingMeasure == _nodeDataArray.Length - 1) {
            DrawColorButton("Remove Measure",
                () => {
                    RemoveMeasure();
                    _selectedCopyMeasure = -1;
                },
                Color.white, 150);
        }        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (_selectingMeasure != -1) {
            DrawColorButton("Copy Measure",
                () => {
                    _selectedCopyMeasure = _selectingMeasure;
                },
                Color.white, 150);
        }

        if (_selectedCopyMeasure != -1 && _selectingMeasure != _selectedCopyMeasure) {
            DrawColorButton("Paste Copied Measure",
                () => {
                    CopyMeasure(_selectedCopyMeasure, _selectingMeasure);
                    _selectedCopyMeasure = -1;
                },
                Color.white, 150);
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

                Color c = _selectingMeasure == measure ? _COLOR_SELECTED_MEASURE : _COLOR_UNSELECTED_MEASURE;
                DrawColorButton(measure.ToString(),
                    () => {
                        _selectingMeasure = measure;
                    },
                    c, 50);

            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (_selectingMeasure != -1) {
            EditorGUILayout.LabelField(string.Format("Now Selecting Measure: {0}", _selectingMeasure));
            DrawColorButton("Create New Node",
                () => {
                    CreateNodeInfo(_selectingMeasure);
                },
                Color.white, 150);
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
                DrawColorButton("^",
                    () => {
                        if (i != 0) {
                            SwapNodeInfo(nodeInfoArray, i, i - 1);
                        }
                    },
                    Color.white, 20);

                DrawColorButton("v",
                    () => {
                        if (i != nodeInfoArray.Length - 1) {
                            SwapNodeInfo(nodeInfoArray, i, i + 1);
                        }
                    },
                    Color.white, 20);
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

                DrawColorButton("Remove",
                    () => {
                        RemoveNodeInfo(_selectingMeasure, i);
                    },
                    Color.red, 60);

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawColorButton("**Save Track Data**",
            () => {
                SaveTrackData();
            },
            Color.yellow);

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawColorButton("Play !!",
           () => {
               PlayTrack();
           },
           Color.white);
    }
    #endregion

    #region Internal Methods
    private void DrawColorButton(string text, Action cb, Color color, float width = -1) {
        Color oriColor = GUI.color;
        GUI.backgroundColor = color;

        if (width == -1) {
            if (GUILayout.Button(text)) {
                if (cb != null) {
                    cb();
                }
            }
        }
        else {
            if (GUILayout.Button(text, GUILayout.Width(width))) {
                if (cb != null) {
                    cb();
                }
            }
        }

        GUI.color = oriColor;
    }

    private void CreateNewTrackData() {
        // NOTE:
        // Ex:  path = D:/UnityProjects/Unity-DDR/DDR/Assets/Resources/Data/TrackData/TrackData_001.asset
        //      Application.dataPath = D:/UnityProjects/Unity-DDR/DDR/Assets

        string path = EditorUtility.SaveFilePanel("Create New Track Data", Define.EDITOR_ASSET_PATH, Define.EDITOR_ASSET_DEFAULT_NAME, "asset");
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        path = path.Replace(Application.dataPath, "Assets");

        TrackData tData = ScriptableObject.CreateInstance<TrackData>();
        AssetDatabase.CreateAsset(tData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        _editingFilePath = path;

        ImportTrackData(tData);

        ShowNotification(new GUIContent(string.Format("New file '{0}' created", _editingFilePath)), _NOTIFICATION_DURATION);
    }

    private void LoadTrackData() {
        // NOTE:
        // Ex:  path = D:/UnityProjects/Unity-DDR/DDR/Assets/Resources/Data/TrackData/TrackData_001.asset
        //      Application.dataPath = D:/UnityProjects/Unity-DDR/DDR/Assets

        string path = EditorUtility.OpenFilePanel("Load Track Data", Define.EDITOR_ASSET_PATH, "asset");
        if (string.IsNullOrEmpty(path)) {
            return;
        }

        path = path.Replace(Application.dataPath, "Assets");
        TrackData tData = AssetDatabase.LoadAssetAtPath<TrackData>(path);
        ImportTrackData(tData);

        _editingFilePath = path;

        ShowNotification(new GUIContent(string.Format("File '{0}' loaded", _editingFilePath)), _NOTIFICATION_DURATION);
    }

    private void SaveTrackData() {
        if (string.IsNullOrEmpty(_editingFilePath)) {
            ShowNotification(new GUIContent("No file editing, try create new file first"), _NOTIFICATION_DURATION);
            return;
        }

        if (EditorUtility.DisplayDialog("Confirm", "Are you sure to save this track data ?", "OK", "Cancel")) {
            TrackData tData = ExportTrackData();
            AssetDatabase.CreateAsset(tData, _editingFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ShowNotification(new GUIContent(string.Format("File '{0}' saved", _editingFilePath)), _NOTIFICATION_DURATION);
        }        
    }

    private void ImportTrackData(TrackData tData) {
        if (tData == null) {
            Debug.LogErrorFormat("Null track data");
            return;
        }

        _acTrack = tData.AudioTrack;
        _acTrackShort = tData.AudioTrackShort;
        _thumbnail = tData.Thumbnail;
        _wallpaper = tData.Wallpaper;
        _trackName = tData.TrackName;
        _bpm = tData.BPM;
        _readyCount = tData.ReadyCount;
        _firstMeasure = tData.FirstMeasure;
        _delayBumpCount = tData.DelayBumpCount;
        _bumpPerMeasure = tData.BumpPerMeasure;
        _nodeDataArray = tData.Nodes;

        _selectingMeasure = -1;
    }

    private TrackData ExportTrackData() {
        TrackData tData = ScriptableObject.CreateInstance<TrackData>();
        tData.AudioTrack = _acTrack;
        tData.AudioTrackShort = _acTrackShort;
        tData.Thumbnail = _thumbnail;
        tData.Wallpaper = _wallpaper;
        tData.TrackName = _trackName;
        tData.BPM = _bpm;
        tData.ReadyCount = _readyCount;
        tData.FirstMeasure = _firstMeasure;
        tData.DelayBumpCount = _delayBumpCount;
        tData.BumpPerMeasure = _bumpPerMeasure;
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

    private void PlayTrack() {
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
