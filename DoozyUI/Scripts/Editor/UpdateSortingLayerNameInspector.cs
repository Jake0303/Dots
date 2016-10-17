using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpdateSortingLayerName), true)]
public class UpdateSortingLayerNameInspector : Editor
{
    UpdateSortingLayerName updateSortingLayerName;

    void OnEnable()
    {
        updateSortingLayerName = (UpdateSortingLayerName)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(8);

        if (GUILayout.Button("Update Canvases"))
        {
            updateSortingLayerName.UpdateCanvases();
        }

        EditorGUILayout.HelpBox("This updates the all the children's canvases sorting layer name to the new sorting layer name", MessageType.Info);

        GUILayout.Space(8);

        if (GUILayout.Button("Update Renderers"))
        {
            updateSortingLayerName.UpdateCanvases();
        }

        EditorGUILayout.HelpBox("This updates the all the children's renderers sorting layer name to the new sorting layer name", MessageType.Info);
    }

}
