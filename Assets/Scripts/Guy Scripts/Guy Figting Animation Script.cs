using System.Collections;
using UnityEngine;

public class GuyFightingAnimationScript : MonoBehaviour
{
    private Animator animator;
    private int input = 0;
    private bool isStance = false;
    public bool isLockedToEnemy = false;
    private GameObject mainCamera;
    private Camera mainCameraComponent;
    public GameObject lockedEnemy = null;
    [SerializeField] private float lockAngleNegativeOffset = 0f;

    public float lockRange = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCameraComponent = mainCamera.GetComponent<Camera>();
    }

    private bool IsEnemyBehindTheWall(GameObject enemy)
    {
        if (enemy == null) return false;
        RaycastHit hit;
        Vector3 directionToEnemy = enemy.transform.position - transform.position;
        if (Physics.Raycast(transform.position, directionToEnemy.normalized, out hit, lockRange))
        {
            if (hit.collider.gameObject != enemy) // If the ray hits something other than the enemy
            {
                return true; // Enemy is behind a wall
            }
        }
        return false; // Enemy is not behind a wall
    }

    private bool IsPlayerLookingAtEnemy(GameObject enemy)
    {
        if (enemy == null) return false;
        Vector3 directionToEnemy = enemy.transform.position - transform.position;
        directionToEnemy.y = 0; // Ignore vertical difference
        Vector3 forward = mainCamera.transform.forward;
        float angle = Vector3.Angle(forward, directionToEnemy.normalized);
        return angle < mainCameraComponent.fieldOfView - lockAngleNegativeOffset; // Adjust the angle threshold as needed
    }

    private GameObject LockToEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return null;

        GameObject closestEnemy = null;
        float closestDistance = lockRange;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && IsPlayerLookingAtEnemy(enemy) && !IsEnemyBehindTheWall(enemy))
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void HandleInput()
    {
        isStance = animator.GetBool("isStance");
        isLockedToEnemy = animator.GetBool("isLockedToEnemy");
        input = 0;
        if (Input.GetKeyUp(KeyCode.F)) animator.SetBool("isStance", !isStance);
        if (Input.GetKeyUp(KeyCode.Q))
        {
            lockedEnemy = LockToEnemy();
            if (lockedEnemy != null && isLockedToEnemy)
                animator.SetBool("isLockedToEnemy", false);
            else if (lockedEnemy != null && !isLockedToEnemy)
                animator.SetBool("isLockedToEnemy", true);
            else if (lockedEnemy == null && isLockedToEnemy)
                animator.SetBool("isLockedToEnemy", false);
            else if (lockedEnemy == null && !isLockedToEnemy)
                animator.SetBool("isLockedToEnemy", false);
            else
                animator.SetBool("isLockedToEnemy", false);
        }
        if (Input.GetKey(KeyCode.W)) input = 1;
        if (Input.GetKey(KeyCode.S)) input = 2;
        if (Input.GetKey(KeyCode.A)) input = 3;
        if (Input.GetKey(KeyCode.D)) input = 4;
    }

    private void HandleAnimation()
    {
        // Tüm adým bool'larýný baþta false yap
        animator.SetBool("boxingForwardStep", false);
        animator.SetBool("boxingBackwardStep", false);
        animator.SetBool("boxingLeftStep", false);
        animator.SetBool("boxingRightStep", false);

        if (input != 0 && isStance)
        {
            if (isLockedToEnemy)
            {
                switch (input)
                {
                    case 1:
                        animator.SetBool("boxingForwardStep", true);
                        break;
                    case 2:
                        animator.SetBool("boxingBackwardStep", true);
                        break;
                    case 3:
                        animator.SetBool("boxingLeftStep", true);
                        break;
                    case 4:
                        animator.SetBool("boxingRightStep", true);
                        break;
                }
            }
            else
            {
                animator.SetBool("boxingForwardStep", true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleAnimation();
    }
}
