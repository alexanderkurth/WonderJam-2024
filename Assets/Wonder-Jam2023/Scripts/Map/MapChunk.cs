using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChunk : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_Renderer;

    public float GetGroundMeshLenght()
    {
        return m_Renderer.bounds.size.z;
    }

    //void OnDrawGizmosSelected()
    //{
    //    // Draw a semitransparent red cube at the transforms position
    //    Gizmos.color = new Color(1, 0, 0, 0.5f);
    //    Gizmos.DrawCube(transform.position, new Vector3(m_ChunkLenght, 1, m_ChunkWidth));
    //}
}
