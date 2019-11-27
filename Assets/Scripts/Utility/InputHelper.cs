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

    /// <summary>
    /// Updates the player count depending on connected Joysticks.
    /// </summary>
    /// <returns>Returns the number of currently active players.</returns>
    public static int UpdatePlayerCount()
    {
        int playerCount = 0;

        foreach (Player player in ReInput.players.Players)
        {
            if (player.controllers.joystickCount >= 1)
            {
                playerCount++;
                player.isPlaying = true;
            }
            else
            {
                player.isPlaying = false;
            }
        }

        return playerCount;
    }

    /// <summary>
    /// Returns input for the defined action from all players.
    /// </summary>
    /// <param name="actionID">The action identifier.</param>
    /// <returns>True if any player pressed the specified button.</returns>
    public static bool GetButtonDown(int actionID)
    {
        bool buttonPressed = false;

        foreach (Player player in ReInput.players.Players)
        {
            if (player.GetButtonDown(actionID)) buttonPressed = true;
        }

        return buttonPressed;
    }

    public static Player GetPlayerButtonDown(int actionID)
    {
        foreach (Player player in ReInput.players.Players)
        {
            if (player.GetButtonDown(actionID)) return player;
        }

        return null;
    }

    /// <summary>
    /// Returns input for the defined action from all players.
    /// Will return true as long as any Player keeps holding that action.
    /// </summary>
    /// <param name="actionID">The action identifier.</param>
    /// <returns>True if any player pressed or holds the specified button.</returns>
    public static bool GetButton(int actionID)
    {
        bool buttonPressed = false;

        foreach(Player player in ReInput.players.Players)
        {
            if (player.GetButton(actionID)) buttonPressed = true;
        }

        return buttonPressed;
    }

    /// <summary>
    /// Gets any button down from all players.
    /// </summary>
    /// <returns>True if any player pressed any button.</returns>
    public static bool GetAnyButtonDown()
    {
        bool buttonPressed = false;

        foreach (Player player in ReInput.players.Players)
        {
            if (player.GetAnyButtonDown()) buttonPressed = true;
        }

        return buttonPressed;
    }
}
