using UnityEngine;
using UnityEditor;

namespace MWU.Shared
{
	[CustomEditor(typeof(SceneLoader))]
	public class SceneLoaderEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			SceneLoader config = (SceneLoader)target;

			DrawDefaultInspector();

			EditorGUILayout.Space();

			if (GUILayout.Button("Load Full Workflow", GUILayout.MinHeight(100), GUILayout.Height(50)))
				config.LoadAllScenes();
			
			EditorGUILayout.Space();

			if (GUILayout.Button("Load Set Only", GUILayout.MinHeight(100), GUILayout.Height(50)))
				config.LoadSetScenes(true);
		}
	}
}