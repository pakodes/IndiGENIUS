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
                new Question("Who developed the C programming language?",
                new string[] { "Bjarne Stroustrup", "James Gosling", "Dennis Ritchie", "Guido van Rossum" },
                "Dennis Ritchie"),
                // 2
                new Question("Which company created Java?",
                new string[] { "Microsoft", "Sun Microsystems", "IBM", "Google" },
                "Sun Microsystems"),
                // 3
                new Question("What was ARPANet?",
                new string[] { "A computer network", "A programming language", "A computer virus", "A computer manufacturer" },
                "A computer network"),
                // 4
                new Question("What does SQL stand for?",
                new string[] { "Simple Query Language", "Structured Query Language", "System Query Language", "Standard Query Language" },
                "Structured Query Language"),
                // 5
                new Question("What is the full form of FORTRAN?",
                new string[] { "Formatted Translation", "Formula Translation", "Format Translation", "Formula Transformer" },
                "Formula Translation"),
                // 6
                new Question("Which company developed COBOL, FORTRAN, and RPG?",
                new string[] { "Microsoft", "IBM", "Apple", "Google" },
                "IBM"),
                // 7
                new Question("What is the function of JavaScript in web pages?",
                new string[] { "Styling the page", "Adding interactivity", "Structuring the content", "Storing data" },
                "Adding interactivity"),
                // 8
                new Question("What is the foundation for modern operating systems?",
                new string[] { "Windows", "UNIX", "MacOS", "Android" },
                "UNIX"),
                // 9
                new Question("Which programming language is known for its ease of use?",
                new string[] { "Java", "Python", "C", "JavaScript" },
                "Python"),
                // 10
                new Question("What is the primary use of Visual Basic?",
                new string[] { "Designing visual graphics", "Creating Windows applications", "Visualizing data", "Building visual applications" },
                "Creating Windows applications"),
                // 11
                new Question("Which company created Swift?",
                new string[] { "Google", "Apple", "Microsoft", "Oracle" },
                "Apple"),
                // 12
                new Question("Which language was primarily designed for artificial intelligence research?",
                new string[] { "Java", "Lisp", "C#", "COBOL" },
                "Lisp"),
                // 13
                new Question("Which programming language is considered the precursor to C?",
                new string[] { "B", "Fortran", "Pascal", "COBOL" },
                "B"),
                // 14
                new Question("What language was developed by Google?",
                new string[] { "Dart", "Go", "Kotlin", "Swift" },
                "Go"),
                // 15
                new Question("What is the \"write once, run anywhere\" principle associated with?",
                new string[] { "HTML", "Java", "JavaScript", "C++" },
                "Java")
            };
        }
        else if (level == "Level_1")
        {
            CurrentQuestions = new List<Question>
            {
               // 1
                new Question("Which language was created by Microsoft as a competitor to Java?",
                new string[] { "C#", "Go", "Swift", "Objective-C" },
                "C#"),
                // 2
                new Question("Who invented the Lisp programming language?",
                new string[] { "John McCarthy", "Alan Turing", "Donald Knuth", "Edsger Dijkstra" },
                "John McCarthy"),
                // 3
                new Question("Who developed JavaScript?",
                new string[] { "Brendan Eich", "James Gosling", "Tim Berners-Lee", "Mark Zuckerberg" },
                "Brendan Eich"),
                // 4
                new Question("Which language was developed at Bell Labs alongside Unix?",
                new string[] { "C++", "Python", "C", "Ruby" },
                "C"),
                // 5
                new Question("Which programming language was originally called 'Oak'?",
                new string[] { "Java", "Kotlin", "C#", "Swift" },
                "Java"),
                // 6
                new Question("What was the main innovation of Prolog?",
                new string[] { "Logical programming", "Object-oriented programming", "Compiled execution", "Concurrency" },
                "Logical programming"),
                // 7
                new Question("Who was known as the first computer programmer?",
                new string[] { "Charles Babbage", "Alan Turing", "Ada Lovelace", "Mark Zuckerberg" },
                "Ada Lovelace"),
                // 8
                new Question("What was JavaScript previously named?",
                new string[] { "OldScript", "LiveScript", "TypeScript", "JS" },
                "LiveScript"),
                // 9
                new Question("Which of these was created first?",
                new string[] { "Assembly", "Autocode", "COBOL", "Fortran" },
                "Assembly"),
                // 10
                new Question("Which was considered as the first object-oriented programming language?",
                new string[] { "Java", "Simula", "Smalltalk", "Ada" },
                "Simula")
            };
        }
        else if (level == "Level_2")
        {
            CurrentQuestions = new List<Question>
            {
                 new Question("What was the programming language designed to program the Z3?",
                new string[] { "Plankalkul", "X", "X", "X" },
                "Plankalkul"),
                // 2
                new Question("What was the test that defined the standard for machine intelligence?",
                new string[] { "Turing Test", "X", "X", "X" },
                "Turing Test"),
                // 3
                new Question("An early programming language used to translate instructions into machine code automatically",
                new string[] { "X", "X", "X", "Autocode" },
                "Autocode"),
                // 4
                new Question("What is the computing method that uses quantum mechanics?",
                new string[] { "Quantum Computing", "X", "X", "X" },
                "Quantum Computing"),
                // 5
                new Question("It is a small, integrated circuit that performs the functions of a computer's CPU",
                new string[] { "Microprocessor", "X", "X", "X" },
                "Microprocessor")
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