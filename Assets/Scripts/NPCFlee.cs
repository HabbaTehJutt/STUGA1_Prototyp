using UnityEngine;

public class FleeFromPlayerRandom : MonoBehaviour
{
    public Transform player;             
    public float fleeDistance = 5f;      
    public float maxSpeed = 5f;          
    public float minSpeed = 1f;          
    public float slowDownDistance = 15f; 

    public float directionChangeInterval = 2f; 
    public float maxRandomAngle = 45f;         

    public float obstacleDetectionDistance = 2f; // Abstand, um Hindernisse zu erkennen
    public LayerMask obstacleLayer;              // Layer für Hindernisse

    private Vector3 currentDirection;
    private float directionChangeTimer;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform nicht zugewiesen!");
        }
        currentDirection = (transform.position - player.position).normalized;
        directionChangeTimer = directionChangeInterval;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Geschwindigkeit abhängig vom Abstand berechnen
        float speed;
        if (distanceToPlayer < fleeDistance)
        {
            speed = maxSpeed;
        }
        else if (distanceToPlayer > slowDownDistance)
        {
            speed = minSpeed;
        }
        else
        {
            float t = (distanceToPlayer - fleeDistance) / (slowDownDistance - fleeDistance);
            speed = Mathf.Lerp(maxSpeed, minSpeed, t);
        }

        // Wenn Spieler zu nahe ist, direkt weg laufen
        if (distanceToPlayer < fleeDistance)
        {
            currentDirection = (transform.position - player.position).normalized;
        }
        else
        {
            // Timer für zufällige Richtungsänderung
            directionChangeTimer -= Time.deltaTime;
            if (directionChangeTimer <= 0f)
            {
                directionChangeTimer = directionChangeInterval;
                float randomAngle = Random.Range(-maxRandomAngle, maxRandomAngle);
                currentDirection = Quaternion.Euler(0, randomAngle, 0) * (transform.position - player.position).normalized;
            }
        }

        // Hindernisse erkennen
        if (Physics.Raycast(transform.position, currentDirection, out RaycastHit hit, obstacleDetectionDistance, obstacleLayer))
        {
            // Richtung anpassen, z.B. nach links oder rechts ausweichen
            Vector3 avoidDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;
            currentDirection = avoidDirection;
        }

        // Bewegung ausführen
        transform.Translate(currentDirection * speed * Time.deltaTime, Space.World);

        // Drehung des NPCs anpassen
        if (currentDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }
}
