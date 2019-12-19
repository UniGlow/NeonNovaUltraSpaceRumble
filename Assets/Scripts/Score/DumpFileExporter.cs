using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class DumpFileExporter : MonoBehaviour 
{

	#region Variable Declarations
	// Serialized Fields
	[SerializeField] PlayerConfig hero1 = null;
	[SerializeField] PlayerConfig hero2 = null;
	[SerializeField] PlayerConfig hero3 = null;
	[SerializeField] PlayerConfig bossConfig = null;
	[SerializeField] List<float> matchDurations = new List<float>();
	[SerializeField] Points points = null;
	[SerializeField] GameSettings gameSettings = null;
	[SerializeField] VersionNumber versionNumber = null;
	// Private

	#endregion



	#region Public Properties

	#endregion



	#region Unity Event Functions

	#endregion



	#region Public Functions
	public void TriggerExport()
	{
		DumpFileExport.CreateDumpFileEntry(hero1, hero2, hero3, bossConfig, matchDurations, points, gameSettings, versionNumber);
	}
	#endregion



	#region Private Functions

	#endregion



	#region GameEvent Raiser

	#endregion



	#region Coroutines

	#endregion
}


[CustomEditor(typeof(DumpFileExporter))]
class DumpFileExporterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		DumpFileExporter script = (DumpFileExporter)target;
		if (GUILayout.Button("Export"))
		{
			script.TriggerExport();
		}
	}
}