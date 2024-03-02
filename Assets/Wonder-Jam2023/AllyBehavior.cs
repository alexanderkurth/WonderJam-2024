using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBehavior : MonoBehaviour
{
    public GameObject OnGround;
    public GameObject StandUp;

    public GameObject SpriteObject;

    [SerializeField, Range(0, 10)]
    private float m_Speed = 1.0f;

    bool m_IsOnGround = true;

    // Start is called before the first frame update
    void Start()
    {
        m_IsOnGround = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_IsOnGround)
        {
            transform.Translate(transform.forward * Time.deltaTime * m_Speed, Space.World);
        }
    }

    public void ChangeState()
    {
        OnGround.SetActive(false);
        StandUp.SetActive(true);
        m_IsOnGround = false;
    }
}
