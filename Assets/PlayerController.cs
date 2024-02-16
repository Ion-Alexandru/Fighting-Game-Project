using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public string horizontalInput;
    public string verticalInput;
    public string fire1Input;
    public string fire2Input;
    public string fire3Input;
    public string fire4Input;
    
    public int currentPlayer;
    public float speed = 10.0f;
    public float jumpForce = 2.0f;

    private Rigidbody rb;

    private Animator animator;

    private bool isJumping = false;
    private bool isAttacking = false;
    private bool isBlocking = false;
    public bool gameStarted = false;

    // Player colliders logic
    public GameObject hitLocation;
    public GameObject hitObject;
    public int hitsReceived = 0;
    private int maxHits = 10;

    public GameObject spellLocation;
    public GameObject spellObject;

    // Player HP logic
    public int maxHP = 200;
    public int currentHP;

    // Audio logic
    private AudioSource audioSource;
    public AudioClip punchSound;
    public AudioClip kickSound;

    // UI logic
    public Slider healthBar;
    public Slider spellBar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentHP = maxHP;
        healthBar.maxValue = maxHP;

        spellBar.maxValue = maxHits;
    }

    void Update()
    {
        UpdateUI();

        if(!isAttacking && !isBlocking && gameStarted)
        {
            float moveHorizontal = Input.GetAxis(horizontalInput);
            float moveVertical = Input.GetAxis(verticalInput);

            UnityEngine.Vector3 movement = new UnityEngine.Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.velocity = new UnityEngine.Vector3(movement.x * speed, rb.velocity.y, movement.z * speed);
            //rb.AddForce(movement * speed);

            if (moveHorizontal > 0)
            {
                transform.rotation = UnityEngine.Quaternion.Euler(0, 90, 0);
                animator.SetBool("isWalking", true);
            }
            else if (moveHorizontal < 0)    
            {
                transform.rotation = UnityEngine.Quaternion.Euler(0, -90, 0);
                animator.SetBool("isWalking", true);
            }
            else if (moveHorizontal == 0)
            {
                animator.SetBool("isWalking", false);
            }

            if (!isJumping)
            {
                if(currentPlayer == 2 && moveVertical < 0)
                {
                    Debug.Log("Jump");
                    rb.AddForce(UnityEngine.Vector3.up * jumpForce, ForceMode.Impulse);
                    isJumping = true;
                } else 
                {
                    if(currentPlayer == 1 && moveVertical > 0)
                    {
                        Debug.Log("Jump");
                        rb.AddForce(UnityEngine.Vector3.up * jumpForce, ForceMode.Impulse);
                        isJumping = true;
                    }
                }
            }
        }

        if(gameStarted)
        {
            if (Input.GetButtonDown(fire4Input) && hitsReceived >= maxHits && !isAttacking && !isBlocking)
            {
                animator.SetTrigger("Spell");
                isAttacking = true;
                hitsReceived = 0;
                //Debug.Log("Spell");

                StartCoroutine(CastSpell());
            }

            if (Input.GetButtonDown(fire1Input) && !isAttacking && !isBlocking)
            {
                animator.SetTrigger("Punch");
                audioSource.PlayOneShot(punchSound);
                isAttacking = true;
                //Debug.Log("Punch");
            }

            if (Input.GetButtonDown(fire2Input) && !isAttacking && !isBlocking)
            {
                animator.SetTrigger("Kick");
                audioSource.PlayOneShot(kickSound);
                isAttacking = true;
                //Debug.Log("Kick");
            }

            if (Input.GetButton(fire3Input) && !isAttacking && !isBlocking)
            {
                animator.SetBool("Block", true);
                isBlocking = true;
            }
            else if (!Input.GetButton(fire3Input) && !isAttacking && isBlocking)
            {
                animator.SetBool("Block", false);
                StartCoroutine(BlockingCooldown());
            }
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
        GameObject newHitObject = Instantiate(hitObject, hitLocation.transform.position, UnityEngine.Quaternion.identity);

        Destroy(newHitObject, 0.1f);
    }

    IEnumerator CastSpell()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        
        GameObject newSpell = Instantiate(spellObject, spellLocation.transform.position, UnityEngine.Quaternion.identity);
        Rigidbody spellRB = newSpell.GetComponent<Rigidbody>();

        spellRB.AddForce(new UnityEngine.Vector3(this.transform.forward.x * 5f, 0, 0), ForceMode.Impulse);

        Destroy(newSpell, 5);
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
                hitsReceived ++;
            }
        } else 
        {
            audioSource.PlayOneShot(kickSound);
        }
    }

    void Die()
    {
        isAttacking = true;
        animator.SetTrigger("Die");

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if(currentPlayer == 1)
        {
            gameManager.PlayerWins(2);
        } else {
            gameManager.PlayerWins(1);
        }
    }

    void UpdateUI()
    {
        healthBar.value = currentHP;
        spellBar.value = hitsReceived;
    }
}