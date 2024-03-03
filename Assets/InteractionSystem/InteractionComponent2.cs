using UnityEngine;

public class InteractionComponent2 : MonoBehaviour
{
     public BaseIA bestTarget = null;
     public MontureController bestSaddle = null;
    HumanController humanController = null;

    private void Start()
    {
        humanController = GetComponent<HumanController>();
    }

    private void Update()
    {
        float distance = Mathf.Infinity;
        float distance2 = Mathf.Infinity;
        Vector3 pos = transform.position;

        foreach (BaseIA animal in InteractionManager2.Instance.m_Animals)
        {
            if (animal != null)
            {
                Vector3 posAnimal = animal.transform.position;
                float tempDistance = Vector3.Distance(posAnimal, pos);
                animal.SetOulineVisibility(false);

                if(tempDistance < distance)
                {
                    distance = tempDistance;
                    if (humanController != null)
                    {
                        humanController._text.gameObject.SetActive(false);
                        if (distance < humanController.radius && !animal.IsGrab)
                        {
                            animal.SetOulineVisibility(true);
                            humanController._text.gameObject.SetActive(true);
                            humanController._text.gameObject.transform.position = animal.transform.position;
                        }
                    }

                    if(bestTarget != null)
                    {
                        bestTarget.SetOulineVisibility(false);
                    }
                    bestTarget = animal;
                }
            }
        }

        foreach (MontureController saddle in InteractionManager2.Instance.m_Saddles)
        {
            if (saddle != null)
            {
                Vector3 posSaddle = saddle.GetSaddlePosition();
                float tempDistance = Vector3.Distance(posSaddle, pos);
                saddle.SetOutlineVisibility(false);

                if (tempDistance < distance2)
                {
                    distance2 = tempDistance;
                    if (humanController != null)
                    {
                        humanController._saddleText.gameObject.SetActive(false);

                        if (distance2 < humanController.radiusSaddle)
                        {
                            saddle.SetOutlineVisibility(true);
                            if(saddle.IsReadyToMount())
                            {
                                humanController._saddleText.text = "MOUNT";
                            }

                            humanController._saddleText.gameObject.SetActive(humanController._currentMount == null);
                            humanController._saddleText.gameObject.transform.position = saddle.transform.position;
                        }
                    }
                    if(bestSaddle != null)
                    {
                        bestSaddle.SetOutlineVisibility(false);
                    }
                    bestSaddle = saddle;
                }
            }
        }
    }
}