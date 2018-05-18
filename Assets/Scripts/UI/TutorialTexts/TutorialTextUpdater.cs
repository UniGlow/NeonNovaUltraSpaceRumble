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

    protected TextMeshPro textMesh;
    protected Player player;
    protected Quaternion startRotation;
    protected Vector3 startOffset;
    public static int colorChanges = 0;
    #endregion



    #region Unity Event Functions
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        player = transform.parent.GetComponent<Player>();
        startRotation = transform.rotation;
        startOffset = transform.position - player.transform.position;

        InheritedStart();
    }

    void Update()
    {
        transform.position = player.transform.position + startOffset;
        transform.rotation = startRotation;
    }
    #endregion



    #region Public Functions
    public static void BossColorChange(int i = -1)
    {
        // Update static variable
        if (i == -1) colorChanges++;
        else colorChanges = i;

        UpdateTexts();
    }

    public static void UpdateTexts()
    {
        // Inform all TutorialTextUpdater in scene
        TutorialTextUpdater[] textUpdaters = GameObject.FindObjectsOfType<TutorialTextUpdater>();
        foreach (TutorialTextUpdater updater in textUpdaters)
        {
            updater.UpdateText();
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
    protected abstract void UpdateText();
    #endregion
}
