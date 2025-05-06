using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }
    public List<Question> CurrentQuestions { get; private set; }


    void Awake()
    {
        Debug.Log("QuestionManager Awake called.");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("QuestionManager Instance set.");
        }
        else
        {
            Debug.LogWarning("Duplicate QuestionManager instance found. Destroying the new one.");
            Destroy(gameObject);
        }
    }

    public void LoadQuestions(string level)
    {
        Debug.Log("Loading questions for " + level);
        // Load questions based on the level
        // Example:
        if (level == "Level_0")
        {
            CurrentQuestions = new List<Question>
            {
                new Question("What is the traditional house of the Ibaloi people called?",
                new string[] { "Fale", "Binayon", "Bale", "Binangiyan" },
                "Binangiyan"),

                new Question("Which province in the Philippines is the primary homeland of the Ibaloi people?",
                new string[] { "Ifugao", "Mountain Province", "Benguet", "Kalinga" },
                "Benguet"),

                new Question("What is the term for the Ibaloi's traditional social gathering or feast, often associated with rituals and celebrations?",
                new string[] { "Cañao", "Ummah", "Gawai", "Pestaan" },
                "Cañao"),

                new Question("Traditionally, what was one of the primary economic activities of the Ibaloi people?",
                new string[] { "Fishing", "Weaving intricate textiles", "Gold mining and trading", "Hunting wild animals in the forest" },
                "Gold mining and trading"),

                new Question("What is the name of the Ibaloi's traditional epic or oral literature?",
                new string[] { "Hudhud", "Biag ni Lam-ang", "Ullalim", "There is no specific known epic" },
                "There is no specific known epic"),

                new Question("What type of agricultural practice have the Ibaloi people traditionally been known for in the highlands?",
                new string[] { "Swidden farming", "Large-scale rice paddies", "Terraced farming", "Planting root crops in forests" },
                "Terraced farming"),

                new Question("What is a 'baknang' in the traditional Ibaloi social structure?",
                new string[] { "A respected elder", "A village warrior", "A wealthy and influential person", "A religious leader" },
                "A wealthy and influential person"),

                new Question("Which of these musical instruments is traditionally associated with the Ibaloi people?",
                new string[] { "Kudyapi (two-stringed lute)", "Gangsa (flat gong)", "Kulintang (gong chimes)", "Agung (large hanging gong)" },
                "Gangsa (flat gong)"),

                new Question("What is the general term used to refer to the indigenous peoples of the Cordillera region, including the Ibaloi?",
                new string[] { "Lumad", "Igorot", "Aeta", "Mangyan" },
                "Igorot"),

                new Question("What is a significant cultural practice of the Ibaloi related to death and remembrance?",
                new string[] { "Mummification", "Burial at sea", "Cremation ceremonies", "Leaving the body in a sacred cave" },
                "Mummification")
            };
        }
        else if (level == "Level_1")
        {
            CurrentQuestions = new List<Question>
            {
                // 1
                new Question("What is the general term used to refer to the indigenous peoples of the Cordillera region, including the Ibaloi?",
                new string[] { "Lumad", "Igorot", "Aeta", "Mangyan" },
                "Igorot"),
                
            };
        }
        else if (level == "Level_2")
        {
            CurrentQuestions = new List<Question>
            {
                new Question("What is the general term used to refer to the indigenous peoples of the Cordillera region, including the Ibaloi?",
                new string[] { "Lumad", "Igorot", "Aeta", "Mangyan" },
                "Igorot")
            };
        }



        ShuffleQuestions(CurrentQuestions);
        CurrentQuestions = RandomizeAnswers(CurrentQuestions);
        Debug.Log("Questions loaded: " + CurrentQuestions.Count);
    }


    private void ShuffleQuestions(List<Question> questions)
    {
        System.Random rnd = new System.Random();
        for (int i = questions.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (questions[i], questions[j]) = (questions[j], questions[i]);
        }
    }

    private List<Question> RandomizeAnswers(List<Question> questions)
    {
        List<Question> randomizedQuestions = new List<Question>();
        System.Random rnd = new System.Random();

        foreach (Question q in questions)
        {
            List<string> choices = new List<string>(q.choices);

            for (int i = choices.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (choices[i], choices[j]) = (choices[j], choices[i]);
            }

            randomizedQuestions.Add(new Question(q.questionText, choices.ToArray(), q.correctAnswer));
        }

        return randomizedQuestions;
    }
}