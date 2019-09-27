using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// Provides functions to handle game-specific tasks regarding the Rewired Input System.
/// </summary>
public static class InputHelper
{
    public class PlayerRuleSet
    {
        public Player player;
        public List<ControllerMapLayoutManager.RuleSet> ruleSets = new List<ControllerMapLayoutManager.RuleSet>();
    }



    /// <summary>
    /// Changes the RuleSet for all players to the defined ruleSet.
    /// </summary>
    /// <param name="ruleSet"></param>
    /// <returns>Returns a list of the currently active rulesets for all players at the time of calling the function.</returns>
    public static List<PlayerRuleSet> ChangeRuleSetForAllPlayers(int ruleSet)
    {
        List<PlayerRuleSet> playerRuleSets = new List<PlayerRuleSet>();

        foreach (Player player in ReInput.players.Players)
        {
            playerRuleSets.Add(new PlayerRuleSet { player = player, ruleSets = player.controllers.maps.layoutManager.ruleSets });
            player.controllers.maps.layoutManager.ruleSets.Clear();
            player.controllers.maps.layoutManager.ruleSets.Add(ReInput.mapping.GetControllerMapLayoutManagerRuleSetInstance(ruleSet));
            player.controllers.maps.layoutManager.Apply();
        }

        return playerRuleSets;
    }

    /// <summary>
    /// Changes the RuleSet for the specified player to the specified RuleSet.
    /// </summary>
    /// <param name="ruleSet">List of Players and RuleSets that sould be changed.</param>
    /// <returns>Returns a list of the previously active rulesets for all players at the time of calling the function (for buffering if you want to revert the rulesets later).</returns>
    public static List<PlayerRuleSet> ChangeRuleSetForPlayers(List<PlayerRuleSet> playerRuleSets)
    {
        List<PlayerRuleSet> previousPlayerRuleSets = new List<PlayerRuleSet>();

        foreach (Player player in ReInput.players.Players)
        {
            // Buffer current playerRuleSets
            previousPlayerRuleSets.Add(new PlayerRuleSet { player = player, ruleSets = player.controllers.maps.layoutManager.ruleSets });

            // Does the given list contain the current player?
            PlayerRuleSet currentPlayerRuleSet = playerRuleSets.Find(x => x.player == player);
            if (currentPlayerRuleSet != null)
            {
                player.controllers.maps.layoutManager.ruleSets.Clear();
                player.controllers.maps.layoutManager.ruleSets.AddRange(currentPlayerRuleSet.ruleSets);
                player.controllers.maps.layoutManager.Apply();
            }
        }

        return previousPlayerRuleSets;
    }
}
