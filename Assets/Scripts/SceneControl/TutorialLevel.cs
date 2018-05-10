using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class TutorialLevel : MonoBehaviour 
{

    #region Variable Declaration
    [SerializeField] PlayerReadyUpdater playerReadyUpdater;
    bool[] playerConfirms;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        playerConfirms = new bool[GameManager.Instance.PlayerCount];
	}
	
	private void Update () 
	{
        // Load next scene if all players are ready
        int ready = 0;
        for (int i = 0; i < playerConfirms.Length; i++)
        {
            if (playerConfirms[i] == true) ready++;
        }
        if (ready == playerConfirms.Length)
        {
            StartCoroutine(Wait(1f, () => { GameManager.Instance.LoadNextScene(); }));
        }

        // Check Inputs to ready up
		if (Input.GetButtonDown(Constants.INPUT_SUBMIT + "1"))
        {
            playerConfirms[0] = !playerConfirms[0];
            playerReadyUpdater.UpdateState(0, playerConfirms[0]);
        }
        else if (Input.GetButtonDown(Constants.INPUT_SUBMIT + "2"))
        {
            playerConfirms[1] = !playerConfirms[1];
            playerReadyUpdater.UpdateState(1, playerConfirms[1]);
        }
        else if (Input.GetButtonDown(Constants.INPUT_SUBMIT + "3"))
        {
            playerConfirms[2] = !playerConfirms[2];
            playerReadyUpdater.UpdateState(2, playerConfirms[2]);
        }
        else if (Input.GetButtonDown(Constants.INPUT_SUBMIT + "4"))
        {
            playerConfirms[3] = !playerConfirms[3];
            playerReadyUpdater.UpdateState(3, playerConfirms[3]);
        }
    }
    #endregion



    #region Coroutines
    IEnumerator Wait(float duration, System.Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        onComplete.Invoke();
    }
    #endregion
}
