using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class HumanAttack : MonoBehaviour
{
    public int DashDistance = 1;
    public int PushDistance = 3;

    public StarterAssetsInputs inputs;

    public HumanController humanController;

    Collider2D collider;

    HumanAttack otherHumanAttack = null;


    // Start is called before the first frame update
    void Start()
    {
        if (inputs == null)
        {
            inputs = transform.parent.gameObject.GetComponent<StarterAssetsInputs>();
        }

        if (humanController == null)
        {
            humanController = transform.parent.gameObject.GetComponent<HumanController>();
        }
    }

    bool isDashReleased = true;
    // Update is called once per frame
    void Update()
    {
        if (inputs != null)
        {
            if (inputs.sprint == true)
            {
                if (isDashReleased)
                {
                    humanController.Dash(DashDistance);
                    //Debug.Log("Attack");

                    PushHuman();
                }

                isDashReleased = false;
            }
            else
            {
                isDashReleased = true;
            }
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<HumanAttack>() != null)
        {
            otherHumanAttack = null;
            //Debug.Log("OnTriggerExit2D" + other.name);

        }
    }
}
