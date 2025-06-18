using UnityEngine;
using UnityEngine.InputSystem;

public class CameraThirdPerson : MonoBehaviour
{
    private Transform playerTransform = null;
    // S�n�f�n ba��na ekleyin:
    private float yaw = 0f;
    private float pitch = 15f;
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float distance = 3.05f;
    [SerializeField] private float height = 1.84f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null) return;

        // Mouse input
        if (Mouse.current != null && Mouse.current.delta != null)
        {
            float mouseX = Mouse.current.delta.x.ReadValue();
            float mouseY = Mouse.current.delta.y.ReadValue();

            // Kamera a��s�n� g�ncelle
            yaw += mouseX * sensitivity;
            pitch -= mouseY * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // Kameran�n konumunu ve rotas�n� g�ncelle
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance) + new Vector3(0, height, 0);
        transform.position = playerTransform.position + offset;
        transform.LookAt(playerTransform.position + new Vector3(0, height, 0));
    }
}
