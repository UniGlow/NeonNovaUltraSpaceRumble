using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides a Debug Mode Menu
/// </summary>
public class DebugMode : SubscribedBehaviour {

    public static DebugMode Instance;

    private bool debugMode = false;



    //Awake is always called before any Start functions
    private void Awake() {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this) { 
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a DebugMode.
            Debug.Log("There can only be one DebugMode instantiated. Destroying this Instance...");
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    private void Update () {
        // Switch the Games Debug Mode On/Off
        if (Input.GetButtonDown(Constants.INPUT_DEBUGMODE)) {
            debugMode = !debugMode;
        }
    }



    #region Custom Event Functions
    // Every child of SubscribedBehaviour can implement these
    #endregion
    


    // Draws the GUI for the Debug Mode and declares it's functionality
    private void OnGUI() {
        if (debugMode) {
            // Setup of the box and title
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Debug Menu");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Button for loading the next scene by buildIndex
            GUILayout.BeginVertical("box");
            GUILayout.Label("Scene Management:");
            if (GUILayout.Button("Load next scene")) {
                Debug.Log("Debug Mode: Loading next scene.");
                int activeScene = SceneManager.GetActiveScene().buildIndex;
                if (activeScene + 1 < SceneManager.sceneCountInBuildSettings) {
                    SceneManager.LoadScene(activeScene + 1);
                }
                else {
                    SceneManager.LoadScene(0);
                }
            }
            if (GUILayout.Button("Reload scene")) {
                Debug.Log("Debug Mode: Reloading current scene.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            GUILayout.EndVertical();
        }
    }
}
