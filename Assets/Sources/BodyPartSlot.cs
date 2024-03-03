using System;
using game;
using UnityEngine;

public class BodyPartSlot : MonoBehaviour
{
    public BodyPartType BodyPartType = BodyPartType.Head;

    public GameObject m_InstantiatedPart = null;

    public bool HasBodyPart()
    {
        return m_InstantiatedPart != null;
    }

    public GameObject GetBodyPart()
    {
        return m_InstantiatedPart;
    }

    public void SetBodyPart(AnimalDatas data, AnimalType type)
    {
        // Prevent eventual editor attachments
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
        if (m_InstantiatedPart != null)
        {
            Destroy(m_InstantiatedPart);
        }

        AnimalDataInfo info = data.GetAnimalInfoByType(type);

        m_InstantiatedPart = Instantiate(info.GetBodyPartTemplate(BodyPartType), transform);
        m_InstantiatedPart.transform.localPosition = Vector3.zero;
    }

    public void AttachToBody(BodyScript body)
    {
        Debug.Assert(m_InstantiatedPart != null, "No body part instantiated in " + gameObject.name);
        LegController legController;
        if (m_InstantiatedPart.TryGetComponent(out legController)) // Leg can be null if the body part is not a leg
        {
            m_InstantiatedPart.transform.localPosition = legController.GetPivotLocation() * (legController.IsLeft ? 1 : -1);
            legController.SetBody(body);
        }
        else
        {
            HeadController headController;
            if (m_InstantiatedPart.TryGetComponent(out headController)) // Head can be null if the body part is not a head
            {
                m_InstantiatedPart.transform.localPosition = - headController.GetPivotLocation();
                headController.SetBody(body.GetComponent<Rigidbody2D>());
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f); 
    }
}
