using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRandomizer : MonoBehaviour
{
    public float maxSize = 3;
    public float minSize = 1;

    #if UNITY_EDITOR
    [ContextMenu("Randomize")]
    void Randomize()
    {
        float newScale = Random.Range(minSize, maxSize);
        gameObject.transform.localScale = new Vector3(newScale,newScale,newScale);
    }
    #endif
}
