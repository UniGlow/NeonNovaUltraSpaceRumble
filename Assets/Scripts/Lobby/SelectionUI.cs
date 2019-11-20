using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

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
    bool keepColorsUpdated = false;
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
            Debug.Log("Test");
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
                    leftArrow.gameObject.SetActive(true);
                    rightArrow.gameObject.SetActive(true);
                    topPanel.SetActive(false);
                    break;
                case SelectionController.Step.ColorSelection:
                    playerColors = CopyColorList(NewSirAlfredLobby.Instance.AvailableColors);
                    FillTopPanel(SelectionController.Step.ColorSelection);
                    leftArrow.gameObject.SetActive(true);
                    rightArrow.gameObject.SetActive(true);
                    topPanel.SetActive(true);
                    break;
                case SelectionController.Step.AbilitySelection:
                    FillTopPanel(SelectionController.Step.AbilitySelection);
                    leftArrow.gameObject.SetActive(true);
                    rightArrow.gameObject.SetActive(true);
                    topPanel.SetActive(true);
                    break;
                case SelectionController.Step.Offline:
                    offlineScreen.SetActive(true);
                    selectionScreen.SetActive(false);
                    leftArrow.gameObject.SetActive(false);
                    rightArrow.gameObject.SetActive(false);
                    topPanel.SetActive(false);
                    break;
                case SelectionController.Step.ReadyToPlay:
                    leftArrow.gameObject.SetActive(false);
                    rightArrow.gameObject.SetActive(false);
                    topPanel.SetActive(false);
                    break;
            }
        }
    }

    public void ChangeText(int panelNumber, NewSirAlfredLobby.PlayerCharacter activeCharacter)
    {
        if(panelNumber == this.panelNumber)
        {
            textMesh.text = activeCharacter.ToString();
        }
    }

    public void AnimateArrow(int panelNumber, Direction direction)
    {
        if(panelNumber == this.panelNumber)
        {
            if(direction == Direction.Left)
            {
                leftArrow.DOScale(.8f, .05f).SetLoops(2, LoopType.Yoyo);
            }
            else
            {
                rightArrow.DOScale(.8f, .05f).SetLoops(2, LoopType.Yoyo);
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
        for(int i = playerColors.Count - 1; i >= 0; i--)
        {
            if (!tempColors.Contains(playerColors[i]))
            {
                GameObject toDestroy = topPanelSelectables[playerColors.IndexOf(playerColors[i])];
                Destroy(toDestroy);
                topPanelSelectables.Remove(toDestroy);
                playerColors.Remove(playerColors[i]);
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

    List<PlayerColor> CopyColorList(List<PlayerColor> listToCopy)
    {
        List<PlayerColor> list = new List<PlayerColor>();
        foreach(PlayerColor pc in listToCopy)
        {
            list.Add(pc);
        }
        return list;
    }
	#endregion
	
	
	
	#region GameEvent Raiser
	
	#endregion
	
	
	
	#region Coroutines
	
	#endregion
}