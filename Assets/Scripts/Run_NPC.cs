using UnityEngine;

public class Run_NPC : MonoBehaviour
{
    [Header("Referenzen")]
    public Transform Player;

    [Header("Einstellungen")]
    public float triggerDistance = 5f;       // Spieler nahe: Fluchtmodus
    public float maxDistance = 15f;          // Geschwindigkeit abhängig von Distanz
    public float minSpeed = 1f;              // Langsamste Fluchtgeschwindigkeit
    public float maxSpeed = 6f;              // Schnellste Fluchtgeschwindigkeit
    public float wanderSpeed = 2f;           // Geschwindigkeit beim Herumwandern
    public float wanderChangeTime = 3f;      // Alle X Sekunden neue Richtung
    public float obstacleAvoidanceRange = 2f;// SphereCast-Länge zum Hindernis erkennen
    public float stuckThreshold = 1f;        // Zeit, nach der neue Richtung gewählt wird, falls festhängt

    private Rigidbody rb;
    private float startY;
    private Vector3 wanderDir;
    private float wanderTimer;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody fehlt am NPC!");
            return;
        }

        startY = transform.position.y;
        PickRandomDirection();
        lastPosition = rb.position;
    }

    private void FixedUpdate()
    {
        if (Player == null || rb == null) return;

        float distance = Vector3.Distance(rb.position, Player.position);
        Vector3 moveDir;
        float speed;

        // === Fluchtmodus ===
        if (distance < triggerDistance)
        {
            moveDir = (rb.position - Player.position).normalized;
            float t = Mathf.InverseLerp(maxDistance, triggerDistance, distance);
            speed = Mathf.Lerp(maxSpeed, minSpeed, t);
        }
        // === Wander-Modus ===
        else
        {
            wanderTimer -= Time.fixedDeltaTime;
            if (wanderTimer <= 0f) PickRandomDirection();
            moveDir = wanderDir;
            speed = wanderSpeed;
        }

        // Intelligente Hindernisvermeidung
        moveDir = AvoidObstacles(moveDir);

        // Bewegung über Rigidbody
        Vector3 targetPos = rb.position + moveDir * speed * Time.fixedDeltaTime;
        targetPos.y = startY; // Höhe fixieren
        rb.MovePosition(targetPos);

        // Drehung in Bewegungsrichtung
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 5f * Time.fixedDeltaTime));
        }

        // === Prüfen, ob NPC festhängt ===
        if (Vector3.Distance(rb.position, lastPosition) < 0.01f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer > stuckThreshold)
            {
                PickRandomDirection();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = rb.position;
    }

        private Vector3 AvoidObstacles(Vector3 dir)
    {
        Vector3[] directions = new Vector3[]
        {
            dir,
            (dir + transform.right).normalized,
            (dir - transform.right).normalized,
            (dir + transform.forward).normalized,
            (dir - transform.forward).normalized,
            transform.right,
            -transform.right,
            -dir
        };

        foreach (var d in directions)
        {
            RaycastHit hit;
            if (!Physics.SphereCast(rb.position, 0.3f, d, out hit, obstacleAvoidanceRange))
            {
                return d; // freie Richtung gefunden
            }
        }

        // Notfall: zufällige Richtung
        return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    // Neue zufällige Wander-Richtung
    private void PickRandomDirection()
    {
        wanderDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        wanderTimer = wanderChangeTime;
    }
}
