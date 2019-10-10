using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerSetup
{

	public static void SetupPlayers(int playerCount, PlayerConfig bossPlayerConfig, PlayerConfig hero1PlayerConfig, PlayerConfig hero2PlayerConfig, PlayerConfig hero3PlayerConfig, Ability damageAbility, Ability tankAbility, Ability victimAbility, ColorSet colorSet)
    {
        switch (playerCount)
        {
            case 1:
                bossPlayerConfig.Initialize(1, Faction.Boss, colorSet.GetRandomColor(), false);
                hero1PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color1, true);
                hero1PlayerConfig.ability = damageAbility;
                hero2PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color2, true);
                hero2PlayerConfig.ability = tankAbility;
                hero3PlayerConfig.Initialize(4, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.ability = victimAbility;
                break;

            case 2:
                hero1PlayerConfig.Initialize(1, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = damageAbility;
                hero2PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = tankAbility;
                hero3PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color3, true);
                hero3PlayerConfig.ability = victimAbility;
                bossPlayerConfig.Initialize(4, Faction.Boss, colorSet.GetRandomColor(), true);
                break;

            case 3:
                hero1PlayerConfig.Initialize(1, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = damageAbility;
                hero2PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = tankAbility;
                hero3PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.ability = victimAbility;
                bossPlayerConfig.Initialize(4, Faction.Boss, colorSet.GetRandomColor(), true);
                break;

            case 4:
                bossPlayerConfig.Initialize(1, Faction.Boss, colorSet.GetRandomColor(), false);
                hero1PlayerConfig.Initialize(2, Faction.Heroes, colorSet.color1, false);
                hero1PlayerConfig.ability = damageAbility;
                hero2PlayerConfig.Initialize(3, Faction.Heroes, colorSet.color2, false);
                hero2PlayerConfig.ability = tankAbility;
                hero3PlayerConfig.Initialize(4, Faction.Heroes, colorSet.color3, false);
                hero3PlayerConfig.ability = victimAbility;
                break;

            default:
                break;
        }

        GameManager.Instance.activeColorSet = colorSet;
        GameManager.Instance.IsInitialized = true;
    }
}
