using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour
{

    private void OnEnable()
    {
        if (transform.childCount == 0) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
