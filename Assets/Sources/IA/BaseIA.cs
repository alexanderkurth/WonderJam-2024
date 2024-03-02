using UnityEngine;
using Random = UnityEngine.Random;

public class BaseIA : MonoBehaviour
{
    public AnimalDataInfo animalInfo;
    public float wanderAngleModifier = 8.0f;
    public Transform wanderTarget;
    
    private float nextAngle = 0;
    private float speed;

    private void Awake()
    {
        speed = Random.Range(animalInfo.minSpeed, animalInfo.maxSpeed);
    }

    private void Wander()
    {
        nextAngle += Random.Range(-wanderAngleModifier, wanderAngleModifier);
        if (Mathf.Abs(nextAngle) > 90)
        {
            nextAngle /= 10;
        }

        transform.RotateAround(transform.position, Vector3.up, nextAngle * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, wanderTarget.position, Time.deltaTime * speed);
    }
}
