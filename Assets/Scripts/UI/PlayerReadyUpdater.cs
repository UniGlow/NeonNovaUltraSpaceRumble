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
    [SerializeField] Color readyColor;
    [SerializeField] Color notReadyColor;

    ReadyState[] readyStates;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
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

        for (int i = GameManager.Instance.PlayerCount; i < readyStates.Length; i++)
        {
            readyStates[i].parent.SetActive(false);
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
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
}
