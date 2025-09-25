using UnityEngine;

public class NPCFlee : MonoBehaviour
{
    [Header("Target & Movement")]
    public Transform player;       // Player Transform
    public float speed = 3f;       // Laufgeschwindigkeit
    public float safeDistance = 5f; // Distanz, ab der er flieht
    public float wanderRadius = 10f; // Radius für zufällige Bewegung
    public float wanderSpeed = 2f;   // Geschwindigkeit beim zufälligen Wandern
    public float wanderChangeInterval = 3f; // Sekunden bis neue Zielrichtung

    private Vector3 wanderTarget;
    private float wanderTimer;

    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.applyRootMotion = false; // Root Motion deaktivieren

        SetNewWanderTarget();
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = transform.position - player.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance < safeDistance)
        {
            // Vor Player fliehen
            Vector3 move = direction.normalized * speed * Time.deltaTime;
            transform.position += move;

            if (move != Vector3.zero)
                transform.forward = move;

            // Optional: Animation auf Lauf setzen
            if (animator != null)
                animator.SetFloat("Speed", speed);
        }
        else
        {
            // Zufällige Wanderbewegung
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderChangeInterval)
            {
                SetNewWanderTarget();
                wanderTimer = 0f;
            }

            Vector3 wanderDir = (wanderTarget - transform.position);
            wanderDir.y = 0;

            if (wanderDir.magnitude > 0.1f)
            {
                Vector3 move = wanderDir.normalized * wanderSpeed * Time.deltaTime;
                transform.position += move;

                transform.forward = move;

                if (animator != null)
                    animator.SetFloat("Speed", wanderSpeed);
            }
            else
            {
                if (animator != null)
                    animator.SetFloat("Speed", 0f);
            }
        }
    }

    void SetNewWanderTarget()
    {
        // Zufällige Position innerhalb Radius
        Vector2 rand = Random.insideUnitCircle * wanderRadius;
        wanderTarget = transform.position + new Vector3(rand.x, 0, rand.y);
    }
}
