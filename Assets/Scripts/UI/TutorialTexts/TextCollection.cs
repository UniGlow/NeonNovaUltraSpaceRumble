using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(fileName = "New Text Collection", menuName = "Scriptable Objects/Text Collection")]
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
