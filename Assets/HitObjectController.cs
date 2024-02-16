using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HitObjectController : MonoBehaviour
{
    public GameObject hitParticle;
    private GameManager gameManager;
    private float originalTimeScale;
    public float amplitude;
    public float frequency;

    void Start()
    {
        originalTimeScale = Time.timeScale;
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if(collider.GetComponent<PlayerController>() != null)
            {
                collider.GetComponent<PlayerController>().ReceiveDamage();
            } else 
            {
                collider.GetComponent<Player2Controller>().ReceiveDamage();
            }
        }

        gameManager.ShakeCamera(amplitude, frequency);

        GameObject newParticle = Instantiate(hitParticle, transform.position, Quaternion.identity);
        Destroy(newParticle, 0.5f);

        Time.timeScale = 0f;

        StartCoroutine(ResumeTimeAfterDelay(0.25f));
    }

    IEnumerator ResumeTimeAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = originalTimeScale;
    }
}
