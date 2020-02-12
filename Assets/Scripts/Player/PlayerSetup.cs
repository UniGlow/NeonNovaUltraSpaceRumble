using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public static class PlayerSetup
{

	public static void SetupPlayers(int playerCount, PlayerConfig bossPlayerConfig, PlayerConfig hero1PlayerConfig, PlayerConfig hero2PlayerConfig, PlayerConfig hero3PlayerConfig, Ability damageAbility, Ability tankAbility, Ability victimAbility, ColorSet colorSet)
    {
        switch (playerCount)
        {
            case 1:
                bossPlayerConfig.Initialize(ReInput.players.GetPlayer(0), 0, Faction.Boss, colorSet.GetRandomColor(), false);
                ReInput.players.GetPlayer(0).isPlaying = true;
                hero1PlayerConfig.Initialize(ReInput.players.GetPlayer(1), 1, Faction.Heroes, colorSet.color1, true);
                hero1PlayerConfig.SetupAbility(damageAbility);
                ReInput.players.GetPlayer(1).isPlaying = false;
                hero2PlayerConfig.Initialize(ReInput.players.GetPlayer(2), 2, Faction.Heroes, colorSet.color2, true);
                hero2PlayerConfig.SetupAbility(tankAbility);
                ReInput.players.GetPlayer(2).isPlaying = false;
                hero3PlayerConfig.Initialize(ReInput.players.GetPlayer(3), 3, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.SetupAbility(victimAbility);
                ReInput.players.GetPlayer(3).isPlaying = false;
                break;

            case 2:
                hero1PlayerConfig.Initialize(ReInput.players.GetPlayer(0), 0, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.SetupAbility(damageAbility);
                ReInput.players.GetPlayer(0).isPlaying = true;
                hero2PlayerConfig.Initialize(ReInput.players.GetPlayer(1), 1, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.SetupAbility(tankAbility);
                ReInput.players.GetPlayer(1).isPlaying = true;
                hero3PlayerConfig.Initialize(ReInput.players.GetPlayer(2), 2, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.SetupAbility(victimAbility);
                ReInput.players.GetPlayer(2).isPlaying = false;
                bossPlayerConfig.Initialize(ReInput.players.GetPlayer(3), 3, Faction.Boss, colorSet.GetRandomColor(), true);
                ReInput.players.GetPlayer(3).isPlaying = false;
                break;

            case 3:
                hero1PlayerConfig.Initialize(ReInput.players.GetPlayer(0), 0, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.SetupAbility(damageAbility);
                ReInput.players.GetPlayer(0).isPlaying = true;
                hero2PlayerConfig.Initialize(ReInput.players.GetPlayer(1), 1, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.SetupAbility(tankAbility);
                ReInput.players.GetPlayer(1).isPlaying = true;
                hero3PlayerConfig.Initialize(ReInput.players.GetPlayer(2), 2, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.SetupAbility(victimAbility);
                ReInput.players.GetPlayer(2).isPlaying = true;
                bossPlayerConfig.Initialize(ReInput.players.GetPlayer(3), 3, Faction.Boss, colorSet.GetRandomColor(), true);
                ReInput.players.GetPlayer(3).isPlaying = false;
                break;

            case 4:
                bossPlayerConfig.Initialize(ReInput.players.GetPlayer(0), 0, Faction.Boss, colorSet.GetRandomColor(), false);
                ReInput.players.GetPlayer(0).isPlaying = true;
                hero1PlayerConfig.Initialize(ReInput.players.GetPlayer(1), 1, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.SetupAbility(damageAbility);
                ReInput.players.GetPlayer(1).isPlaying = true;
                hero2PlayerConfig.Initialize(ReInput.players.GetPlayer(2), 2, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.SetupAbility(tankAbility);
                ReInput.players.GetPlayer(2).isPlaying = true;
                hero3PlayerConfig.Initialize(ReInput.players.GetPlayer(3), 3, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.SetupAbility(victimAbility);
                ReInput.players.GetPlayer(3).isPlaying = true;
                break;

            default:
                break;
        }

        GameManager.Instance.activeColorSet = colorSet;
        GameManager.Instance.IsInitialized = true;
    }
}
