using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class HumanAttack : MonoBehaviour
{
    public int PushDistance = 3;

    public StarterAssetsInputs inputs;

    public HumanController humanController;

    Collider2D collider;

    HumanAttack otherHumanAttack = null;


    // Start is called before the first frame update
    void Start()
    {
        if (humanController == null)
        {
            humanController = transform.parent.gameObject.GetComponent<HumanController>();
        }
    }

    public void PushHuman()
    {
        if (otherHumanAttack != null)
        {
            otherHumanAttack.humanController.GetPushed(PushDistance, humanController.transform.up);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<HumanAttack>() != null)
        {
            otherHumanAttack = other.gameObject.GetComponent<HumanAttack>();
            //Debug.Log("OnTriggerEnter2D" + other.name);
        }

        if (humanController.isDashing)
        {
            PushHuman();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (otherHumanAttack != null && humanController.isDashing)
        {
            PushHuman();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<HumanAttack>() != null)
        {
            otherHumanAttack = null;
            //Debug.Log("OnTriggerExit2D" + other.name);

        }
    }
}
