using UnityEngine;

public class FleeFromPlayerSmooth : MonoBehaviour
{
    public Transform player;             // Referenz auf den Spieler
    public float fleeDistance = 5f;      // Abstand, ab dem der NPC schnell flieht
    public float maxSpeed = 5f;          // Geschwindigkeit, wenn nah am Spieler
    public float minSpeed = 1f;          // Geschwindigkeit, wenn weit weg
    public float slowDownDistance = 15f; // Abstand, ab dem der NPC langsamer wird
    public float obstacleCheckDistance = 1.5f; // Abstand für Hindernisprüfung
    public LayerMask obstacleLayers;     // Layer, die als Hindernisse gelten
    public float turnSmoothness = 5f;    // Wie schnell die Richtung sich anpasst

    private Vector3 currentDirection;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform nicht zugewiesen!");
        }
        currentDirection = (transform.position - player.position).normalized;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;

        // Geschwindigkeit abhängig vom Abstand berechnen
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
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

        Vector3 targetDirection = directionAwayFromPlayer;

        // Hindernisprüfung
        if (Physics.Raycast(transform.position, currentDirection, out RaycastHit hit, obstacleCheckDistance, obstacleLayers))
        {
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0;
            // Richtungsänderung sanft durch Lerp zur reflektierten Richtung
            targetDirection = Vector3.Reflect(currentDirection, hitNormal).normalized;
        }

        // Spieler-Seitencheck: leicht ausweichen
        Vector3 toPlayer = player.position - transform.position;
        float sideDot = Vector3.Dot(transform.right, toPlayer.normalized);
        if (Mathf.Abs(sideDot) > 0.7f && distanceToPlayer < fleeDistance * 2f)
        {
            Vector3 sideOffset = transform.right * (sideDot > 0 ? -1 : 1); // seitlich weg
            targetDirection = (targetDirection + sideOffset).normalized;
        }

        // Sanfte Richtungsänderung
        currentDirection = Vector3.Slerp(currentDirection, targetDirection, turnSmoothness * Time.deltaTime);

        // Bewegung ausführen
        transform.Translate(currentDirection * speed * Time.deltaTime, Space.World);

        // Drehung anpassen
        if (currentDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSmoothness * Time.deltaTime);
        }
    }
}
