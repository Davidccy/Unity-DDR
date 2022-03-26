using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(SelectData), true)]
[CanEditMultipleObjects]
public class SelectDataEditor : Editor {
    private SerializedProperty _selectInfos = null;

    private ReorderableList _selectInfoList = null;

    private void OnEnable() {
        _selectInfos = serializedObject.FindProperty("SelectInfos");
        _selectInfoList = new ReorderableList(serializedObject, _selectInfos, true, true, true, true);
        _selectInfoList.drawHeaderCallback = DrawHeader;
        _selectInfoList.drawElementCallback = DrawListItems;
        _selectInfoList.elementHeightCallback = ElementHeight;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        _selectInfoList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(Rect rect) {
        EditorGUI.LabelField(rect, "SelectInfo");
    }

    private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) {
        SerializedProperty element = _selectInfoList.serializedProperty.GetArrayElementAtIndex(index);

        // Order
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 80, EditorGUIUtility.singleLineHeight), string.Format("Index = {0}", index + 1));

        // TrackID
        EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 80, EditorGUIUtility.singleLineHeight), "TrackID");
        EditorGUI.PropertyField(
            new Rect(rect.x + 80, rect.y + 20, 80, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("TrackID"),
            GUIContent.none
        );

        // TrackName
        EditorGUI.LabelField(new Rect(rect.x, rect.y + 40, 80, EditorGUIUtility.singleLineHeight), "TrackName");
        EditorGUI.PropertyField(
            new Rect(rect.x + 80, rect.y + 40, 150, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("TrackName"),
            GUIContent.none
        );

        // Thumbnail
        EditorGUI.LabelField(new Rect(rect.x, rect.y + 60, 80, EditorGUIUtility.singleLineHeight), "Thumbnail");
        EditorGUI.PropertyField(
            new Rect(rect.x + 80, rect.y + 60, 150, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("Thumbnail"),
            GUIContent.none
        );

        // BPM
        EditorGUI.LabelField(new Rect(rect.x, rect.y + 80, 80, EditorGUIUtility.singleLineHeight), "BPM");
        EditorGUI.PropertyField(
            new Rect(rect.x + 80, rect.y + 80, 80, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("BPM"),
            GUIContent.none
        );
    }

    private float ElementHeight(int index) {
        SerializedProperty element = _selectInfoList.serializedProperty.GetArrayElementAtIndex(index);
        float height = EditorGUIUtility.standardVerticalSpacing;

        height += EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;

        return height + 80;
    }
}
