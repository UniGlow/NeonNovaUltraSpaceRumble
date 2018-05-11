using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class HeroTutorialTextUpdater : MonoBehaviour 
{

    #region Variable Declarations
    [SerializeField] TextCollection textCollection;

    TextMeshPro textMesh;
    Hero hero;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start () 
	{
        textMesh = GetComponent<TextMeshPro>();
        hero = transform.parent.GetComponent<Hero>();

        if (hero.ability == Ability.Damage) UpdateText("Damage");
        else if (hero.ability == Ability.Tank) UpdateText("Tank");
        else if (hero.ability == Ability.Opfer) UpdateText("Opfer");
    }
	
	private void Update () 
	{
		
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
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
}
