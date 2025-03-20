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
    public Image transitionPanel;
    public AudioClip loseClip;
    public AudioClip victoryClip;
    public AudioClip playerHitClip;
    public AudioClip enemyHitClip;
    private AudioSource audioSource;
    public AudioSource bgmSource; 

    private GameObject instantiatedPlayer;
    private GameObject instantiatedEnemy;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject playerAttackFXPrefab; 
    public GameObject enemyAttackFXPrefab; 

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public TextMeshProUGUI progressText;
  
    

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

    private ScreenShake screenShake;

    void Start()
    {
        Debug.Log("BattleSystem Start called.");

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
            Debug.Log("QuestionManager Instance found.");
            if (QuestionManager.Instance.CurrentQuestions == null || QuestionManager.Instance.CurrentQuestions.Count == 0)
            {
                Debug.Log("Loading questions for Level_0.");
                QuestionManager.Instance.LoadQuestions("Level_0");
            }

            remainingQuestions = new List<Question>(QuestionManager.Instance.CurrentQuestions);
            Debug.Log("Remaining questions count: " + remainingQuestions.Count);

            // Shuffle the questions
            ShuffleQuestions(remainingQuestions);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the GameObject.");
        }

        screenShake = Camera.main.GetComponent<ScreenShake>();

        StartCoroutine(SetupBattle());
    }


    IEnumerator Transition(float duration, bool fadeIn)
    {
        float elapsedTime = 0f;
        Color color = transitionPanel.color;

        while (elapsedTime < duration)
        {
            float alpha = fadeIn ? (elapsedTime / duration) : (1 - (elapsedTime / duration));
            color.a = alpha;
            transitionPanel.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = fadeIn ? 1 : 0;
        transitionPanel.color = color;
    }
    IEnumerator SetupBattle()
    {
        Debug.Log("Setting up battle...");

        instantiatedPlayer = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = instantiatedPlayer.GetComponent<Unit>();

        instantiatedEnemy = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = instantiatedEnemy.GetComponent<Unit>();

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
        UpdateProgress();
    }

    IEnumerator PlayerCorrectAnswer()
    {
        dialogueText.text = "Correct! You deal damage!";
        yield return new WaitForSeconds(1.5f);

        // Trigger player attack sequence
        yield return StartCoroutine(AttackSequence(instantiatedPlayer, instantiatedEnemy, playerHitClip, true));

        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.SetHP(enemyUnit.currentHP, enemyUnit.maxHP); // Update enemy health bar

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
        yield return StartCoroutine(AttackSequence(instantiatedEnemy, instantiatedPlayer, enemyHitClip, false));

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        playerHUD.SetHP(playerUnit.currentHP, playerUnit.maxHP); // Update player health bar

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
    IEnumerator AttackSequence(GameObject attacker, GameObject target, AudioClip hitClip, bool isPlayerAttack)
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

        // Move attacker forward
        Vector3 originalPosition = attacker.transform.position;
        Vector3 targetPosition = target.transform.position;
        Vector3 attackPosition = Vector3.Lerp(originalPosition, targetPosition, 0.4f); // Move attacker farther

        float elapsedTime = 0f;
        float moveDuration = 0.05f; // Faster movement

        while (elapsedTime < moveDuration)
        {
            attacker.transform.position = Vector3.Lerp(originalPosition, attackPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Instantiate attack effect
        GameObject attackEffect = Instantiate(isPlayerAttack ? playerAttackFXPrefab : enemyAttackFXPrefab, target.transform.position, Quaternion.identity);
        Animator animator = attackEffect.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(0); // Play the default animation
        }
        Destroy(attackEffect, 1f); // Destroy the effect after 1 second

        // Trigger screen shake
        if (screenShake != null)
        {
            StartCoroutine(screenShake.Shake(0.1f, 0.1f)); // Adjust duration and magnitude as needed
        }

        // Move target back
        Vector3 targetOriginalPosition = target.transform.position;
        Vector3 targetBackPosition = targetOriginalPosition + (targetOriginalPosition - originalPosition).normalized * 0.1f; // Move target back a bit

        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            target.transform.position = Vector3.Lerp(targetOriginalPosition, targetBackPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move attacker back to original position
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            attacker.transform.position = Vector3.Lerp(attackPosition, originalPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move target back to original position
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            target.transform.position = Vector3.Lerp(targetBackPosition, targetOriginalPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        attacker.transform.position = originalPosition;
        target.transform.position = targetOriginalPosition;
    }

    void EndBattle()
    {
        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        // Stop the BGM
        if (bgmSource != null)
        {
            bgmSource.Stop();
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
            Debug.Log("Current Scene: " + currentScene);

            if (currentScene == "Level_0")
            {
                Debug.Log("Loading Level 1 questions...");
                QuestionManager.Instance.LoadQuestions("Level_1"); // Load Level 1 questions
                StartCoroutine(LoadNextScene("Level_1"));
            }
            else if (currentScene == "Level_1")
            {
                Debug.Log("Loading Level 2 questions...");
                QuestionManager.Instance.LoadQuestions("Level_2"); // Load Level 2 questions
                StartCoroutine(LoadNextScene("Level_2"));
            }
        }
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        yield return StartCoroutine(Transition(1f, true)); // Fade out
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(Transition(1f, false)); // Fade in
    }


    void ShuffleQuestions(List<Question> questions)
    {
        for (int i = 0; i < questions.Count; i++)
        {
            Question temp = questions[i];
            int randomIndex = Random.Range(i, questions.Count);
            questions[i] = questions[randomIndex];
            questions[randomIndex] = temp;
        }
    }

    void UpdateProgress()
    {
        int totalQuestions = QuestionManager.Instance.CurrentQuestions.Count;
        int answeredQuestions = totalQuestions - remainingQuestions.Count;
        float progress = (float)answeredQuestions / totalQuestions;

        progressText.text = $"Progress: {answeredQuestions}/{totalQuestions} ({progress * 100:F0}%)";
 
    }
    public void BackToHome()
    {
        if (state == BattleState.LOST)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

