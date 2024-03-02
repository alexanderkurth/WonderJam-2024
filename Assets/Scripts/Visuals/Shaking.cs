using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaking : MonoBehaviour
{
    [SerializeField]
    float shakeAmount = 1.0f;
    [SerializeField]
    float shakeSpeed = 1.0f;

    float offset;
    Vector2 startingPos;

    void Awake()
    {
        offset = Random.value * 1000;
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 curPos = transform.position;
        curPos.x = startingPos.x + Mathf.Sin((Time.time + offset) * shakeSpeed) * shakeAmount/10;
        curPos.z = startingPos.y + Mathf.Sin((Time.time + offset) * shakeSpeed) * shakeAmount/10;
        transform.position = curPos;

    }
}
