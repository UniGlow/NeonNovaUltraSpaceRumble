using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class SelectionUI : MonoBehaviour 
{

    #region Variable Declarations
    // Serialized Fields
    [Header("References")]
    [Tooltip("The Number of this Panel")]
    [Range(1,4)]
    [SerializeField] int panelNumber = 1;
    [SerializeField] GameObject offlineScreen = null;
    [SerializeField] GameObject selectionScreen = null;
    [SerializeField] GameObject topPanel = null;
    [SerializeField] TextMeshProUGUI textMesh = null;
    [SerializeField] RawImage renderTexture = null;
    [SerializeField] Transform leftArrow = null;
    [SerializeField] Transform rightArrow = null;
    [SerializeField] Image backgroundImage = null;
    [SerializeField] GameObject topPanelSelectablePrefab = null;
    // Private
    SelectionController.Step activeStep = SelectionController.Step.Offline;
    List<PlayerColor> playerColors = null;
    List<GameObject> topPanelSelectables = new List<GameObject>();
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
		
	}
	#endregion
	
	
	
	#region Public Functions
	public void ChangeStep(int panelNumber, SelectionController.Step nextStep)
    {
        if(activeStep == SelectionController.Step.ColorSelection)
        {
            UpdateTopPanel();
        }
        if (this.panelNumber == panelNumber)
        {
            if(nextStep != SelectionController.Step.ColorSelection)
            {
                ClearTopPanel();
            }
            activeStep = nextStep;
            switch (nextStep)
            {
                case SelectionController.Step.CharacterSelection:
                    offlineScreen.SetActive(false);
                    selectionScreen.SetActive(true);
                    break;
                case SelectionController.Step.ColorSelection:
                    playerColors = NewSirAlfredLobby.Instance.AvailableColors;
                    FillTopPanel(SelectionController.Step.ColorSelection);
                    break;
                case SelectionController.Step.AbilitySelection:
                    FillTopPanel(SelectionController.Step.AbilitySelection);
                    break;
                case SelectionController.Step.Offline:
                    offlineScreen.SetActive(true);
                    selectionScreen.SetActive(false);
                    break;
            }
        }
    }
	#endregion
	
	
	
	#region Private Functions
	void FillTopPanel(SelectionController.Step forStep)
    {
        switch (forStep)
        {
            case SelectionController.Step.ColorSelection:
                foreach(PlayerColor pc in playerColors)
                {
                    GameObject topPanelSelectable = Instantiate(topPanelSelectablePrefab, topPanel.transform);
                    topPanelSelectables.Add(topPanelSelectable);
                    topPanelSelectable.GetComponent<Image>().color = pc.uiElementColor;
                }
                break;
        }
    }

    void UpdateTopPanel()
    {
        List<PlayerColor> tempColors = NewSirAlfredLobby.Instance.AvailableColors;
        foreach(PlayerColor pc in tempColors)
        {
            if (!playerColors.Contains(pc))
            {
                playerColors.Add(pc);
                GameObject toAdd = Instantiate(topPanelSelectablePrefab, topPanel.transform);
                toAdd.GetComponent<Image>().color = pc.uiElementColor;
                topPanelSelectables.Add(toAdd);
            }
        }
        foreach(PlayerColor pc in playerColors)
        {
            if (!tempColors.Contains(pc))
            {
                GameObject toDestroy = topPanelSelectables[playerColors.IndexOf(pc)];
                Destroy(toDestroy);
                topPanelSelectables.Remove(toDestroy);
                playerColors.Remove(pc);
            }
        }
    }

    void ClearTopPanel()
    {
        foreach(GameObject go in topPanelSelectables)
        {
            Destroy(go);
        }
        topPanelSelectables.Clear();
    }
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}

