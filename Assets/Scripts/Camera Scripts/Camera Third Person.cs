using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraThirdPerson : MonoBehaviour
{
    private GameObject player = null;
    private GuyFightingScript guyFightingScript = null;
    private bool isLockedToEnemy = false;
    private GameObject lockedEnemy = null;
    private float yaw = 0f;
    private float pitch = 15f;
    private Vector3 currentCameraPosition;
    private Vector3 currentLookAtPosition;
    private Vector3 velocity = Vector3.zero;
    private Vector3 lookAtVelocity = Vector3.zero;
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float distance = 3.05f;
    [SerializeField] private float height = 1.84f;
    [SerializeField] private float lockedHeightOffset = 0.20f;
    [SerializeField] private float lockedPositionOffset = 0.20f;
    [SerializeField] private float smoothTime = 0.15f; // PID benzeri yumu�atma i�in
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        guyFightingScript = player.GetComponent<GuyFightingScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        guyFightingScript = player.GetComponent<GuyFightingScript>();
        // Ba�lang�� pozisyonlar�n� ayarla
        currentCameraPosition = transform.position;
        currentLookAtPosition = player.transform.position + new Vector3(0, height, 0);
    }

    // Update is called once per frame
    void Update()
    {
        isLockedToEnemy = guyFightingScript.isLockedToEnemy;
        lockedEnemy = guyFightingScript.lockedEnemy;

        ReadMouseData();
        TurnCamera();
    }

    private void ReadMouseData()
    {
        if (player == null) return;

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
    }

    private void TurnCamera()
    {
        if (!isLockedToEnemy)
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            Vector3 offset = rotation * new Vector3(0, 0, -distance) + new Vector3(0, height, 0);
            Vector3 targetPosition = player.transform.position + offset;
            Vector3 targetLookAt = player.transform.position + new Vector3(0, height, 0);

            // Pozisyonu ve bak�� noktas�n� yumu�ak ge�i�le ayarla
            currentCameraPosition = Vector3.SmoothDamp(currentCameraPosition, targetPosition, ref velocity, smoothTime);
            currentLookAtPosition = Vector3.SmoothDamp(currentLookAtPosition, targetLookAt, ref lookAtVelocity, smoothTime);

            transform.position = currentCameraPosition;
            transform.LookAt(currentLookAtPosition);
        }
        else if (lockedEnemy != null)
        {
            Quaternion rotation = Quaternion.LookRotation(lockedEnemy.transform.position - player.transform.position);
            Vector3 offset = rotation * new Vector3(lockedPositionOffset, 0, -distance) + new Vector3(0, height + lockedHeightOffset, 0);
            Vector3 targetPosition = player.transform.position + offset;
            Vector3 targetLookAt = lockedEnemy.transform.position + new Vector3(0, height, 0);

            // Pozisyonu ve bak�� noktas�n� yumu�ak ge�i�le ayarla
            currentCameraPosition = Vector3.SmoothDamp(currentCameraPosition, targetPosition, ref velocity, smoothTime);
            currentLookAtPosition = Vector3.SmoothDamp(currentLookAtPosition, targetLookAt, ref lookAtVelocity, smoothTime);

            transform.position = currentCameraPosition;
            transform.LookAt(currentLookAtPosition);
        }
    }
}
