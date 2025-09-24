using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    [Header("Kameras")]
    public Camera fpsCamera;       // normale Ego-Kamera
    public Camera selfieCamera;    // Selfie-Kamera

    [Header("Spieler-Referenz")]
    public Transform player;       // das Spielerobjekt (Root Transform)

    [Header("Selfie Einstellungen")]
    public Vector3 selfieOffset = new Vector3(0f, 1.6f, -1.5f); // Position vor Spieler
    public KeyCode toggleKey = KeyCode.C; // Taste zum Umschalten

    private bool selfieActive = false;

    void Start()
    {
        // Sicherheit: nur FPS-Kamera aktiv zu Beginn
        fpsCamera.enabled = true;
        selfieCamera.enabled = false;
    }

    void Update()
    {
        // Kamera umschalten
        if (Input.GetKeyDown(toggleKey))
        {
            selfieActive = !selfieActive;

            fpsCamera.enabled = !selfieActive;
            selfieCamera.enabled = selfieActive;
        }

        // Selfie-Kamera ausrichten
        if (selfieActive && player != null)
        {
            // Positioniere die Kamera relativ zum Spieler
            Vector3 targetPos = player.position + player.TransformDirection(selfieOffset);
            selfieCamera.transform.position = targetPos;

            // Schaue den Spieler an (leicht auf Kopf-Höhe)
            Vector3 lookTarget = player.position + Vector3.up * 1.5f;
            selfieCamera.transform.LookAt(lookTarget);
        }
    }
}
