using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(fileName = "NewTextCollection", menuName = "TextCollection")]
public class TextCollection : ScriptableObject 
{
    [System.Serializable]
	public class Message
    {
        public string title;
        [TextArea(2,4)]
        public string body;
    }

    public Message[] messages;
}
