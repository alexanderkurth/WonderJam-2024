using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    MapChunk[] m_AvailableChunks;

    [SerializeField]
    MapChunk m_SartingMapChunk;

    [SerializeField]
    MapChunk m_EndingMapChunk;

    [SerializeField]
    private int m_NbOfChunks;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 lastChunkPos = m_SartingMapChunk.transform.position;
        MapChunk lastChunk = m_SartingMapChunk;

        for (int i = 0; i < m_NbOfChunks; i++)
        {
            int nextChunkIndex = Random.Range(0, m_AvailableChunks.Length - 1);
            MapChunk nextChunk = m_AvailableChunks[nextChunkIndex];

            float currentChunkLenght = lastChunk.GetGroundMeshLenght();
            float nextChunkLenght = nextChunk.GetGroundMeshLenght();

            Vector3 nextChunkPosition = lastChunkPos;
            nextChunkPosition.z += (currentChunkLenght / 2.0f) + (nextChunkLenght / 2.0f);

            MapChunk newChunk = Instantiate(nextChunk, nextChunkPosition, Quaternion.identity);

            lastChunkPos = nextChunkPosition;
            lastChunk = newChunk;
        }

        //Finally spawn last chunk
        float beforeLastChunkLenght = lastChunk.GetGroundMeshLenght();
        float lastChunkLenght = m_EndingMapChunk.GetGroundMeshLenght();
        Vector3 chunkPos = lastChunkPos;
        chunkPos.z += (beforeLastChunkLenght / 2.0f) + (lastChunkLenght / 2.0f);

        Instantiate(m_EndingMapChunk, chunkPos, Quaternion.identity);
    }
}
