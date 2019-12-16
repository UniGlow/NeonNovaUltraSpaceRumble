using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public static class DumpFileExport 
{

	#region Variable Declarations
	// Serialized Fields

	// Private
	static string filepath = "Assets/Resources/StatsDump.txt";
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions

	#endregion
	
	
	
	#region Public Functions
	public static void CreateDumpFileEntry(PlayerConfig hero1, PlayerConfig hero2, PlayerConfig hero3, PlayerConfig bossConfig, float[] matchTimes, Points points, GameSettings gameSettings)
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
		// Offset between each Player in dumpArray		!!! Change when adding any new Values to Player Specific writeouts or things will break !!!
		int offset = 16;
		for (int x = 0; x < 3; x++)
		{
			// Declaring which Hero to Write
			PlayerConfig hero;
			switch (x)
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
			dumpArray[x * offset][0] = "Player;";
			dumpArray[x * offset][1] = hero.ColorConfig.name + ";";
			dumpArray[x * offset][2] = "AIControlled;";
			dumpArray[x * offset][3] = hero.AIControlled + ";";
			dumpArray[x * offset][4] = "EndScore;";
			// Generating End Score
			int endScore = 0;
			Dictionary<ScoreCategory, int> heroEndScores = hero.HeroScore.GetScore();
			foreach (KeyValuePair<ScoreCategory, int> valuePair in heroEndScores)
			{
				endScore += valuePair.Value;
			}
			dumpArray[x * offset][3] = endScore + "\n";
			
			// TODO: Write first Column here									!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


			// Collecting all LevelScore Data and write it into the DumpFile
			// Column at which to start (values only, that's why it starts at column 2)
			int i = 1;
			foreach (LevelScore levelScore in heroScores)
			{
			// Collecting all the Data
				// Getting all Score-Dictionaries
				Dictionary<ScoreCategory, int> damageScore = levelScore.DamageScore.GetScore();
				Dictionary<ScoreCategory, int> tankScore = levelScore.TankScore.GetScore();
				Dictionary<ScoreCategory, int> runnerScore = levelScore.RunnerScore.GetScore();

				// Damage values
				int damageDone = levelScore.DamageScore.Damage;
				int damageDoneScore = 0;
				int critDamageDone = levelScore.DamageScore.CritDamage;
				int critDamageDoneScore = 0;
				float activeDamageTime = levelScore.DamageScore.ActiveTime;
					// Get Score Values from Dictionary
				damageScore.TryGetValue(gameSettings.DamageScoreCategories.Find(x => x.name == "DamageDone"), out damageDone);
				damageScore.TryGetValue(gameSettings.DamageScoreCategories.Find(x => x.name == "CritDamageDone"), out critDamageDone);

				// Tank values
				int damageShielded = levelScore.TankScore.Shielded;
				int bossTotalPointsDuringActivation = levelScore.TankScore.BossTotalPointsDuringActivation;
				float shieldedPercentage = levelScore.TankScore.ShieldedPercentage;
				int shieldedScore = 0;
				float activeShieldTime = levelScore.TankScore.ActiveTime;
					// Get Score Values from Dictionary
				tankScore.TryGetValue(gameSettings.TankScoreCategories.Find(x => x.name == "DamageShielded"), out shieldedScore);

				// Runner values
				int orbHeroHits = levelScore.RunnerScore.OrbHeroHits;
				int orbHeroHitsScore = 0;
				int orbBossHits = levelScore.RunnerScore.OrbBossHits;
				int orbBossHitsScore = 0;
				float activeRunnerTime = levelScore.RunnerScore.ActiveTime;
					// Get Score Values from Dictionary
				runnerScore.TryGetValue(gameSettings.RunnerScoreCategories.Find(x => x.name == "OrbHeroHits"), out orbHeroHitsScore);
				runnerScore.TryGetValue(gameSettings.RunnerScoreCategories.Find(x => x.name == "OrbBossHits"), out orbBossHitsScore);


			// Putting it all into the Dump Array
				// Damage
				dumpArray[x * offset + 1][i] = damageDone + PutRightLineEnding(i);
				dumpArray[x * offset + 2][i] = damageDoneScore + PutRightLineEnding(i);
				dumpArray[x * offset + 3][i] = critDamageDone + PutRightLineEnding(i);
				dumpArray[x * offset + 4][i] = critDamageDoneScore + PutRightLineEnding(i);
				dumpArray[x * offset + 5][i] = activeDamageTime + PutRightLineEnding(i);

				// Tank
				dumpArray[x * offset + 6][i] = damageShielded + PutRightLineEnding(i);
				dumpArray[x * offset + 7][i] = bossTotalPointsDuringActivation + PutRightLineEnding(i);
				dumpArray[x * offset + 8][i] = shieldedPercentage + PutRightLineEnding(i);
				dumpArray[x * offset + 9][i] = shieldedScore + PutRightLineEnding(i);
				dumpArray[x * offset + 10][i] = activeShieldTime + PutRightLineEnding(i);

				// Runner
				dumpArray[x * offset + 11][i] = orbHeroHits + PutRightLineEnding(i);
				dumpArray[x * offset + 12][i] = orbHeroHitsScore + PutRightLineEnding(i);
				dumpArray[x * offset + 13][i] = orbBossHits + PutRightLineEnding(i);
				dumpArray[x * offset + 14][i] = orbBossHitsScore + PutRightLineEnding(i);
				dumpArray[x * offset + 15][i] = activeRunnerTime + PutRightLineEnding(i);

				i++;
			}
		}

		// Boss Stats
		int lineIdentifier = offset * 3;
		// First Line: Player Identification, AIControlled
		dumpArray[lineIdentifier][0] = "Boss;";
		dumpArray[lineIdentifier][2] = "AIControlled;";
		dumpArray[lineIdentifier][3] = bossConfig.AIControlled + ";";
		
		lineIdentifier++;
		
		// Values
	}
	#endregion
	
	
	
	#region Private Functions
	private static string PutRightLineEnding(int i)
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

