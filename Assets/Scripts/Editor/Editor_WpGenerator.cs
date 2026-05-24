using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingManager))]
public class Editor_WpGenerator : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		PathfindingManager targetScript = (PathfindingManager)target;
		
	}
}
