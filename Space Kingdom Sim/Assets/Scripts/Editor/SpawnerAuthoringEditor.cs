using UnityEditor;

[CustomEditor(typeof(BoundsData))]
public class PositionFactoryDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var thisObject = target as BoundsData;

        DisplayCommonSettings();

        switch (thisObject.boundsType)
        {
            case BoundsEnum.Square: DisplaySquareSettings(); break;
            case BoundsEnum.Circle: DisplayCircleSettings(); break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayCommonSettings()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("boundsType"));
    }

    private void DisplaySquareSettings()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bounds"));
    }

    private void DisplayCircleSettings()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRadius"));
    }
}