using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField]
    float TimeDestroySeconds = 5.0f;

    float m_Elapsed;
    // Start is called before the first frame update
    void Awake()
    {
        m_Elapsed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_Elapsed += Time.deltaTime;
        if(m_Elapsed >= TimeDestroySeconds)
        {
            Destroy(this.gameObject);
            return;
        }
    }
}
