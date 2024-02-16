using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    //UI logic
    public TextMeshProUGUI winText;
    public TextMeshProUGUI submitText;
    private bool gameEnded = false;

    public TextMeshProUGUI timerText;
    public float timeRemaining = 180;

    public TextMeshProUGUI[] texts;

    private AudioSource audioSource;
    public AudioClip fightSound;
    
    // Start is called before the first frame update
    void Start()
    {
        winText.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        
        if (cinemachineVirtualCamera != null)
        {
            virtualCameraNoise = cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        }

        StartCoroutine(StartTheGame());
    }

    IEnumerator StartTheGame()
    {
        yield return new WaitForSeconds(1);
        texts[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(1);

        texts[1].gameObject.SetActive(true);
        texts[0].gameObject.SetActive(false);
        yield return new WaitForSeconds(1);

        texts[2].gameObject.SetActive(true);
        texts[1].gameObject.SetActive(false);
        yield return new WaitForSeconds(1);

        texts[3].gameObject.SetActive(true);
        texts[2].gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        
        texts[3].gameObject.SetActive(false);
        audioSource.PlayOneShot(fightSound);

        PlayerController[] players = FindObjectsOfType<PlayerController>();

        foreach (PlayerController player in players)
        {
            player.gameStarted = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameEnded)
        {
            if (timeRemaining > 0)
            {   
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                SceneManager.LoadScene("FinalPunch");
                timeRemaining = 0;
            }
        } else 
        {
            submitText.gameObject.SetActive(true);
            
            if(Input.GetButtonDown("Submit"))
            {
                SceneManager.LoadScene("MainGame");
            }
        }
        
        timerText.text = ((int)timeRemaining).ToString();

        if (virtualCameraNoise != null && virtualCameraNoise.m_AmplitudeGain > 0)
        {
            virtualCameraNoise.m_AmplitudeGain -= Time.deltaTime;
        }
    }

    public void PlayerWins(int player)
    {
        winText.text = "Player " + player + " WINS!";
        StartCoroutine(PlayerWinsTextAppear());
    }

    IEnumerator PlayerWinsTextAppear()
    {
        yield return new WaitForSecondsRealtime(3);
        
        winText.gameObject.SetActive(true);
        gameEnded = true;
    }

    public void ShakeCamera(float amplitude, float frequency)
    {
        virtualCameraNoise.m_AmplitudeGain = amplitude;
        virtualCameraNoise.m_FrequencyGain = frequency;
    }
}
