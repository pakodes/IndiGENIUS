using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    public List<Question> currentQuestions;

    private Dictionary<string, List<Question>> levelQuestions = new Dictionary<string, List<Question>>()
    {
        { "Level_0", new List<Question>
            {
                new Question("Who developed the C programming language?", new string[] { "Bjarne Stroustrup", "James Gosling", "Dennis Ritchie", "Guido van Rossum" }, "Dennis Ritchie"),
                new Question("Which language was primarily used for early web development in the 1990s?", new string[] { "Python", "Java", "Perl", "JavaScript" }, "JavaScript"),
                new Question("What year was the Python programming language first released?", new string[] { "1989", "1995", "2000", "1972" }, "1989"),
                new Question("Which company created Java?", new string[] { "Microsoft", "Sun Microsystems", "IBM", "Google" }, "Sun Microsystems"),
                new Question("Which language was heavily inspired by C and developed for Apple in the 1980s?", new string[] { "Swift", "Objective-C", "Ruby", "Fortran" }, "Objective-C"),
                new Question("Who developed the first version of Fortran?", new string[] { "John McCarthy", "Dennis Ritchie", "John Backus", "Alan Turing" }, "John Backus"),
                new Question("What was the first high-level programming language?", new string[] { "COBOL", "FORTRAN", "Lisp", "C" }, "FORTRAN"),
                new Question("What language was created as a successor to C?", new string[] { "Rust", "C++", "Swift", "Kotlin" }, "C++"),
                new Question("Who invented the Lisp programming language?", new string[] { "John McCarthy", "Alan Turing", "Donald Knuth", "Edsger Dijkstra" }, "John McCarthy"),
                new Question("What language introduced object-oriented programming?", new string[] { "Java", "Smalltalk", "C++", "Simula" }, "Simula")
            }
        },
        { "Level_1", new List<Question>
            {
                new Question("Who developed JavaScript?", new string[] { "Brendan Eich", "James Gosling", "Tim Berners-Lee", "Mark Zuckerberg" }, "Brendan Eich"),
                new Question("What year was Java officially released?", new string[] { "1989", "1995", "2000", "1985" }, "1995"),
                new Question("Which language was primarily designed for artificial intelligence research?", new string[] { "Java", "Lisp", "C#", "COBOL" }, "Lisp"),
                new Question("Which company created Swift?", new string[] { "Google", "Apple", "Microsoft", "Oracle" }, "Apple"),
                new Question("Who developed the first compiler?", new string[] { "Alan Turing", "Grace Hopper", "Dennis Ritchie", "Donald Knuth" }, "Grace Hopper"),
                new Question("Which language was created to improve Pascal?", new string[] { "Ada", "Modula-2", "Delphi", "COBOL" }, "Modula-2"),
                new Question("What was the primary purpose of COBOL?", new string[] { "Mathematical computing", "Web development", "Business applications", "System programming" }, "Business applications"),
                new Question("Which language was developed at Bell Labs alongside Unix?", new string[] { "C++", "Python", "C", "Ruby" }, "C"),
                new Question("Who developed PHP?", new string[] { "Rasmus Lerdorf", "Guido van Rossum", "Larry Page", "Mark Zuckerberg" }, "Rasmus Lerdorf"),
                new Question("Which language was created by Microsoft as a competitor to Java?", new string[] { "C#", "Go", "Swift", "Objective-C" }, "C#")
            }
        },
        { "Level_2", new List<Question>
            {
                new Question("Which programming language is considered the precursor to C?", new string[] { "B", "Fortran", "Pascal", "COBOL" }, "B"),
                new Question("Which of these languages was created first?", new string[] { "Python", "Java", "C", "Fortran" }, "Fortran"),
                new Question("What year was C++ officially released?", new string[] { "1979", "1983", "1991", "2000" }, "1983"),
                new Question("Which programming language was heavily influenced by Smalltalk?", new string[] { "Java", "Ruby", "C++", "Swift" }, "Ruby"),
                new Question("What language was developed by Google?", new string[] { "Dart", "Go", "Kotlin", "Swift" }, "Go"),
                new Question("Who created the concept of structured programming?", new string[] { "John von Neumann", "Edsger Dijkstra", "Donald Knuth", "Alan Turing" }, "Edsger Dijkstra"),
                new Question("Which programming language was originally called 'Oak'?", new string[] { "Java", "Kotlin", "C#", "Swift" }, "Java"),
                new Question("What was the main innovation of Prolog?", new string[] { "Logical programming", "Object-oriented programming", "Compiled execution", "Concurrency" }, "Logical programming"),
                new Question("Which language introduced garbage collection?", new string[] { "C", "Java", "Lisp", "Python" }, "Lisp"),
                new Question("What was the first language to introduce exception handling?", new string[] { "C++", "Ada", "Java", "FORTRAN" }, "Ada")
            }
        }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadQuestions(string levelName)
    {
        if (levelQuestions.ContainsKey(levelName))
        {
            List<Question> questions = new List<Question>(levelQuestions[levelName]);
            currentQuestions = RandomizeAnswers(questions);
        }
    }

    private List<Question> RandomizeAnswers(List<Question> questions)
    {
        List<Question> randomizedQuestions = new List<Question>();

        foreach (Question q in questions)
        {
            List<string> choices = new List<string>(q.choices);
            string correctAnswer = q.correctAnswer;

            // Shuffle the choices
            System.Random rnd = new System.Random();
            for (int i = choices.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (choices[i], choices[j]) = (choices[j], choices[i]);
            }

            randomizedQuestions.Add(new Question(q.questionText, choices.ToArray(), correctAnswer));
        }

        return randomizedQuestions;
    }
}
