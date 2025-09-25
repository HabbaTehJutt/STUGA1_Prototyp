using UnityEngine;

public class FleeFromPlayerSimple : MonoBehaviour
{
    public Transform player;             // Referenz auf den Spieler
    public float fleeDistance = 5f;      // Abstand, ab dem der NPC schnell flieht
    public float maxSpeed = 5f;          // Geschwindigkeit, wenn nah am Spieler
    public float minSpeed = 1f;          // Geschwindigkeit, wenn weit weg
    public float slowDownDistance = 15f; // Abstand, ab dem der NPC langsamer wird

    void Update()
    {
        if (player == null) return;

        // Richtung weg vom Spieler berechnen
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
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

        // Bewegung ausführen
        transform.Translate(directionAwayFromPlayer * speed * Time.deltaTime, Space.World);

        // Optional: NPC in Bewegungsrichtung drehen
        if (directionAwayFromPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionAwayFromPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }
}
