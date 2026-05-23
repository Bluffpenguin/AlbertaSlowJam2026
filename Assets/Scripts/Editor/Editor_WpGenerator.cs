using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointGenerator))]
public class Editor_WpGenerator : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		WaypointGenerator targetScript = (WaypointGenerator)target;
		if (GUILayout.Button("Generate"))
		{
			targetScript.Generate();
		}

		if (GUILayout.Button("Remove Waypoints"))
		{
			targetScript.DeleteWaypoints();
		}
	}
}
