using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public TextMeshProUGUI dialogueText;
    public Button[] answerButtons;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    private Question currentQuestion;
    private List<Question> remainingQuestions = new List<Question>
    {
        new Question("Who developed C?", new string[] { "Dennis Ritchie", "Bjarne Stroustrup", "James Gosling", "Guido van Rossum" }, "Dennis Ritchie"),
        new Question("Which language is used for Android app development?", new string[] { "C#", "Swift", "Kotlin", "Ruby" }, "Kotlin"),
        new Question("What does HTML stand for?", new string[] { "Hyper Text Markup Language", "High Tech Modern Layout", "Hyperlink and Text Management Language", "Home Tool Markup Language" }, "Hyper Text Markup Language"),
        new Question("Who developed Python?", new string[] { "Guido van Rossum", "Linus Torvalds", "Mark Zuckerberg", "Bill Gates" }, "Guido van Rossum"),
        new Question("What year was Java released?", new string[] { "1991", "1995", "2000", "1989" }, "1995"),
        new Question("Which of these is a compiled language?", new string[] { "Python", "JavaScript", "C", "Ruby" }, "C"),
    };

    void Start()
    {
        state = BattleState.START;
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

        dialogueText.text = currentQuestion.questionText; // Display the question immediately

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
        dialogueText.text = "Wrong! The enemy attacks!";

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1.5f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + " attacks!";

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1.5f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1.5f);

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

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated...";
        }

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }
}

public class Question
{
    public string questionText;
    public string[] choices;
    public string correctAnswer;

    public Question(string questionText, string[] choices, string correctAnswer)
    {
        this.questionText = questionText;
        this.choices = choices;
        this.correctAnswer = correctAnswer;
    }
}
