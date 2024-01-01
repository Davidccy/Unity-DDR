using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(TrackData), true)]
[CanEditMultipleObjects]
public class TrackDataEditor : Editor {
    private SerializedProperty _audioTrack = null;
    private SerializedProperty _trackName = null;
    private SerializedProperty _bpm = null;    
    private SerializedProperty _readyCount = null;
    private SerializedProperty _startDelay = null;
    private SerializedProperty _firstMeasure = null;
    private SerializedProperty _bumpPerMeasure = null;
    private SerializedProperty _nodes = null;

    private ReorderableList _nodeList = null;

    private void OnEnable() {
        _audioTrack = serializedObject.FindProperty("AudioTrack");
        _trackName = serializedObject.FindProperty("TrackName");
        _bpm = serializedObject.FindProperty("BPM");
        _readyCount = serializedObject.FindProperty("ReadyCount");
        _startDelay = serializedObject.FindProperty("StartDelay");
        _firstMeasure = serializedObject.FindProperty("FirstMeasure");
        _bumpPerMeasure = serializedObject.FindProperty("BumpPerMeasure");
        _nodes = serializedObject.FindProperty("Nodes");

        _nodeList = new ReorderableList(serializedObject, _nodes, true, true, true, true);
        _nodeList.drawHeaderCallback = DrawHeader;
        _nodeList.drawElementCallback = DrawListItems;
        _nodeList.elementHeightCallback = ElementHeight;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        //base.OnInspectorGUI();
        EditorGUILayout.PropertyField(_audioTrack);
        EditorGUILayout.PropertyField(_trackName);
        EditorGUILayout.PropertyField(_bpm);
        EditorGUILayout.PropertyField(_readyCount);
        //EditorGUILayout.PropertyField(_startDelay);
        EditorGUILayout.PropertyField(_firstMeasure);
        EditorGUILayout.PropertyField(_bumpPerMeasure);
        _nodeList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(Rect rect) {
        EditorGUI.LabelField(rect, "Note List");
    }

    private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) {
        SerializedProperty element = _nodeList.serializedProperty.GetArrayElementAtIndex(index);
        
        // Mesaure
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), "Measure");
        EditorGUI.PropertyField(
            new Rect(rect.x + 60, rect.y, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("Measure"),
            GUIContent.none
        );


        //EditorGUI.LabelField(new Rect(rect.x + 120, rect.y, 80, EditorGUIUtility.singleLineHeight), "TestArray");
        //EditorGUI.PropertyField(
        //    new Rect(rect.x + 200, rect.y, 30, EditorGUIUtility.singleLineHeight),
        //    element.FindPropertyRelative("TestArray"),
        //    GUIContent.none
        //);

        // NodeList
        SerializedProperty TestArray = element.FindPropertyRelative("NodeInfoList");
        EditorGUI.PropertyField(
            new Rect(rect.x + 120, rect.y, rect.width - 120, EditorGUI.GetPropertyHeight(TestArray, true)),
            TestArray, true);
    }

    private float ElementHeight(int index) {
        SerializedProperty element = _nodeList.serializedProperty.GetArrayElementAtIndex(index);
        float height = EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty testArray = element.FindPropertyRelative("NodeInfoList");
        height += EditorGUI.GetPropertyHeight(testArray, true) + EditorGUIUtility.standardVerticalSpacing;

        return height + EditorGUIUtility.standardVerticalSpacing;
    }
}
