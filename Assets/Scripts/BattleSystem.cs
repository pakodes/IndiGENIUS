using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public AudioClip loseClip;
    public AudioClip victoryClip;
    public AudioClip playerHitClip;
    public AudioClip enemyHitClip;
    private AudioSource audioSource;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public GameObject playerHitEffectPrefab; 
    public GameObject enemyHitEffectPrefab;

    public float playerShakeDuration = 0.1f;
    public float playerShakeMagnitude = 0.1f;
    public float enemyShakeDuration = 0.2f;
    public float enemyShakeMagnitude = 0.2f;

    Unit playerUnit;
    Unit enemyUnit;

    public TextMeshProUGUI dialogueText;
    public Button[] answerButtons;
    public Button nextRoundButton;
    public Button backToHomeButton;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    private Question currentQuestion;
    private List<Question> remainingQuestions;

    void Start()
    {
        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        nextRoundButton.gameObject.SetActive(false);
        backToHomeButton.gameObject.SetActive(false);

        nextRoundButton.onClick.AddListener(NextRound);
        backToHomeButton.onClick.AddListener(BackToHome);

        state = BattleState.START;

        // Ensure QuestionManager is initialized
        if (QuestionManager.Instance == null)
        {
            Debug.LogError("QuestionManager Instance is null! Ensure it exists in the scene.");
        }
        else
        {
            if (QuestionManager.Instance.CurrentQuestions == null || QuestionManager.Instance.CurrentQuestions.Count == 0)
            {
                QuestionManager.Instance.LoadQuestions("Level_0");
            }

            remainingQuestions = new List<Question>(QuestionManager.Instance.CurrentQuestions);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the GameObject.");
        }

        StartCoroutine(SetupBattle());
    }


    IEnumerator SetupBattle()
    {

        GameObject player = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = player.GetComponent<Unit>();

        GameObject enemy = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemy.GetComponent<Unit>();

        dialogueText.text = "A wild " + enemyUnit.unitName + " appears!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        AskQuestion();
    }

    void AskQuestion()
    {
        if (remainingQuestions.Count == 0)
        {
            dialogueText.text = "No more questions! You win!";
            state = BattleState.WON;
            EndBattle();
            return;
        }

        if (currentQuestion == null)
        {
            int index = Random.Range(0, remainingQuestions.Count);
            currentQuestion = remainingQuestions[index];
            remainingQuestions.RemoveAt(index);
        }

        dialogueText.text = currentQuestion.questionText;

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(true);
        }

        DisplayAnswers();
    }

    void DisplayAnswers()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.choices[i];
            answerButtons[i].onClick.RemoveAllListeners();
            string selectedAnswer = currentQuestion.choices[i];
            answerButtons[i].onClick.AddListener(() => CheckAnswer(selectedAnswer));
        }
    }

    void CheckAnswer(string selectedAnswer)
    {
        if (state != BattleState.PLAYERTURN) return;

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        if (selectedAnswer == currentQuestion.correctAnswer)
        {
            StartCoroutine(PlayerCorrectAnswer());
        }
        else
        {
            StartCoroutine(PlayerWrongAnswer());
        }
    }

    IEnumerator PlayerCorrectAnswer()
    {
        dialogueText.text = "Correct! You deal damage!";
        yield return new WaitForSeconds(1.5f);

        // Trigger player attack sequence
        yield return StartCoroutine(AttackSequence(playerBattleStation, enemyBattleStation, playerHitClip, playerHitEffectPrefab, playerShakeDuration, playerShakeMagnitude));

        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.SetHP(enemyUnit.currentHP);

        currentQuestion = null;

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            AskQuestion();
        }
    }

    IEnumerator PlayerWrongAnswer()
    {
        state = BattleState.ENEMYTURN;
        yield return StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = "Wrong! " + enemyUnit.unitName + " attacks!";
        yield return new WaitForSeconds(1.5f);

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        // Trigger enemy attack sequence
        yield return StartCoroutine(AttackSequence(enemyBattleStation, playerBattleStation, enemyHitClip, enemyHitEffectPrefab, enemyShakeDuration, enemyShakeMagnitude));

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        playerHUD.SetHP(playerUnit.currentHP);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            AskQuestion();
        }
    }


    IEnumerator AttackSequence(Transform attacker, Transform target, AudioClip hitClip, GameObject hitEffectPrefab, float shakeDuration, float shakeMagnitude)
    {
        // Play hit sound
        if (hitClip != null && audioSource != null)
        {
            audioSource.clip = hitClip;
            audioSource.Play();
            Debug.Log("Playing hit sound.");
        }
        else
        {
            Debug.LogWarning("Hit clip or audio source is missing.");
        }

        // Trigger hit effect
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, target.position, Quaternion.identity);
            Destroy(hitEffect, 1f); // Destroy the effect after 1 second
        }

        // Trigger screen shake for attacker
        yield return StartCoroutine(ScreenShake(attacker, shakeDuration, shakeMagnitude));
    }


    void EndBattle()
    {
        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
            foreach (Button btn in answerButtons)
            {
                btn.gameObject.SetActive(false);
            }
            nextRoundButton.gameObject.SetActive(true);
            nextRoundButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next Round";

            if (victoryClip != null && audioSource != null)
            {
                audioSource.clip = victoryClip;
                audioSource.Play();
            }
        }
        else if (state == BattleState.LOST)
        {

            dialogueText.text = "You were defeated...";
            foreach (Button btn in answerButtons)
            {
                btn.gameObject.SetActive(false);
            }
            backToHomeButton.gameObject.SetActive(true);
            backToHomeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Try Again?";
           
            if (loseClip != null && audioSource != null)
            {
                audioSource.clip = loseClip;
                audioSource.Play();
            }
        }
    }

    public void NextRound()
    {
        if (state == BattleState.WON)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Level_0")
            {
                QuestionManager.Instance.LoadQuestions("Level_1"); // Load Level 1 questions
                SceneManager.LoadScene("Level_1");
            }
            else if (currentScene == "Level_1")
            {
                QuestionManager.Instance.LoadQuestions("Level_2"); // Load Level 2 questions
                SceneManager.LoadScene("Level_2");
            }
        }
    }


    public void BackToHome()
    {
        if (state == BattleState.LOST)
        {
            SceneManager.LoadScene("MainMenu");
        }

    }
    // Add this method to the BattleSystem class
    IEnumerator ScreenShake(Transform target, float duration, float magnitude)
    {
        Vector3 originalPosition = target.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            target.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        target.localPosition = originalPosition;
    }
}



