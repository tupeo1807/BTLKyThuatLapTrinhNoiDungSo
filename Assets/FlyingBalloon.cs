using UnityEngine;

public class FlyingBalloon : MonoBehaviour
{
    public float speed = 5f;
    public float detectionRadius = 10f;
    public float changeDirectionInterval = 5f;
    public float rotationSpeed = 5f;

    private Vector3 targetDirection;
    private float timeSinceLastDirectionChange;

    private void Start()
    {
        targetDirection = RandomDirection();
        timeSinceLastDirectionChange = 0f;
    }

    private void Update()
    {
        // Change direction if the time since the last change is greater than the change direction interval
        timeSinceLastDirectionChange += Time.deltaTime;
        if (timeSinceLastDirectionChange >= changeDirectionInterval)
        {
            targetDirection = RandomDirection();
            timeSinceLastDirectionChange = 0f;
        }

        // Move the balloon in the target direction
        transform.position += targetDirection * speed * Time.deltaTime;

        // Rotate the balloon to face the target direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private Vector3 RandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}