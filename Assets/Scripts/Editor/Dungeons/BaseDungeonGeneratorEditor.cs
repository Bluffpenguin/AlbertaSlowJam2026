[CustomEditor(typeof(BaseDungeonGenerator), editorForChildClasses: true)]
public class BaseDungeonGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var target = this.target as BaseDungeonGenerator;

		if (GUILayout.Button("Generate Dungeon")) {
			target.Generate();
		}

		if (GUILayout.Button("Clear")) {
			target.Clear();
		}
	}
}
