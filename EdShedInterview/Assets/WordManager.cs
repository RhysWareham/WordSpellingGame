using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WordManager : MonoBehaviour
{
    [SerializeField] private TMP_Text wordDisplay;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text errorsText;
    [SerializeField] private Button restartButton;


    private List<Word> words = new List<Word>();
    private int currentIndex = 0;
    private int score = 0;
    private int errors = 0;
    [SerializeField] private float timeLeft = 60f;
    private bool gameActive = false;

    void Start()
    {
        inputField.onSubmit.AddListener(OnWordEntered);
        restartButton.onClick.AddListener(RestartGame);
        StartCoroutine(FetchWordList());
    }

    private void Update()
    {
        if (!gameActive) return;

        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(timeLeft, 0);

        timerText.text = "Time: " + Mathf.Round(timeLeft);

        if (timeLeft <= 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        gameActive = false;
        inputField.interactable = false;
        wordDisplay.text = "Time's Up!";
    }

    private void RestartGame()
    {
        score = 0;
        scoreText.text = "Score: 0";
        errors = 0;
        errorsText.text = "Mistakes: 0";
        timeLeft = 60f;
        gameActive = true;
        inputField.text = "";
        inputField.interactable = true;
        ShowRandomWord();
    }

    private void OnWordEntered(string input)
    {
        if (!gameActive) return;

        if (input.Trim().ToLower() == words[currentIndex].text.ToLower())
        {
            score++;
            scoreText.text = "Score: " + score;
            inputField.text = "";
            ShowRandomWord();
        }
        else
        {
            errors++;
            errorsText.text = "Mistakes: " + errors;
            StartCoroutine(FlashIncorrectInput());
        }
    }

    private IEnumerator FlashIncorrectInput()
    {
        inputField.DeactivateInputField();

        Color originalColor = inputField.textComponent.color;

        // Set color to red
        inputField.textComponent.color = Color.red;

        yield return new WaitForSeconds(0.3f);

        // Reset color and clear input
        inputField.textComponent.color = originalColor;
        inputField.text = "";
        inputField.ActivateInputField(); // Refocus input
    }

    private IEnumerator FetchWordList()
    {
        string url = "https://api.edshed.com/lists/Y12";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch data: " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                Root root = JsonConvert.DeserializeObject<Root>(json);
                words = root.list.words;
            }
        }
    }

    private void ShowRandomWord()
    {
        currentIndex = Random.Range(0, words.Count);
        wordDisplay.text = words[currentIndex].text;
        inputField.ActivateInputField();
    }
}
