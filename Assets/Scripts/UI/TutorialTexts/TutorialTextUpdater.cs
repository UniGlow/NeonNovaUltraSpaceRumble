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
    #endregion



    #region Public Functions
    public void UpdateText(string title)
    {
        for (int i = 0; i < textCollection.messages.Length; i++)
        {
            if (textCollection.messages[i].title == title)
            {
                textMesh.text = textCollection.messages[i].body;
            }
        }
    }

    public abstract void BossColorChanged();
    #endregion



    #region Protected Functions
    protected virtual void InheritedStart() { }
    #endregion
}
