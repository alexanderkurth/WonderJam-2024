using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField]
    private GameObject m_EnemyPrefab;
    [SerializeField]
    private GameObject m_TargetPrefab;
    [SerializeField]
    private Transform m_PivotTransform;
    [SerializeField]
    public Vector2 m_DeltaTime;
    [SerializeField]
    private float m_SpawnAngle = 45.0f;
    [SerializeField]
    private int m_nbSpawnedPosition = 5;

    private int m_nbEnemy;
    private float m_TimeBeforeStart = 3.0f;

    // To be called by GameMode
    public void StartSpawn(int nbSpawnedEntity)
    {
        m_nbEnemy = nbSpawnedEntity;
        StartCoroutine(Spawn());
    }

    public void SetStartTimer(float time)
    {
        m_TimeBeforeStart = time;
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(m_TimeBeforeStart);

        yield return new WaitForSeconds(Random.Range(m_DeltaTime.x, m_DeltaTime.y));

        float AngleStep = m_SpawnAngle * 2.0f / (float)m_nbSpawnedPosition;

        for (int i = 1; i < m_nbEnemy; i++)
        {
            int position = Random.Range(0, m_nbSpawnedPosition);
            GameObject newSurrounderObject = Instantiate(m_EnemyPrefab, m_PivotTransform.position, Quaternion.identity, null);

            float angle = (AngleStep * position) - (m_SpawnAngle / 2.0f);

            newSurrounderObject.transform.RotateAround(transform.position, Vector3.up, angle);
            newSurrounderObject.transform.rotation = Quaternion.identity;
            EnemyMove enemy = newSurrounderObject.GetComponent<EnemyMove>();
            enemy.m_Target = m_TargetPrefab;

            yield return new WaitForSeconds(Random.Range(m_DeltaTime.x, m_DeltaTime.y));
        }
    }
}
