using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player2Controller : MonoBehaviour
{
    public float speed = 10.0f;
    public float jumpForce = 2.0f;

    private Rigidbody rb;

    private Animator animator;

    private bool isJumping = false;
    private bool isWalking;
    private bool isAttacking = false;
    private bool isBlocking = false;

    // Player colliders logic

    public GameObject hitLocation;
    public GameObject hitObject;

    // Player HP logic

    private int maxHP = 200;
    public int currentHP;
    private bool isDead = false;

    // UI logic

    public Slider healthBar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentHP = maxHP;
        healthBar.maxValue = maxHP;
    }

    void Update()
    {
        //PlayerMovement();
        //PlayerCombat();
        UpdateUI();
    }

    void PlayerMovement()
    {
        if(!isAttacking && !isBlocking)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            rb.velocity = movement * speed;

            if (moveHorizontal > 0)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                animator.SetBool("isWalking", true);
            }
            else if (moveHorizontal < 0)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
                animator.SetBool("isWalking", true);
            }
            else if (moveHorizontal == 0)
            {
                animator.SetBool("isWalking", false);
            }

            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
                isJumping = true;
            }
        }
    }

    void PlayerCombat()
    {
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            animator.SetTrigger("Punch");
            isAttacking = true;
            //Debug.Log("Punch");
        }

        if (Input.GetButtonDown("Fire2") && !isAttacking)
        {
            animator.SetTrigger("Kick");
            isAttacking = true;
            //Debug.Log("Kick");
        }

        if (Input.GetKey(KeyCode.Mouse2) && !isAttacking && !isBlocking)
        {
            animator.SetBool("Block", true);
            isBlocking = true;
        }
        else if (!Input.GetKey(KeyCode.Mouse2) && isBlocking)
        {
            animator.SetBool("Block", false);
            StartCoroutine(BlockingCooldown());
        }
    }
    
    IEnumerator BlockingCooldown()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        isBlocking = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }

    public void DealDamage()
    {
        GameObject newHitObject = Instantiate(hitObject, hitLocation.transform.position, Quaternion.identity);

        Destroy(newHitObject, 0.1f);
    }

    public void ReceiveDamage()
    {
        if(!isBlocking)
        {
            currentHP -= 10;

            isAttacking = true;

            if(currentHP <= 0)
            {
                Die();
            } else {
                animator.SetTrigger("Hit");
            }
        }
    }

    void Die()
    {
        isAttacking = true;
        animator.SetTrigger("Die");
    }

    void UpdateUI()
    {
        healthBar.value = currentHP;
    }
}