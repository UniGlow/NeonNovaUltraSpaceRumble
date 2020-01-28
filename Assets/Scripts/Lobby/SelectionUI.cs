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
    [SerializeField] Transform leftArrow = null;
    [SerializeField] Transform rightArrow = null;
    [SerializeField] GameObject topPanelSelectablePrefab = null;
    [SerializeField] Transform selectionMarker = null;
    // Private
    SelectionController.Step activeStep = SelectionController.Step.Offline;
    List<PlayerColor> playerColors = null;
    PlayerColor activeColor = null;
    List<GameObject> topPanelSelectables = new List<GameObject>();
	#endregion
	
	
	
	#region Public Properties
	
	#endregion
	
	
	
	#region Unity Event Functions

	#endregion
	
	
	
	#region Public Functions
    /// <summary>
    /// Call this when a Player enters a new Lobby State
    /// </summary>
    /// <param name="panelNumber">Number of Panel the Player is occupying</param>
    /// <param name="nextStep">The Lobby State that got entered by the Player</param>
	public void StepChanged(int panelNumber, SelectionController.Step nextStep)
    {
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
                    selectionMarker.gameObject.SetActive(false);
                    break;
                case SelectionController.Step.ColorSelection:
                    playerColors = CopyColorList(SirAlfredLobby.Instance.AvailableColors);
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
                    selectionMarker.gameObject.SetActive(true);
                    break;
                case SelectionController.Step.Offline:
                    offlineScreen.SetActive(true);
                    selectionScreen.SetActive(false);
                    leftArrow.gameObject.SetActive(false);
                    rightArrow.gameObject.SetActive(false);
                    topPanel.SetActive(false);
                    selectionMarker.gameObject.SetActive(false);
                    break;
                case SelectionController.Step.ReadyToPlay:
                    leftArrow.gameObject.SetActive(false);
                    rightArrow.gameObject.SetActive(false);
                    StartCoroutine(InvokeOneFrameLater(() =>
                    {
                        topPanel.SetActive(false);
                    }, 0.05f));
                    selectionMarker.gameObject.SetActive(false);
                    break;
            }
        }
        if (activeStep == SelectionController.Step.ColorSelection)
        {
            UpdateTopPanel();
        }
    }

    /// <summary>
    /// Call this to Change the Text displayed in the lower Center of this Panel
    /// </summary>
    /// <param name="panelNumber">The Panel Number of the Player</param>
    /// <param name="activeCharacter">The Active Character that Player can see to display its Name</param>
    public void ChangeText(int panelNumber, SirAlfredLobby.PlayerCharacter activeCharacter)
    {
        if(panelNumber == this.panelNumber)
        {
            textMesh.text = activeCharacter.ToString();
        }
    }

    /// <summary>
    /// Animates the Arrow in UI depending of the given Direction
    /// </summary>
    /// <param name="panelNumber">The Panel Number of the Player</param>
    /// <param name="direction">Direction of the Arrow that should be animated</param>
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

    /// <summary>
    /// Call this when a Player changed the selected Color
    /// </summary>
    /// <param name="panelNumber">The Panel Number of the Player</param>
    /// <param name="playerColor">The new selected Color</param>
    // TODO: Once GameEvents can handle Optional Parameters combine following Methods into one
    public void SelectionChanged(int panelNumber, PlayerColor playerColor)
    {
        if(panelNumber == this.panelNumber)
        {
            activeColor = playerColor;
            for(int i = 0; i < playerColors.Count; i++)
            {
                if (playerColors[i] == playerColor)
                    PlaceSelectionMarker(i, true);
            }
        }
    }
    // Not Public but sits here for contextual reasons, see Method aboth
    void SelectionChanged(PlayerColor playerColor, bool animated)
    {
        activeColor = playerColor;
        for (int i = 0; i < playerColors.Count; i++)
        {
            if (playerColors[i] == playerColor)
                PlaceSelectionMarker(i, animated);
        }
    }
	#endregion
	
	
	
	#region Private Functions
    /// <summary>
    /// This will fill the Top Panel with all selectable Items such as Colors etc.
    /// </summary>
    /// <param name="forStep">The Lobby State for which to fill the Top Panel (contextual)</param>
	void FillTopPanel(SelectionController.Step forStep)
    {
        // TODO: Step for AbilitySelection once that is implemented
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
        if(activeColor == null)
        {
            activeColor = playerColors[0];
        }
        // Set Selection Marker to size of Top Panel Selectables (+1 unit in every direction) (takes 2x Height of a TopPanelSelectable to fit the Aspect Radio of Selection Marker)
        StartCoroutine(InvokeOneFrameLater(() =>
        {
            selectionMarker.GetComponent<RectTransform>().sizeDelta = new Vector2(topPanelSelectables[0].GetComponent<RectTransform>().rect.height, topPanelSelectables[0].GetComponent<RectTransform>().rect.height);
            SelectionChanged(activeColor, false);
        }));
    }

    /// <summary>
    /// Updates the Top Panel to Account for Changes in Availablility of selectable Items.
    /// </summary>
    void UpdateTopPanel()
    {
        bool somethingChanged = false;
        switch (activeStep) {
            case SelectionController.Step.ColorSelection:
                List<PlayerColor> tempColors = SirAlfredLobby.Instance.AvailableColors;
                foreach (PlayerColor pc in tempColors)
                {
                    if (!playerColors.Contains(pc))
                    {
                        playerColors.Add(pc);
                        GameObject toAdd = Instantiate(topPanelSelectablePrefab, topPanel.transform);
                        toAdd.GetComponent<Image>().color = pc.uiElementColor;
                        topPanelSelectables.Add(toAdd);
                        somethingChanged = true;
                    }
                }
                for (int i = playerColors.Count - 1; i >= 0; i--)
                {
                    if (!tempColors.Contains(playerColors[i]))
                    {
                        GameObject toDestroy = topPanelSelectables[playerColors.IndexOf(playerColors[i])];
                        Destroy(toDestroy);
                        topPanelSelectables.Remove(toDestroy);
                        playerColors.Remove(playerColors[i]);
                        somethingChanged = true;
                    }
                }
                break;
        }
        if (somethingChanged)
        {
            Debug.Log("Active Step: " + activeStep.ToString());
            // Set Selection Marker to size of Top Panel Selectables (takes 2x Height of a TopPanelSelectable to fit the Aspect Radio of Selection Marker)
            StartCoroutine(InvokeOneFrameLater(()=>
            {
                selectionMarker.GetComponent<RectTransform>().sizeDelta = new Vector2(topPanelSelectables[0].GetComponent<RectTransform>().rect.height, topPanelSelectables[0].GetComponent<RectTransform>().rect.height);
                SelectionChanged(activeColor, false);
            }));
        }
    }

    /// <summary>
    /// Places the SelectionMarker at the Position of the given Item-Number inside the Top Panel
    /// </summary>
    /// <param name="itemNumber">Number (or Position) of the Item to Select (beginning at 0)</param>
    /// <param name="animated"></param>
    void PlaceSelectionMarker(int itemNumber, bool animated = false)
    {
        if (!animated)
        {
            // Set Selection Marker to Position of first Object
            selectionMarker.DOMove(topPanelSelectables[itemNumber].transform.position, 0.001f).OnComplete(()=> {
                selectionMarker.gameObject.SetActive(true);
            });

        }
        else
        {
            // Set Selection Marker to its new Target, Take Position from Selectable List (Animated)
            StartCoroutine(InvokeOneFrameLater(() =>
            {
                selectionMarker.DOMove(topPanelSelectables[itemNumber].transform.position, 0.25f).SetEase(Ease.InOutCubic);
            }));
        }
    }

    /// <summary>
    /// Deletes all entries for Selectable Items in the Top Panel
    /// </summary>
    void ClearTopPanel()
    {
        foreach(GameObject go in topPanelSelectables)
        {
            Destroy(go);
        }
        topPanelSelectables.Clear();
    }

    /// <summary>
    /// Makes a deep copy of the given List of PlayerColors and returns it
    /// </summary>
    /// <param name="listToCopy">List to Copy</param>
    /// <returns>Deep Copy of the given List</returns>
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
    /// <summary>
    /// Invokes the given Action one Frame later
    /// </summary>
    /// <param name="action">Action to Invoke</param>
    /// <returns>Nothing</returns>
	IEnumerator InvokeOneFrameLater(System.Action action, float delay = 0f)
    {
        if (delay == 0f)
            yield return null;
        else
            yield return new WaitForSecondsRealtime(delay);
        action.Invoke();
    }
	#endregion
}