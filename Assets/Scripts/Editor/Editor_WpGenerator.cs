using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingManager))]
public class Editor_WpGenerator : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		PathfindingManager targetScript = (PathfindingManager)target;
		if (GUILayout.Button("Test_Generate"))
		{
			targetScript.Test_Generate();
		}

		if (GUILayout.Button("Remove Waypoints"))
		{
			targetScript.Test_Delete();
		}
	}
}
