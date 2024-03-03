using UnityEngine;

public class InteractionComponent2 : MonoBehaviour
{
     public GameObject bestTarget = null;
     public MontureController bestSaddle = null;

    private void Update()
    {
        float distance = Mathf.Infinity;
        float distance2 = Mathf.Infinity;
        Vector3 pos = transform.position;

        foreach (GameObject animal in InteractionManager2.Instance.m_Animals)
        {
            if (animal != null)
            {
                Vector3 posAnimal = animal.transform.position;
                float tempDistance = Vector3.Distance(posAnimal, pos);

                if(tempDistance < distance)
                {
                    distance = tempDistance;
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

                if (tempDistance < distance2)
                {
                    distance2 = tempDistance;
                    bestSaddle = saddle;
                }
            }
        }
    }
}