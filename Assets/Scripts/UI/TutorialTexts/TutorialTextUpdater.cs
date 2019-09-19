using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public abstract class TutorialTextUpdater : MonoBehaviour 
{

    #region Variable Declarations
    [SerializeField] protected TextCollection textCollection;
    [Range(1, 4)]
    [SerializeField] protected int playerToFollow;

    protected TextMeshPro textMesh;
    protected Transform player;
    protected Quaternion startRotation;
    protected Vector3 startOffset;
    public static int colorChanges = 0;
    #endregion



    #region Unity Event Functions
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();

        InheritedStart();
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position + startOffset;
            transform.rotation = startRotation;
        }
    }
    #endregion



    #region Public Functions
    public void Initialize(PlayerConfig hero1, PlayerConfig hero2, PlayerConfig hero3, PlayerConfig boss)
    {
        switch (playerToFollow)
        {
            case 1:
                player = hero1.playerTransform;
                break;
            case 2:
                player = hero2.playerTransform;
                break;
            case 3:
                player = hero3.playerTransform;
                break;
            case 4:
                player = boss.playerTransform;
                break;
            default:
                break;
        }

        startRotation = transform.rotation;
        startOffset = transform.position - player.transform.position;
    }

    public static void BossColorChange(PlayerConfig bossConfig, int i = -1)
    {
        // Update static variable
        if (i == -1) colorChanges++;
        else colorChanges = i;

        UpdateTexts(bossConfig);
    }

    public static void UpdateTexts(PlayerConfig bossConfig)
    {
        // Inform all TutorialTextUpdater in scene
        // TODO: Falls behalten: Referenz ändern
        TutorialTextUpdater[] textUpdaters = GameObject.FindObjectsOfType<TutorialTextUpdater>();
        foreach (TutorialTextUpdater updater in textUpdaters)
        {
            updater.UpdateText(bossConfig);
        }
    }
    #endregion



    #region Protected Functions
    protected void ChangeTextTo(string title)
    {
        if (title == "")
        {
            textMesh.text = "";
            return;
        }

        for (int i = 0; i < textCollection.messages.Length; i++)
        {
            if (textCollection.messages[i].title == title)
            {
                textMesh.text = textCollection.messages[i].body;
            }
        }
    }

    protected virtual void InheritedStart() { }
    public abstract void UpdateText(PlayerConfig bossConfig);
    #endregion
}
