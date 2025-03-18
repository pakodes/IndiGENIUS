using UnityEngine;

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
