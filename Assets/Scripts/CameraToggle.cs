using UnityEngine;
using UnityEngine.UI;   
using System.Collections;

public class CameraToggle : MonoBehaviour
{
    [Header("Kameras")]
    public Camera fpsCamera;
    public Camera selfieCamera;

    [Header("Spieler-Referenz")]
    public Transform player;

    [Header("Selfie Einstellungen")]
    public Vector3 selfieOffset = new Vector3(0f, 1.6f, -1.5f);
    public KeyCode toggleKey = KeyCode.F;

    [Header("UI")]
    public Image selfieUIImage; // <- dieses Feld erscheint im Inspector!
    public Image photoPolaroid;

    [Header("Foto Einstellungen")]
    public KeyCode takePhotoKey = KeyCode.Space;
    public float photoDuration = 1f;
    public int screenshotWidth = 600;
    public int screenshotHeight = 900;
    public float displayScale = 0.33f;
    
    private bool selfieActive = false;
    private FirstPersonController playerMovementScript;

    void Start()
    {
        if (fpsCamera) fpsCamera.enabled = true;
        if (selfieCamera) selfieCamera.enabled = false;

        if (selfieUIImage != null)
            selfieUIImage.gameObject.SetActive(false);

        if (photoPolaroid != null)
            photoPolaroid.gameObject.SetActive(false);

        if (player != null)
        {
            playerMovementScript = player.GetComponent<FirstPersonController>();
            if (playerMovementScript == null)
                Debug.LogWarning("CameraToggle: FirstPersonController nicht auf Player gefunden");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            selfieActive = !selfieActive;

            if (fpsCamera) fpsCamera.enabled = !selfieActive;
            if (selfieCamera) selfieCamera.enabled = selfieActive;

            if (selfieUIImage != null)
                selfieUIImage.gameObject.SetActive(selfieActive);

            if (playerMovementScript != null)
                playerMovementScript.enabled = !selfieActive;
        }

        if (selfieActive && player != null && selfieCamera != null)
        {
            Vector3 targetPos = player.position + player.TransformDirection(selfieOffset);
            selfieCamera.transform.position = targetPos;

            Vector3 lookTarget = player.position + Vector3.up * 1f;
            selfieCamera.transform.LookAt(lookTarget);
        }

        if (selfieActive && Input.GetKeyDown (takePhotoKey))
        {
            if (photoPolaroid != null)
                StartCoroutine(TakeScreenshot());
        }
    }

    private IEnumerator TakeScreenshot()
    {
         // RenderTexture vorbereiten
        RenderTexture rt = new RenderTexture(screenshotWidth, screenshotHeight, 24);
        selfieCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.RGB24, false);

        // Rendern
        selfieCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
        screenShot.Apply();

        selfieCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Als Sprite ins UI
        Sprite sprite = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), new Vector2(0.5f, 0.5f));
        photoPolaroid.sprite = sprite;

        float displayWidth = screenshotWidth * displayScale;
        float displayHeight = screenshotHeight * displayScale;
        photoPolaroid.rectTransform.sizeDelta = new Vector2(screenshotWidth, screenshotHeight);
        photoPolaroid.preserveAspect = true;

        // Polaroid kurz anzeigen
        photoPolaroid.gameObject.SetActive(true);
        yield return new WaitForSeconds(photoDuration);
        photoPolaroid.gameObject.SetActive(false);
    }
}
