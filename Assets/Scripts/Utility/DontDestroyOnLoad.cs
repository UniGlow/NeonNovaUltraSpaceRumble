using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour {

    private void OnEnable() {
        DontDestroyOnLoad(gameObject);
    }
}
