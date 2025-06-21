using UnityEngine;

public class CameraLockToEnemyScript : MonoBehaviour
{
    private GameObject player;
    private bool isLockedToEnemy = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        isLockedToEnemy = player.GetComponent<GuyFightingAnimationScript>().isLockedToEnemy;
        if (isLockedToEnemy)
        {
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
            {
                transform.LookAt(enemy.transform.position + new Vector3(0, 1.5f, 0)); // Adjust height as needed
            }
        }
        else
        {
            // Reset camera rotation or handle other logic when not locked to an enemy
            transform.rotation = Quaternion.identity; // Example: reset to default rotation
        }
    }
}
