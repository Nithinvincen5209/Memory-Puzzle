using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public List<Sprite> cardImages;
    public TextMeshProUGUI attemptText;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject matchSparkleEffect; 

    public int gridWidth = 4;
    public int gridHeight = 4;
    public float cardSpacing = 2f;

    private Card firstFlippedCard;
    private Card secondFlippedCard;
    private int wrongAttempts = 0;
    private int matchesFound = 0;
    private int totalMatches;
    private bool canFlip = true;
    public int attemptsAllowed = 5;

    private AudioSource AS;
    public AudioClip matchSfx;
    public AudioClip victorySfx;

    public TextMeshProUGUI timer;
    public TextMeshProUGUI timerText;
    public float timeRemaining = 120f;
    private bool timerIsRunning = false;



    void Start()
    {
        totalMatches = (gridWidth * gridHeight) / 2;
        SetUpGame();
        UpdateAttemptsUI();
        timerIsRunning = true;
        AS = GetComponent<AudioSource>();
        
    }
    private void Update()
    {
        if(timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                canFlip = false;
                if(matchesFound  != totalMatches)
                {
                    gameOverPanel.SetActive(true);
                    Time.timeScale = 0f;
                }
               
            }
        }
    }

    private void DisplayTime(float timeRemaining)
    {
        timeRemaining += 0.99f;

        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void SetUpGame()
    {
        List<int> cardIDs = new List<int>();
        for (int i = 0; i < totalMatches; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }
        cardIDs = cardIDs.OrderBy(x => Random.value).ToList();

        float StartX = -(gridWidth - 1) * cardSpacing / 2;
        float StartY = (gridHeight - 1) * cardSpacing / 2;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int index = y * gridWidth + x;
                GameObject CarObj = Instantiate(cardPrefab, new Vector3(StartX + x * cardSpacing, StartY - y * cardSpacing, 0), Quaternion.identity);
                Card card = CarObj.GetComponent<Card>();

                int cardID = cardIDs[index];
                Sprite cardImage = cardImages[cardID];
                card.SetUp(cardID, cardImage, this);
            }
            
        }
    }

    public bool CanFlip()
    {
        return canFlip;
    }

    public void CardFlipped(Card card)
    {
        if (firstFlippedCard == null)
        {
            firstFlippedCard = card;
        }
        else
        {
            secondFlippedCard = card;
            canFlip = false;
            StartCoroutine(CheckForMatch());
        }
    }

    
    IEnumerator CheckForMatch()
    {
        
        yield return new WaitForSeconds(2f);

        if (firstFlippedCard.GetCardID() == secondFlippedCard.GetCardID())
        {
            
            firstFlippedCard.SetMatched();
            secondFlippedCard.SetMatched();
            matchesFound++;

            
            Instantiate(matchSparkleEffect, firstFlippedCard.transform.position, Quaternion.identity);
            Instantiate(matchSparkleEffect, secondFlippedCard.transform.position, Quaternion.identity);
            AS.PlayOneShot(matchSfx);
        }
        else
        {
          
            
            firstFlippedCard.Shake();
            secondFlippedCard.Shake();
            wrongAttempts++;
            UpdateAttemptsUI();
        }

        firstFlippedCard = null;
        secondFlippedCard = null;
        if(timerIsRunning)
        {
            canFlip = true;
        }
        

        CheckForGameOver();
    }

    
    void UpdateAttemptsUI()
    {
        
        attemptText.text = $"ATTEMPTS LEFT: {attemptsAllowed - wrongAttempts}";
    }

    void CheckForGameOver()
    {
        if (matchesFound == totalMatches)
        {
            victoryPanel.SetActive(true);
            AS.PlayOneShot(victorySfx);
            timerIsRunning = false;
            timer.enabled  = false;
            timerText.enabled = false;
            attemptText.enabled = false;
        }
        else if (wrongAttempts >= attemptsAllowed)
        {
            gameOverPanel.SetActive(true);
            timerIsRunning = false;
            canFlip = false;
            timer.enabled  = false;
            timerText.enabled = false;
            attemptText.enabled = false;
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}