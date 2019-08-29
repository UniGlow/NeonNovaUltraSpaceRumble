#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace MWU.Shared
{

    [CreateAssetMenu(fileName = "SceneLoader", menuName = "Scriptable Objects/Scene Loader", order = 2)]
    public class SceneLoader : ScriptableObject
    {
		[Header("Active Scene")]
		[SerializeField] Object activeScene = null;

		[Header("Set Scenes")]
		[SerializeField] Object[] setScenes = null;



		public void LoadAllScenes()
		{
			if (activeScene == null)
			{
				Debug.LogError("No Active Scene has been specified. Loading failed.");
				return;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

			EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(activeScene), OpenSceneMode.Single);
            
			LoadSetScenes(false);
		}

		public void LoadSetScenes(bool unloadOtherScenes)
		{
			if (setScenes.Length == 0)
			{
				if (unloadOtherScenes)
					Debug.LogError("No Set Scene has been specified. Loading failed.");

				return;
			}

			OpenSceneMode mode = OpenSceneMode.Additive;

			if (unloadOtherScenes)
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				mode = OpenSceneMode.Single;
			}

			EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(setScenes[0]), mode);

			for (int i = 1; i < setScenes.Length; i++)
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(setScenes[i]), OpenSceneMode.Additive);

            // Implement this line as soon as you understand how to cast an Object to a Scene asset
            //EditorSceneManager.MoveSceneAfter(activeScene, setScenes[setScenes.Length - 1]);
        }
	}
}
#endif