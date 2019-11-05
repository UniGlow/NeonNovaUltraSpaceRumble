using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class PlayerReadyUpdater : MonoBehaviour 
{
    [System.Serializable]
    public class ReadyState
    {
        public GameObject parent;
        public TextMeshProUGUI readyText;
    }

    #region Variable Declarations
    [SerializeField] Color readyColor = new Color();
    [SerializeField] Color notReadyColor = new Color();

    ReadyState[] readyStates;
    int playerCount;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Awake () 
	{
        readyStates = new ReadyState[4];
        for (int i = 0; i < readyStates.Length; i++)
        {
            readyStates[i] = new ReadyState
            {
                parent = transform.GetChild(i).gameObject,
                readyText = transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>()
            };
        }
	}
	#endregion
	
	
	
	#region Public Functions
	public void UpdateState(int playerNumber, bool ready)
    {
        if (ready)
        {
            readyStates[playerNumber].readyText.text = "READY";
            readyStates[playerNumber].readyText.color = readyColor;
        }
        else
        {
            readyStates[playerNumber].readyText.text = "PRESS A TO READY UP";
            readyStates[playerNumber].readyText.color = notReadyColor;
        }
    }

    public void UpdateUIElements()
    {
        int playerCount = InputHelper.UpdatePlayerCount();

        if (playerCount == this.playerCount)
        {
            return;
        }

        this.playerCount = playerCount;
        for (int i = 0; i < readyStates.Length; i++)
        {
            UpdateState(i, false);
            readyStates[i].parent.SetActive(false);
        }

        for (int i = 0; i < readyStates.Length; i++)
        {
            if (playerCount > i)
            {
                readyStates[i].parent.SetActive(true);
            }
        }
    }
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
}
