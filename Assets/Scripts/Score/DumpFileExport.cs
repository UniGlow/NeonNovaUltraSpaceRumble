using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class DumpFileExport : MonoBehaviour
{

	#region Variable Declarations
	// Serialized Fields
	public bool manualExport = false;
	[ConditionalHide("manualExport", true)]
	[SerializeField] PlayerConfig hero1Config = null;
	[ConditionalHide("manualExport", true)]
	[SerializeField] PlayerConfig hero2Config = null;
	[ConditionalHide("manualExport", true)]
	[SerializeField] PlayerConfig hero3Config = null;
	[ConditionalHide("manualExport", true)]
	[SerializeField] PlayerConfig bossConfig = null;
	[ConditionalHide("manualExport", true)]
	[SerializeField] List<float> matchDurations = new List<float>();
	[ConditionalHide("manualExport", true)]
	[SerializeField] Points points = null;
	[ConditionalHide("manualExport", true)]
	[SerializeField] GameSettings gameSettings = null;
	[ConditionalHide("manualExport", true)]
	[SerializeField] VersionNumber versionNumber = null;
	// Private
	static string filepath = "Assets/Resources/StatsDump.txt";
    #endregion



    #region Public Properties

    #endregion



    #region Unity Event Functions

    #endregion



    #region Public Functions
    /// <summary>
    /// Adds an Entry into the Dump File with all the Data needed for Balancing the Score System
    /// </summary>
    /// <param name="hero1">PlayerConfig of 1st Hero</param>
    /// <param name="hero2">PlayerConfig of 2nd Hero</param>
    /// <param name="hero3">PlayerConfig of 3rd Hero</param>
    /// <param name="bossConfig">PlayerConfig of Boss Player</param>
    /// <param name="matchDurations">List that contains all Match Durations in Seconds</param>
    /// <param name="points">The Points Scriptable Object</param>
    /// <param name="gameSettings">The GameSettings Scriptable Object</param>
    /// <param name="version">The VersionNumber Scriptable Object</param>
    public void CreateDumpFileEntry(PlayerConfig hero1, PlayerConfig hero2, PlayerConfig hero3, PlayerConfig bossConfig, List<float> matchDurations, Points points, GameSettings gameSettings, VersionNumber version)
	{

		// Declaration and initialization DumpArray with all needed Information
		string[][] dumpArray = new string[62][];
		for (int i = 0; i < dumpArray.Length; i++)
		{
			dumpArray[i] = new string[6];
			for (int j = 0; j < 6; j++)
			{
				dumpArray[i][j] = PutRightLineEnding(j);
			}
		}

		// Each Hero Player Stats
		// Offset between each Player in dumpArray		!!! Change when adding any new Values to Player specific writeouts or things will break !!!
		int offset = 16;
		for (int a = 0; a < 3; a++)
		{
			// Declaring which Hero to Write
			PlayerConfig hero;
			switch (a)
			{
				case 0:
					hero = hero1;
					break;
				case 1:
					hero = hero2;
					break;
				case 2:
					hero = hero3;
					break;
				default:
					hero = null;
					break;
			}
			List<LevelScore> heroScores = hero.HeroScore.LevelScores;

			// First Line: Player Identification, AIControlled and EndScore
			dumpArray[a * offset][0] = "Player;";
			dumpArray[a * offset][1] = hero.ColorConfig.name + ";";
			dumpArray[a * offset][2] = "AIControlled;";
			dumpArray[a * offset][3] = hero.AIControlled + ";";
			dumpArray[a * offset][4] = "EndScore;";
			// Generating End Score
			int endScore = 0;
            List<ScoreCategoryResult> heroEndScores = hero.HeroScore.GetScore();
			foreach (ScoreCategoryResult scoreCategoryResult in heroEndScores)
			{
				endScore += scoreCategoryResult.result;
			}
			dumpArray[a * offset][5] = endScore + "\n";

            // Fill first Column
            dumpArray[a * offset + 1][0] = "DamageDone;";
            dumpArray[a * offset + 2][0] = "DamageDoneScore;";
            dumpArray[a * offset + 3][0] = "CritDamageDone;";
            dumpArray[a * offset + 4][0] = "CritDamageDoneScore;";
            dumpArray[a * offset + 5][0] = "ActiveTime;";
            dumpArray[a * offset + 6][0] = "Shielded;";
            dumpArray[a * offset + 7][0] = "BossTotalPointsDuringActivation;";
            dumpArray[a * offset + 8][0] = "ShieldedPercentage;";
            dumpArray[a * offset + 9][0] = "ShieldedScore;";
            dumpArray[a * offset + 10][0] = "ActiveTime;";
            dumpArray[a * offset + 11][0] = "OrbHeroHits;";
            dumpArray[a * offset + 12][0] = "OrbHeroHitsScore;";
            dumpArray[a * offset + 13][0] = "OrbBossHits;";
            dumpArray[a * offset + 14][0] = "OrbBossHitsScore;";
            dumpArray[a * offset + 15][0] = "ActiveTime;";

			// Collecting all LevelScore Data and write it into the DumpFile
			// Column at which to start (values only, that's why it starts at column 2)
			int i = 1;
			foreach (LevelScore levelScore in heroScores)
			{
                // Collecting all the Data
                // Getting all Score-Dictionaries
                List<ScoreCategoryResult> damageScores = levelScore.DamageScore.GetScore();
                List<ScoreCategoryResult> tankScores = levelScore.TankScore.GetScore();
                List<ScoreCategoryResult> runnerScores = levelScore.RunnerScore.GetScore();

				// Damage values
				int damageDone = levelScore.DamageScore.Damage;
				int critDamageDone = levelScore.DamageScore.CritDamage;
				float activeDamageTime = levelScore.DamageScore.ActiveTime;

				// Tank values
				int damageShielded = levelScore.TankScore.Shielded;
				int bossTotalPointsDuringActivation = levelScore.TankScore.BossTotalPointsDuringActivation;
				float shieldedPercentage = levelScore.TankScore.ShieldedPercentage;
				float activeShieldTime = levelScore.TankScore.ActiveTime;

				// Runner values
				int orbHeroHits = levelScore.RunnerScore.OrbHeroHits;
				int orbBossHits = levelScore.RunnerScore.OrbBossHits;
				float activeRunnerTime = levelScore.RunnerScore.ActiveTime;


			// Putting it all into the Dump Array
				// Damage
				dumpArray[a * offset + 1][i] = damageDone.ToString() + PutRightLineEnding(i);
				dumpArray[a * offset + 2][i] = damageScores.Find(x => x.scoreCategory.name == "DamageDone").result + PutRightLineEnding(i);
				dumpArray[a * offset + 3][i] = critDamageDone + PutRightLineEnding(i);
				dumpArray[a * offset + 4][i] = damageScores.Find(x => x.scoreCategory.name == "CritDamageDone").result + PutRightLineEnding(i);
				dumpArray[a * offset + 5][i] = activeDamageTime + PutRightLineEnding(i);

				// Tank
				dumpArray[a * offset + 6][i] = damageShielded + PutRightLineEnding(i);
				dumpArray[a * offset + 7][i] = bossTotalPointsDuringActivation + PutRightLineEnding(i);
				dumpArray[a * offset + 8][i] = shieldedPercentage + PutRightLineEnding(i);
				dumpArray[a * offset + 9][i] = tankScores.Find(x => x.scoreCategory.name == "DamageShielded").result + PutRightLineEnding(i);
				dumpArray[a * offset + 10][i] = activeShieldTime + PutRightLineEnding(i);

				// Runner
				dumpArray[a * offset + 11][i] = orbHeroHits + PutRightLineEnding(i);
				dumpArray[a * offset + 12][i] = runnerScores.Find(x => x.scoreCategory.name == "OrbHeroHits").result + PutRightLineEnding(i);
				dumpArray[a * offset + 13][i] = orbBossHits + PutRightLineEnding(i);
				dumpArray[a * offset + 14][i] = runnerScores.Find(x => x.scoreCategory.name == "OrbBossHits").result + PutRightLineEnding(i);
				dumpArray[a * offset + 15][i] = activeRunnerTime + PutRightLineEnding(i);

				i++;
			}
		}

		// Boss Stats
		int lineIdentifier = offset * 3;
		// First Line: Player Identification, AIControlled
		dumpArray[lineIdentifier][0] = "Boss;";
		dumpArray[lineIdentifier][2] = "AIControlled;";
		dumpArray[lineIdentifier][3] = bossConfig.AIControlled + ";";
		dumpArray[lineIdentifier][5] = "\n";
		
		lineIdentifier++;

        // First Column
        dumpArray[lineIdentifier][0] = "DamageDone;";
        dumpArray[lineIdentifier + 1][0] = "CritDamageDone;";
        dumpArray[lineIdentifier + 2][0] = "DamageShieled;";

		// Values
        for(int i = 0; i < points.BossDamageInLevels.Count; i++)
        {
            dumpArray[lineIdentifier][i + 1] = points.BossDamageInLevels[i] + PutRightLineEnding(i+1);
            dumpArray[lineIdentifier + 1][i + 1] = points.BossCritDamageInLevels[i] + PutRightLineEnding(i+1);
            dumpArray[lineIdentifier + 2][i + 1] = points.BossDamageShieldedInLevels[i] + PutRightLineEnding(i+1);
        }
        lineIdentifier += 3;

        // LevelStats
        dumpArray[lineIdentifier][0] = "Winner;";
        for(int i = 0; i < points.WinningFactions.Count; i++)
        {
            dumpArray[lineIdentifier][i + 1] = points.WinningFactions[i] + PutRightLineEnding(i+1);
        }
        lineIdentifier++;

        dumpArray[lineIdentifier][0] = "LevelDuration;";
        for(int i = 0; i < matchDurations.Count; i++)
        {
            dumpArray[lineIdentifier][i + 1] = matchDurations[i] + PutRightLineEnding(i+1);
        }
        lineIdentifier++;

        // General Information
        dumpArray[lineIdentifier][0] = "General;";
        lineIdentifier++;
        dumpArray[lineIdentifier][0] = "VersionNumber;";
        dumpArray[lineIdentifier][1] = version.versionNumber + ";";
        lineIdentifier++;

        // Game Settings
        dumpArray[lineIdentifier][0] = "PointsLeadToWin;";
        dumpArray[lineIdentifier][1] = gameSettings.PointLeadToWin + ";";
		lineIdentifier++;

        dumpArray[lineIdentifier][0] = "CritDamageMultiplier;";
        dumpArray[lineIdentifier][1] = gameSettings.CritDamageMultiplier + ";";
		lineIdentifier++;

        dumpArray[lineIdentifier][0] = "IntensifyTime;";
        dumpArray[lineIdentifier][1] = gameSettings.IntensifyTime + ";";
		lineIdentifier++;

        dumpArray[lineIdentifier][0] = "IntensifyAmount;";
        dumpArray[lineIdentifier][1] = gameSettings.IntensifyAmount + ";";
		lineIdentifier++;

        dumpArray[lineIdentifier][0] = "BossColorSwitchInterval;";
        dumpArray[lineIdentifier][1] = gameSettings.BossColorSwitchInterval + ";";
		lineIdentifier++;

        dumpArray[lineIdentifier][0] = "OptimalPointsPerSecond;";
        dumpArray[lineIdentifier][1] = gameSettings.OptimalScorePerSecond + ";";
		lineIdentifier++;

        // Define writer string and fill it with all the Information contained in the DumpArray
        string dumpText = "";
        for(int i = 0; i < dumpArray.Length; i++)
        {
            for(int j = 0; j < dumpArray[i].Length; j++)
            {
                dumpText += dumpArray[i][j];
            }
        }
        // Add End to file for readability
        dumpText += "\n\n";
        
        // Write at Fileend or Create a new one if it doesn't exist
        using (StreamWriter sw = File.AppendText(filepath))
        {
            sw.Write(dumpText);
        }
		Debug.Log(dumpText);
	}

	public void CreateTestDump()
	{
		CreateDumpFileEntry(hero1Config, hero2Config, hero3Config, bossConfig, points.LevelTimes, points, gameSettings, versionNumber);
	}
    #endregion



    #region Private Functions
    /// <summary>
    /// Returns ";" on default or "\n" when i == 5
    /// </summary>
    /// <param name="i">Column</param>
    /// <returns></returns>
    private string PutRightLineEnding(int i)
	{
		if (i == 5)
		{
			return "\n";
		}
		else
		{
			return ";";
		}
	}
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(DumpFileExport))]
class DumpFileExportEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DumpFileExport script = (DumpFileExport)target;
		base.OnInspectorGUI();
		if (script.manualExport)
		{
			if (GUILayout.Button("Export now!"))
			{
				script.CreateTestDump();
			}
		}
	}
}

#endif