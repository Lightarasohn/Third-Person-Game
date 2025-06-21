using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class NewMonoBehaviourScript : MonoBehaviour
{
    private Animator animator;
    private int isWalkingHash;
    public bool isStance;
    private bool isLockedToEnemy = false;
    private Rigidbody rb;

    private Transform cameraTransform;
    private float targetRotation = 0f;
    private float currentRotation = 0f;

    public float rotationSpeed = 10f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheckTransform;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        animator.updateMode = AnimatorUpdateMode.Fixed;
        isWalkingHash = Animator.StringToHash("isWalking");

        cameraTransform = Camera.main?.transform ?? GameObject.FindGameObjectWithTag("MainCamera")?.transform;
        currentRotation = transform.eulerAngles.y;
    }

     void Update()
     {
        isStance = animator.GetBool("isStance");
        isLockedToEnemy = animator.GetBool("isLockedToEnemy");
    }

    private void FixedUpdate()
    {
        HandleInput();

        ApplyGravityIfNeeded();
        
    }

    private void HandleInput()
    {
        Vector2 input = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) input.y += 1;
        if (Input.GetKey(KeyCode.S)) input.y -= 1;
        if (Input.GetKey(KeyCode.D)) input.x += 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;

        bool sprint = Input.GetKey(KeyCode.LeftShift);

        // isLockedToEnemy ve isStance modundayken boxing stepler ile hareket
        if (input != Vector2.zero)
        {
            if (isLockedToEnemy && isStance)
            {
                // Normal yürüme/koşma animasyonlarını kapat
                animator.SetBool(isWalkingHash, false);
                animator.SetBool("isSprinting", false);

                // Düşmana dönük ol
                GameObject fightingScriptObj = GetComponent<GuyFightingScript>()?.gameObject;
                if (fightingScriptObj != null)
                {
                    GuyFightingScript fightingScript = fightingScriptObj.GetComponent<GuyFightingScript>();
                    if (fightingScript != null && fightingScript.lockedEnemy != null)
                    {
                        Vector3 dir = fightingScript.lockedEnemy.transform.position - transform.position;
                        dir.y = 0;
                        if (dir.sqrMagnitude > 0.01f)
                        {
                            targetRotation = Quaternion.LookRotation(dir).eulerAngles.y;
                        }
                    }
                }
            }
            else
            {
                if (sprint)
                    animator.SetBool("isSprinting", true);
                else
                {
                    animator.SetBool(isWalkingHash, true);
                    animator.SetBool("isSprinting", false);
                }

                float inputAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
                float cameraY = cameraTransform?.eulerAngles.y ?? 0f;
                targetRotation = cameraY + inputAngle;
            }
        }
        else
        {
            animator.SetBool(isWalkingHash, false);
        }
    }

    private void OnAnimatorMove()
    {
        if (animator && animator.applyRootMotion)
        {
            // Boxing modunda da root motion uygula
            Vector3 velocity = animator.deltaPosition / Time.fixedDeltaTime;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

            // Dönüşü yumuşak şekilde uygula
            currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(Quaternion.Euler(0f, currentRotation, 0f));
        }
    }

    private void ApplyGravityIfNeeded()
    {
        if (!IsGrounded())
        {
            rb.AddForce(Physics.gravity, ForceMode.Acceleration); // Elle yerçekimi uygula
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
    }
}
