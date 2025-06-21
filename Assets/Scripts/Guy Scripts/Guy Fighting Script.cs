using UnityEngine;
using UnityEngine.InputSystem;

public class GuyFightingScript : MonoBehaviour
{
    private Animator animator;
    private const float COMBO_TIMEOUT = 2f;

    private float comboTimer = 0f;
    private int comboStep = 0; // 0: idle, 1-6: combo ad�mlar�
    private int input = 0;     // 1 = sol t�k, 2 = sa� t�k

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // E�er animasyon bitti ise input ve combo ad�mlar�n� s�f�rla
        if (comboStep > 0 && stateInfo.normalizedTime >= 1f && !stateInfo.loop)
        {
            ResetCombo();
        }
        // Combo zamanlay�c�s�n� g�ncelle
        if (comboStep > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > COMBO_TIMEOUT)
            {
                ResetCombo();
            }
        }

        // Input alma: input 0 ise yeni input alabiliriz
        if (input == 0)
        {
            if (Input.GetMouseButtonDown(0)) input = 1; // sol t�k
            else if (Input.GetMouseButtonDown(1)) input = 2; // sa� t�k

            if (input != 0)
            {
                comboStep++;
                comboTimer = 0f;

                animator.SetInteger("input", input);
                animator.SetInteger("comboStep", comboStep);

            }
        }
    }

    // Animasyon Event'ten �a�r�l�r
    public void ClearInput()
    {
        input = 0;
        animator.SetInteger("input", 0);
    }

    public void ResetCombo()
    {
        comboStep = 0;
        input = 0;
        comboTimer = 0f;

        animator.SetInteger("comboStep", 0);
        animator.SetInteger("input", 0);
    }

}
