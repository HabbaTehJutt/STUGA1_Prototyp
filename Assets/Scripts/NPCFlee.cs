using UnityEngine;

public class NPCFlee : MonoBehaviour
{
    public Transform player;     // Player Transform
    public float speed = 4f;     // Bewegungsgeschwindigkeit
    public float safeDistance = 6f; // ab dieser Distanz hört er auf zu fliehen

    void Update()
    {
        if (player == null) return;

        // Richtung vom Player weg
        Vector3 direction = transform.position - player.position;
        direction.y = 0; // nur horizontal fliehen

        float distance = direction.magnitude;

        if (distance < safeDistance)
        {
            // Normalisieren und bewegen
            Vector3 move = direction.normalized * speed * Time.deltaTime;
            transform.position += move;

            // NPC schaut in Fluchtrichtung
            if (move != Vector3.zero)
                transform.forward = move;
        }
    }
}
