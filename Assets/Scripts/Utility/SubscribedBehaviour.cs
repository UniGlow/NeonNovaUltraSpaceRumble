using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom Behaviour for our game. Every object, that needs to know about the current game state 
/// needs to inherit from this (instead of MonoBehaviour) and implement it's methods
/// </summary>
abstract public class SubscribedBehaviour : MonoBehaviour {

    // Subscribing to all custom game events
    protected virtual void OnEnable() {

    }

    // Unsubscribing from all custom game events
    protected virtual void OnDisable() {

    }

}
