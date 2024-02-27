using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

// Quiz Class
class Quiz
{
    public int id;
    public string title;
    public int maximumScore;
    public bool isPublished;
    public List<Question> questions;
    public List<QuizAttempt> quizAttempts;

    public Quiz()
    {
        questions = new List<Question>();
        quizAttempts = new List<QuizAttempt>();
    }

    public Quiz(int i, string t)
    {
        id = i;
        title = t;
        questions = new List<Question>();
    }

    public void AddQuestion(Question q)
    {
        questions.Add(q);
        maximumScore += q.points; 
    }

    public override string ToString()
    {
        return $"Quiz ID: {id}\nTitle: {title}\nMaximum Score: {maximumScore}\nIs Published: {isPublished}";
    }
}
// QuizAttempt Class
class QuizAttempt
{
  
    public int totalScore;
    public DateTime dateCreated;
    public List<int> questionScores;

    public QuizAttempt()
    {
        questionScores = new List<int>();
    }

    public QuizAttempt(Quiz q)
    {
        totalScore = 0;
        dateCreated = DateTime.Now;
        questionScores = new List<int>(q.questions.Count);
    }
}
// Question Class
class Question
{
    public int id;
    public string prompt;
    public int points;

    public Question()
    {
    }

    public Question(int i, string pr, int po)
    {
        id = i;
        prompt = pr;
        points = po;
    }

    public virtual int CheckAnswer(List<string> answers)
    {
        return 0;
    }

    public override string ToString()
    {
        return $"Question ID: {id}\nPrompt: {prompt}\nPoints: {points}";
    }
}

// MultipleChoiceQuestion Class (inherits Question)
class MultipleChoiceQuestion : Question
{
    public List<string> choices;
    public int correctAnswerIndex;

    public MultipleChoiceQuestion()
    {
        choices = new List<string>();
    }

    public MultipleChoiceQuestion(int i, string pr, int po, List<string> ch, int co)
        : base(i, pr, po)
    {
        choices = ch;
        correctAnswerIndex = co;
    }

    public override int CheckAnswer(List<string> answers)
    {
        if (answers.Count != 1)
        {
            Console.WriteLine("Invalid answer. Please try again.");
            return 0;
        }

        int selectedChoiceIndex;
        if (!int.TryParse(answers[0], out selectedChoiceIndex) || selectedChoiceIndex < 1 || selectedChoiceIndex > choices.Count)
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return 0;
        }

        if (selectedChoiceIndex - 1 == correctAnswerIndex)
        {
            Console.WriteLine("Correct answer!");
            return points;
        }
        else
        {
            Console.WriteLine("Incorrect answer!");
            return 0;
        }
    }

    public override string ToString()
    {
        return $"MCQ;{id};{prompt};{points};{string.Join(";", choices)};{correctAnswerIndex}";
    }
}

// MultipleResponseQuestion Class (inherits Question)
class MultipleResponseQuestion : Question
{
    public List<Choice> choices;

    public MultipleResponseQuestion()
    {
        choices = new List<Choice>();
    }

    public MultipleResponseQuestion(int i, string pr, int po, List<Choice> ch)
        : base(i, pr, po)
    {
        choices = ch;
    }



    public override int CheckAnswer(List<string> answers)
    {
        // Initialize score as maximum, it will be reduced to 0 if any answer is incorrect
        int score = points;

        foreach (var answer in answers)
        {
            int selectedChoiceIndex;
            if (!int.TryParse(answer, out selectedChoiceIndex) || selectedChoiceIndex < 0 || selectedChoiceIndex >= choices.Count)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                return 0;
            }

            // If any of the user's choices is not correct, set score to 0
            if (!choices[selectedChoiceIndex].isCorrect)
            {
                score = 0;
                break;
            }
        }

        // If score is still maximum, all selected answers were correct.
        // Now check if the user selected all correct answers
        if (score == points)
        {
            // Check if all correct choices are selected by the user
            for (int i = 0; i < choices.Count; i++)
            {
                if (choices[i].isCorrect && !answers.Contains(i.ToString()))
                {
                    // A correct choice was not selected by the user, set score to 0
                    score = 0;
                    break;
                }
            }
        }

        if (score == points)
        {
            Console.WriteLine("All answers are correct!");
        }
        else
        {
            Console.WriteLine("Some answers are incorrect!");
        }

        return score;
    }


    public override string ToString()
    {
        var choicesStrings = choices.Select(choice => $"{choice.text},{choice.isCorrect}").ToList();
        return $"MRQ;{id};{prompt};{points};{string.Join(";", choicesStrings)}";
    }
}


// Choice Class
class Choice
{
    public string text;
    public bool isCorrect;

    public Choice()
    {
    }

    public Choice(string t, bool c)
    {
        text = t;
        isCorrect = c;
    }

    public override string ToString()
    {
        return $"Text: {text}\nIs Correct: {isCorrect}";
    }
}

// Main class
class Program
{
   
    static List<Quiz> unpublishedQuizzes = new List<Quiz>(); // New list to store newly added ques


    static void Main(string[] args)
    {
       
        List<Quiz> questions = new List<Quiz>();
        List<Quiz> quizzes = new List<Quiz>();


        int option = 0;
        // Load questions from questions.txt
        LoadQuestionsFromFile(questions);

        while (option != 12)
        {
            DisplayMainMenu();

            if (int.TryParse(Console.ReadLine(), out option))
            {
                switch (option)
                {
                    case 1:
                        // List all multiple-choice questions
                        ListAllMultipleChoiceQuestions(questions);
                        break;
                    case 2:
                        ListAllMultipleResponseQuestions(questions);
                        break;

                    case 3:
                        // Add multiple-choice question
                        AddMultipleChoiceQuestion();
                        break;

                    case 4:
                        // Add multiple-response question
                        AddMultipleResponseQuestion();
                        break;
                    case 5:
                        // Load questions
                        LoadQuestionsFromFile(questions);
                        Console.WriteLine("Questions uploaded");
                        break;

                    case 6:
                        ListAllQuizzes(quizzes);
                        break;
                    case 7:
                        // Add quiz
                        AddQuiz(quizzes);
                        break;

                    case 8:
                        AddQuestionToQuiz(quizzes);
                        break;

                    case 9:
                        // Publish quiz
                        PublishQuiz(quizzes);
                        break;
                    case 10:
                        // Start quiz
                        StartQuiz(quizzes);
                        break;
                    case 11:
                        // List quiz scores
                        ListQuizScores(quizzes);
                        break;

                    case 12:
                        break;
                    case 13:
                        EditQuestionText();
                        LoadQuestionsFromFile(questions);
                        Console.WriteLine("Questions updated");
                        break;
                    case 14:
                        RandomizeQuizQuestionsOrder();
                        break;
                    case 15:
                        RemoveQuestionFromQuiz();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid option.");
            }

            Console.WriteLine();
        }
    }
    static void DisplayMainMenu()
    {
        Console.WriteLine("Main Menu");
        Console.WriteLine("1. List all multiple-choice questions");
        Console.WriteLine("2. List all multiple-response questions");
        Console.WriteLine("3. Add multiple-choice question");
        Console.WriteLine("4. Add multiple-response question");
        Console.WriteLine("5. Load questions");
        Console.WriteLine("6. List all quizzes");
        Console.WriteLine("7. Add quiz");
        Console.WriteLine("8. Add question to quiz");
        Console.WriteLine("9. Publish quiz");
        Console.WriteLine("10. Start quiz");
        Console.WriteLine("11. List quiz scores");
        Console.WriteLine("12. To Exit");
        Console.WriteLine("13. Edit a question");
        Console.WriteLine("14. Randomize Questions");
        Console.WriteLine("15. Remove Question from file ");
        Console.Write("Enter option: ");
    }
    // ...

    // Helper method to retrieve a specific question from questions.txt based on its ID
  


    static void LoadQuestionsFromFile(List<Quiz> questions)
    {
        questions.Clear(); // Clear the existing list of ques
        try
        {
            string[] lines = File.ReadAllLines("questions.txt");
            Dictionary<int, Quiz> quizDict = new Dictionary<int, Quiz>(); // New dictionary to keep track of ques

            // Iterate through each line in the "questions.txt" file
            foreach (var line in lines)
            {
                string[] parts = line.Split(';');

                // Ensure the line has enough parts to represent a valid question
                if (parts.Length >= 6)
                {
                    string type = parts[0];
                    int id = int.Parse(parts[1]);
                    string prompt = parts[2];
                    int points = int.Parse(parts[3]);

                    // Check if the quiz with the given ID already exists in the dictionary
                    if (!quizDict.TryGetValue(id, out Quiz currentQuiz))
                    {
                        // If not, create a new quiz object and add it to the dictionary
                        currentQuiz = new Quiz(id, "Quiz " + id);
                        questions.Add(currentQuiz);
                        quizDict.Add(id, currentQuiz);
                    }


                    if (type == "MCQ")
                    {
                        List<string> choices = parts.Skip(4).Take(parts.Length - 5).ToList();
                        int correctAnswerIndex = int.Parse(parts[parts.Length - 1]);

                        // Create a new MultipleChoiceQuestion object with the extracted information
                        MultipleChoiceQuestion mcq = new MultipleChoiceQuestion(id, prompt, points, choices, correctAnswerIndex);

                        // Add the multiple-choice question to the current quiz's list of questions
                        currentQuiz.AddQuestion(mcq);
                    }
                    else if (type == "MRQ")
                    {
                        // Handle MultipleResponseQuestion
                        List<string> choicesData = parts.Skip(4).ToList(); // No need to use Take here
                        List<Choice> choices = new List<Choice>();

                        foreach (string choiceData in choicesData)
                        {
                            string[] choiceParts = choiceData.Split(',');
                            if (choiceParts.Length == 2)
                            {
                                string choiceText = choiceParts[0];
                                bool isCorrect = bool.Parse(choiceParts[1]);
                                choices.Add(new Choice(choiceText, isCorrect));
                            }
                        }

                        // Create a new MultipleResponseQuestion object with the extracted information
                        MultipleResponseQuestion mrq = new MultipleResponseQuestion(id, prompt, points, choices);

                        // Add the multiple-response question to the current quiz's list of questions
                        currentQuiz.AddQuestion(mrq);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading questions from file: " + ex.Message);
        }
    }


    static bool IsQuestionIdPresentInFile(int id)
    {
        try
        {
            string[] lines = File.ReadAllLines("questions.txt");
            foreach (var line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    int questionId = int.Parse(parts[1]);
                    if (questionId == id)
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading questions from file: " + ex.Message);
        }

        return false;
    }
    static void AddMultipleChoiceQuestion()
    {
        int id = 0; // Initialize to a default value
        bool validId = false;

        while (!validId)
        {
            // Prompt the user to enter the id of the question
            Console.Write("Enter id: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out id))
            {
                // Check if the ID is already present in "questions.txt"
                if (IsQuestionIdPresentInFile(id))
                {
                    Console.WriteLine("ID already exists. Please enter a different ID.");
                }
                else
                {
                    validId = true;
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a valid numeric ID.");
            }
        }

        // Prompt the user to enter the prompt (question text)
        Console.Write("Enter prompt: ");
        string prompt = Console.ReadLine();

        // Prompt the user to enter the points (score) for the question
        Console.Write("Enter points: ");
        int points = int.Parse(Console.ReadLine());

        // Prompt the user to enter the number of choices for the question
        Console.Write("Enter number of choices: ");
        int numChoices = int.Parse(Console.ReadLine());

        // Create a list to store the choices
        List<string> choices = new List<string>();

        // Loop through each choice
        for (int i = 0; i < numChoices; i++)
        {
            // Generate the letter corresponding to the choice (A, B, C, ...)
            char letter = (char)('A' + i);

            // Prompt the user to enter the choice text
            Console.Write($"Enter choice {letter}: ");
            choices.Add(Console.ReadLine());
        }

        int correctAnswerIndex = -1;
        while (correctAnswerIndex < 0 || correctAnswerIndex >= numChoices)
        {
            // Prompt the user to enter the index of the correct answer (A, B, C, ...)
            Console.Write("Enter correct answer (A, B, C, ...): ");
            string correctAnswer = Console.ReadLine().ToUpper();

            if (correctAnswer.Length != 1 || correctAnswer[0] < 'A' || correctAnswer[0] >= 'A' + numChoices)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                continue;
            }

            correctAnswerIndex = correctAnswer[0] - 'A';
        }

        // Create a new instance of the MultipleChoiceQuestion class with the provided information
        MultipleChoiceQuestion mcq = new MultipleChoiceQuestion(id, prompt, points, choices, correctAnswerIndex);

        // Open the "questions.txt" file and append the string representation of the question to it
        using (StreamWriter sw = File.AppendText("questions.txt"))
        {
            sw.WriteLine(mcq.ToString());
        }

        // Notify the user that the question has been added
        Console.WriteLine("Question has been added.");
    }

    static void AddMultipleResponseQuestion()
    {
        int id = 0; // Initialize to a default value
        bool validId = false;

        while (!validId)
        {
            // Prompt the user to enter the id of the question
            Console.Write("Enter id: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out id))
            {
                // Check if the ID is already present in "questions.txt"
                if (IsQuestionIdPresentInFile(id))
                {
                    Console.WriteLine("ID already exists. Please enter a different ID.");
                }
                else
                {
                    validId = true;
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a valid numeric ID.");
            }
        }
        // Prompt the user to enter the prompt (question text)
        Console.Write("Enter prompt: ");
        string prompt = Console.ReadLine();

        // Prompt the user to enter the points (score) for the question
        Console.Write("Enter points: ");
        int points = int.Parse(Console.ReadLine());

        // Prompt the user to enter the number of choices for the question
        Console.Write("Enter number of choices: ");
        int numChoices = int.Parse(Console.ReadLine());

        // Create a list to store the response choices
        List<Choice> responseChoices = new List<Choice>();

        // Loop through each response choice
        for (int i = 0; i < numChoices; i++)
        {
            // Generate the letter corresponding to the choice (A, B, C, ...)
            char letter = (char)('A' + i);

            // Prompt the user to enter the choice text
            Console.Write($"Enter choice {letter}: ");
            string choiceText = Console.ReadLine();

            // Prompt the user to enter whether the choice is correct or not (yes/no)
            Console.Write($"Choice {letter} is correct (yes/no): ");
            bool isCorrect = Console.ReadLine().ToLower() == "yes";

            // Create a new instance of the Choice class with the provided information and add it to the list
            responseChoices.Add(new Choice(choiceText, isCorrect));
        }

        // Create a new instance of the MultipleResponseQuestion class with the provided information
        MultipleResponseQuestion mrq = new MultipleResponseQuestion(id, prompt, points, responseChoices);

        // Open the "questions.txt" file and append the string representation of the question to it
        using (StreamWriter sw = File.AppendText("questions.txt"))
        {
            sw.WriteLine(mrq.ToString());
        }

        // Notify the user that the question has been added
        Console.WriteLine("Question has been added.");
    }
 
    static void AddQuiz(List<Quiz> quizzes)
    {
        bool validId = false;
        int id = 0;

        while (!validId)
        {
            // Prompt user to enter id
            Console.Write("Enter id: ");
            if (int.TryParse(Console.ReadLine(), out id))
            {
                // Check if the ID already exists in the list of quizzes
                if (quizzes.Any(quiz => quiz.id == id))
                {
                    Console.WriteLine("Quiz with the entered ID already exists. Please enter a different ID.");
                }
                else
                {
                    validId = true;
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a valid numeric ID.");
            }
        }

        // Prompt user to enter title
        Console.Write("Enter title: ");
        string title = Console.ReadLine();

        // Create a new Quiz object
        Quiz newQuiz = new Quiz
        {
            id = id,
            title = title,
            maximumScore = 0,
            isPublished = false
        };

        quizzes.Add(newQuiz);

        // Add the newly created quiz to the list of unpublished quizzes
        unpublishedQuizzes.Add(newQuiz);

        // Display a message to indicate that the quiz has been added
        Console.WriteLine("Quiz has been added.");
    }

    static Question GetQuestionById(int questionId)
    {
        string[] lines = File.ReadAllLines("questions.txt");

        foreach (var line in lines)
        {
            string[] parts = line.Split(';');
            if (parts.Length >= 2)
            {
                int id = int.Parse(parts[1]);

                // Check if the question ID matches the desired ID
                if (id == questionId)
                {
                    string type = parts[0];
                    string prompt = parts[2];
                    int points = int.Parse(parts[3]);

                    if (type == "MCQ")
                    {
                        List<string> choices = parts.Skip(4).Take(parts.Length - 5).ToList();
                        int correctAnswerIndex = int.Parse(parts[parts.Length - 1]);
                        return new MultipleChoiceQuestion(id, prompt, points, choices, correctAnswerIndex);
                    }
                    else if (type == "MRQ")
                    {
                        List<string> choicesData = parts.Skip(4).ToList();
                        List<Choice> choices = new List<Choice>();

                        foreach (string choiceData in choicesData)
                        {
                            string[] choiceParts = choiceData.Split(',');
                            if (choiceParts.Length == 2)
                            {
                                string choiceText = choiceParts[0];
                                bool isCorrect = bool.Parse(choiceParts[1]);
                                choices.Add(new Choice(choiceText, isCorrect));
                            }
                        }

                        return new MultipleResponseQuestion(id, prompt, points, choices);
                    }
                }
            }
        }

        return null; // If question with the given ID is not found
    }

    // Case 8: Add question to quiz
    static void AddQuestionToQuiz(List<Quiz> quizzes)
    {
        if (unpublishedQuizzes.Count == 0)
        {
            Console.WriteLine("No quizzes found. Please add a quiz first.");
            return;
        }

        // List all quizzes to choose from
        Console.WriteLine("Available Quizzes:");
        foreach (var quiz in quizzes)
        {
            Console.WriteLine($"Quiz ID: {quiz.id}, Title: {quiz.title}");
        }

        Console.Write("Select quiz: ");
        if (int.TryParse(Console.ReadLine(), out int quizId))
        {
            // Find the selected quiz in the list of quizzes
            Quiz selectedQuiz = quizzes.FirstOrDefault(quiz => quiz.id == quizId);

            if (selectedQuiz != null)
            {
                ListAllQuestionsFromFile();
                Console.Write("Enter the ID of the question to add: ");
                if (int.TryParse(Console.ReadLine(), out int questionId))
                {
                    // Retrieve the question based on the given ID
                    Question questionToAdd = GetQuestionById(questionId);
                    if (questionToAdd != null)
                    {
                        selectedQuiz.AddQuestion(questionToAdd);
                        Console.WriteLine("Question has been added to quiz.");
                        bool addAnotherQuestion = true;

                        while (addAnotherQuestion)
                        {
                            Console.Write("Add another question (yes/no)? ");
                            string answer = Console.ReadLine().ToLower();
                            addAnotherQuestion = (answer == "yes");
                            if (addAnotherQuestion)
                            {
                                ListAllQuestionsFromFile();
                                Console.Write("Enter the ID of the question to add: ");
                                if (int.TryParse(Console.ReadLine(), out questionId))
                                {
                                    // Retrieve the question based on the given ID
                                    questionToAdd = GetQuestionById(questionId);
                                    if (questionToAdd != null)
                                    {
                                        selectedQuiz.AddQuestion(questionToAdd);
                                        Console.WriteLine("Question has been added to quiz.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Question not found. Please enter a valid question ID.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please enter a valid numeric ID.");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Question not found. Please enter a valid question ID.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid numeric ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid quiz ID. Quiz not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid numeric ID.");
        }
    }

    static void ListAllQuizzes(List<Quiz> quizzes)
    {
        if (quizzes.Count > 0)
        {
            Console.WriteLine("All Quizzes:");
            foreach (var quiz in quizzes)
            {
                Console.WriteLine($"Quiz ID: {quiz.id}\nTitle: {quiz.title}\n");
            }

            Console.Write("Select a quiz by its ID: ");
            if (int.TryParse(Console.ReadLine(), out int selectedQuizId))
            {
                Quiz selectedQuiz = quizzes.FirstOrDefault(quiz => quiz.id == selectedQuizId);
                if (selectedQuiz != null)
                {
                    Console.WriteLine("Questions added to the selected quiz:");
                    foreach (var question in selectedQuiz.questions)
                    {
                        Console.WriteLine(question.prompt);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid quiz ID. Quiz not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid numeric ID.");
            }
        }
        else
        {
            Console.WriteLine("No quiz found");
        }
    }
    static void ListAllQuizzonly(List<Quiz> quizzes)
    {
        if (quizzes.Count > 0)
        {
            Console.WriteLine("All Quizzes:");
            foreach (var quiz in quizzes)
            {
                Console.WriteLine($"Quiz ID: {quiz.id}\nTitle: {quiz.title}\n");
            }
        }
        else
        {
            Console.WriteLine("No quiz found");
        }
    }
    static void PublishQuiz(List<Quiz> quizzes)
    {
        // Display all unpublished ques
        ListAllQuizzonly(unpublishedQuizzes);

        // Check if there are any unpublished ques
        if (unpublishedQuizzes.Count == 0)
        {
            Console.WriteLine("No unpublished ques found. Please add a quiz first.");
            return;
        }

        // Prompt the user to select a quiz by its ID
        Console.Write("Select quiz: ");
        int selectedQuizId = int.Parse(Console.ReadLine());

        // Find the selected quiz in the list of unpublished ques
        Quiz selectedQuiz = unpublishedQuizzes.FirstOrDefault(quiz => quiz.id == selectedQuizId);

        if (selectedQuiz != null)
        {
            // Change the quiz status to published
            selectedQuiz.isPublished = true;

            // Remove the published quiz from the list of unpublished ques
            unpublishedQuizzes.Remove(selectedQuiz);

            Console.WriteLine("Quiz has been published.");
        }
        else
        {
            Console.WriteLine("Invalid quiz ID. Quiz not found.");
        }
    }

    static void StartQuiz(List<Quiz> quizzes)
    {
        // Filter published quizzes
        List<Quiz> publishedQuizzes = quizzes.Where(quiz => quiz.isPublished).ToList();

        // Check if there are any published quizzes
        if (publishedQuizzes.Count == 0)
        {
            Console.WriteLine("No published quizzes found. Please publish a quiz first.");
            return;
        }

        // Display all published quizzes
        ListAllQuizzonly(publishedQuizzes);

        // Prompt the user to select a quiz by its ID
        Console.Write("Select quiz: ");
        int selectedQuizId;
        if (int.TryParse(Console.ReadLine(), out selectedQuizId))
        {
            // Find the selected quiz in the list of published quizzes
            Quiz selectedQuiz = publishedQuizzes.FirstOrDefault(quiz => quiz.id == selectedQuizId);

            if (selectedQuiz != null)
            {
                // Display quiz information
                Console.WriteLine($"Id from the StartQuiz function: {selectedQuiz.id}");
                Console.WriteLine($"Title: {selectedQuiz.title}");
                Console.WriteLine($"MaximumScore: {selectedQuiz.maximumScore}");
                Console.WriteLine($"IsPublished: {selectedQuiz.isPublished}");

                // Create a new quiz attempt object for the selected quiz
                QuizAttempt attempt = new QuizAttempt(selectedQuiz);

                // Loop through each question in the quiz and prompt the user for answers
                foreach (var question in selectedQuiz.questions)
                {
                    Console.WriteLine(question.prompt);

                    // Display choices if the question is a MultipleChoiceQuestion or MultipleResponseQuestion
                    if (question is MultipleChoiceQuestion mcq)
                    {
                        for (int i = 0; i < mcq.choices.Count; i++)
                        {
                            var choice = mcq.choices[i];
                            Console.WriteLine($"{(char)('A' + i)}. {choice}");
                        }

                        // Prompt the user to enter the answer(s) to the question
                        Console.Write("Enter option(s): ");
                        string userInput = Console.ReadLine().ToUpper();

                        // Convert the user input (e.g., "A, B") into a list of selected choices (e.g., "0, 1")
                        string[] userChoiceLetters = userInput.Split(',');
                        List<string> userAnswers = new List<string>();
                        foreach (string choiceLetter in userChoiceLetters)
                        {
                            int choiceIndex = choiceLetter[0] - 'A' + 1;
                            if (choiceIndex >= 0 && choiceIndex < mcq.choices.Count)
                            {
                                userAnswers.Add(choiceIndex.ToString());
                            }
                        }

                        attempt.questionScores.Add(mcq.CheckAnswer(userAnswers));
                    }
                    else if (question is MultipleResponseQuestion mrq)
                    {
                        for (int i = 0; i < mrq.choices.Count; i++)
                        {
                            var choice = mrq.choices[i];
                            Console.WriteLine($"{(char)('A' + i)}. {choice.text}");
                        }

                        // Prompt the user to enter the answer(s) to the question
                        Console.Write("Enter option(s): ");
                        string userInput = Console.ReadLine().ToUpper();

                        // Convert the user input (e.g., "A, B") into a list of selected choices
                        string[] userChoiceLetters = userInput.Split(',');
                        List<string> userAnswers = new List<string>();

                        foreach (string choiceLetter in userChoiceLetters)
                        {
                            int choiceIndex = choiceLetter[0] - 'A';
                            if (choiceIndex >= 0 && choiceIndex < mrq.choices.Count)
                            {
                                userAnswers.Add(choiceIndex.ToString());
                            }
                        }

                        // Calculate the total score for the attempt using the user's selected choices
                        int score = mrq.CheckAnswer(userAnswers);

                        // Display the points earned for this question
                        Console.WriteLine($"Points: {score}\n");

                        // Add the score to the attempt's questionScores
                        attempt.questionScores.Add(score);
                    }

                }

                    // Calculate the total score for the attempt
                    attempt.totalScore = attempt.questionScores.Sum();

                     // Display the total score for the quiz attempt
                      Console.WriteLine($"Total score: {attempt.totalScore} out of {selectedQuiz.maximumScore}\n");

                // Add the attempt to the list of quiz attempts for the selected quiz
                selectedQuiz.quizAttempts.Add(attempt);
            }
            else
            {
                Console.WriteLine("Invalid quiz ID. Quiz not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid numeric ID.");
        }
    }



    static void ListQuizScores(List<Quiz> quizzes)
    {
        // Filter published ques
        List<Quiz> publishedQuizzes = quizzes.Where(quiz => quiz.isPublished).ToList();

        // Check if there are any published ques
        if (publishedQuizzes.Count == 0)
        {
            Console.WriteLine("No published ques found. Please publish a quiz first.");
            return;
        }

        // Display all published ques
        ListAllQuizzonly(publishedQuizzes);

        // Prompt the user to select a quiz by its ID
        Console.Write("Select quiz: ");
        int selectedQuizId = int.Parse(Console.ReadLine());

        // Find the selected quiz in the list of published ques
        Quiz selectedQuiz = publishedQuizzes.FirstOrDefault(quiz => quiz.id == selectedQuizId);

        if (selectedQuiz != null)
        {
            // Check if there are any quiz attempts for the selected quiz
            if (selectedQuiz.quizAttempts.Count == 0)
            {
                Console.WriteLine("No quiz attempts found for the selected quiz.");
            }
            else
            {
                // Display information for all quiz attempts
                foreach (var attempt in selectedQuiz.quizAttempts)
                {
                    Console.WriteLine($"Attempt Date: {attempt.dateCreated}");
                    Console.WriteLine($"Total Score: {attempt.totalScore} out of {selectedQuiz.maximumScore}");
                    Console.WriteLine("------------------------------");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid quiz ID. Quiz not found.");
        }
    }


    // Function to allow the user to edit the text of a question
    static void EditQuestionText()
    {
        // Display all unpublished ques
        ListAllQuizzonly(unpublishedQuizzes);

        // Check if there are any unpublished ques
        if (unpublishedQuizzes.Count == 0)
        {
            Console.WriteLine("No unpublished quiz found. Please add a quiz first.");
            return;
        }
        else
        {
            // Prompt the user to select a quiz by its ID
            Console.Write("Select quiz: ");
            int selectedQuizId = int.Parse(Console.ReadLine());

            // Find the selected quiz in the list of unpublished ques
            Quiz selectedQuiz = unpublishedQuizzes.FirstOrDefault(quiz => quiz.id == selectedQuizId);

            if (selectedQuiz != null)
            {
                // Display the questions in the selected quiz
                Console.WriteLine("Questions in the selected quiz:");
                foreach (var question in selectedQuiz.questions)
                {
                    Console.Write(question.id);
                    Console.WriteLine("    " + question.prompt);
                }

                // Prompt the user to enter the ID of the question they want to edit
                Console.Write("Enter the ID of the question you want to edit: ");
                int selectedQuestionId = int.Parse(Console.ReadLine());

                // Find the selected question in the list of all questions
                Question selectedQuestion = selectedQuiz.questions.FirstOrDefault(question => question.id == selectedQuestionId);

                if (selectedQuestion != null)
                {
                    // Prompt the user to enter the new text for the question
                    Console.Write("Enter the new text for the question: ");
                    string newQuestionText = Console.ReadLine();

                    // Update the question text
                    selectedQuestion.prompt = newQuestionText;

                    Console.WriteLine("Question text updated.");
                }
                else
                {
                    Console.WriteLine("Invalid question ID. Question not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid quiz ID. Quiz not found.");
            }
        }
    }

    // Function to randomize the order of questions in a quiz
    static void RandomizeQuizQuestionsOrder()
    {
        // Display all unpublished ques
        ListAllQuizzes(unpublishedQuizzes);

        // Check if there are any unpublished ques
        if (unpublishedQuizzes.Count == 0)
        {
            Console.WriteLine("No unpublished quiz found. Please add a quiz first.");
            return;
        }
        else
        {
            // Prompt the user to select a quiz by its ID
            Console.Write("Select quiz: ");
            int selectedQuizId = int.Parse(Console.ReadLine());

            // Find the selected quiz in the list of unpublished ques
            Quiz selectedQuiz = unpublishedQuizzes.FirstOrDefault(quiz => quiz.id == selectedQuizId);

            if (selectedQuiz != null)
            {
                // Randomize the order of questions using Fisher-Yates shuffle algorithm
                Random rng = new Random();
                int n = selectedQuiz.questions.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    var value = selectedQuiz.questions[k];
                    selectedQuiz.questions[k] = selectedQuiz.questions[n];
                    selectedQuiz.questions[n] = value;
                }

                Console.WriteLine("Quiz questions order randomized.");
            }
            else
            {
                Console.WriteLine("Invalid quiz ID. Quiz not found.");
            }
        }
    }

    // Function to allow the user to remove a question from a quiz
    static void RemoveQuestionFromQuiz()
    {
        // Display all unpublished ques
        ListAllQuizzonly(unpublishedQuizzes);

        // Check if there are any unpublished ques
        if (unpublishedQuizzes.Count == 0)
        {
            Console.WriteLine("No unpublished quiz found. Please add a quiz first.");
            return;
        }

        // Prompt the user to select a quiz by its ID
        Console.Write("Select quiz: ");
        int selectedQuizId;
        if (!int.TryParse(Console.ReadLine(), out selectedQuizId))
        {
            Console.WriteLine("Invalid input. Please enter a valid quiz ID.");
            return;
        }

        // Find the selected quiz in the list of unpublished ques
        Quiz selectedQuiz = unpublishedQuizzes.FirstOrDefault(quiz => quiz.id == selectedQuizId);

        if (selectedQuiz != null)
        {
            // Display the questions in the selected quiz
            Console.WriteLine("Questions in the selected quiz:");
            foreach (var question in selectedQuiz.questions)
            {
                Console.Write(question.id);
                Console.WriteLine("    "+ question.prompt);
            }

            // Prompt the user to enter the ID of the question they want to remove
            Console.Write("Enter the ID of the question you want to remove: ");
            int selectedQuestionId;
            if (!int.TryParse(Console.ReadLine(), out selectedQuestionId))
            {
                Console.WriteLine("Invalid input. Please enter a valid question ID.");
                return;
            }

            // Find the selected question in the list of questions
            Question selectedQuestion = selectedQuiz.questions.FirstOrDefault(question => question.id == selectedQuestionId);

            if (selectedQuestion != null)
            {
                // Remove the question from the quiz
                selectedQuiz.questions.Remove(selectedQuestion);

                Console.WriteLine("Question removed from the quiz.");
            }
            else
            {
                Console.WriteLine("Invalid question ID. Question not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid quiz ID. Quiz not found.");
        }
    }

    static void ListAllQuestionsFromFile()
    {
        try
        {
            string[] lines = File.ReadAllLines("questions.txt");

            // Initialize variables to keep track of found questions
            bool foundMultipleChoiceQuestions = false;
            bool foundMultipleResponseQuestions = false;

            // Iterate through each line in the "questions.txt" file
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(';');

                // Ensure the line has enough parts to represent a valid question
                if (parts.Length >= 5)
                {
                    string type = parts[0];
                    int id = int.Parse(parts[1]);
                    string prompt = parts[2];
                    int points = int.Parse(parts[3]);

                    // Display question type and ID based on 'type' value
                    if (type == "MCQ")
                    {
                        foundMultipleChoiceQuestions = true;
                        Console.WriteLine($"Multiple choice question (ID: {id})");
                        Console.WriteLine($"Prompt: {prompt}\tPoints: {points}");

                        // Initialize a letter to represent the options (A, B, C, etc.)
                        char optionLetter = 'A';

                        // Determine the number of choices based on question type
                        int numChoices = parts.Length - 5;

                        // Iterate through each choice in the question
                        for (int j = 0; j < numChoices; j++)
                        {
                            string choiceText = parts[j + 4];
                            bool isCorrect = j == int.Parse(parts[parts.Length - 1]);

                            // Display each choice with its corresponding option letter and indication of whether it's correct or not
                            Console.WriteLine($"{optionLetter}. {choiceText}\t({(isCorrect ? "yes" : "no")})");
                            optionLetter++;
                        }

                        // For multiple-choice questions, display the correct option letter
                        if (int.TryParse(parts[parts.Length - 1], out int correctAnswerIndex) && correctAnswerIndex >= 0 && correctAnswerIndex < numChoices)
                        {
                            char correctOption = (char)('A' + correctAnswerIndex);
                            Console.WriteLine($"Correct option: {correctOption}\n");
                        }
                    }
                    else if (type == "MRQ")
                    {
                        foundMultipleResponseQuestions = true;
                        Console.WriteLine($"Multiple response question (ID: {id})");
                        Console.WriteLine($"Prompt: {prompt}\tPoints: {points}");

                        // Initialize a letter to represent the options (A, B, C, etc.)
                        char optionLetter = 'A';

                        // Determine the number of choices based on question type
                        int numChoices = parts.Length - 4;

                        // Iterate through each choice in the question
                        for (int j = 0; j < numChoices; j++)
                        {
                            string[] choiceParts = parts[j + 4].Split(',');
                            if (choiceParts.Length != 2)
                            {
                                // Skip this choice if it's not in the correct format
                                continue;
                            }

                            string choiceText = choiceParts[0];
                            bool isCorrect = bool.Parse(choiceParts[1]);

                            // Display each choice with its corresponding option letter and indication of whether it's correct or not
                            Console.WriteLine($"{optionLetter}. {choiceText}\t({(isCorrect ? "yes" : "no")})");
                            optionLetter++;
                        }

                        // For multiple-response questions, display the correct answer(s)
                        List<string> correctAnswers = new List<string>();
                        for (int j = 0; j < numChoices; j++)
                        {
                            string[] choiceParts = parts[j + 4].Split(',');
                            if (choiceParts.Length != 2)
                            {
                                // Skip this choice if it's not in the correct format
                                continue;
                            }

                            bool isCorrect = bool.Parse(choiceParts[1]);
                            if (isCorrect)
                            {
                                correctAnswers.Add(((char)('A' + j)).ToString());
                            }
                        }
                        Console.WriteLine($"Correct answer(s): {string.Join(", ", correctAnswers)}\n");
                    }
                }
            }

            // Display a message if no questions were found
            if (!foundMultipleChoiceQuestions && !foundMultipleResponseQuestions)
            {
                Console.WriteLine("No questions found in the file.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading questions from file: " + ex.Message);
        }
    }
    static void ListAllMultipleResponseQuestions(List<Quiz> ques)
    {
        bool foundQuestions = false;

        // Iterate through each quiz in the list of ques
        foreach (var quiz in ques)
        {
            // Iterate through each question in the current quiz's list of questions
            foreach (var question in quiz.questions)
            {
                // Check if the current question is a multiple-response question
                if (question is MultipleResponseQuestion mrq)
                {
                    foundQuestions = true;
                    Console.WriteLine("Multiple response question :");

                    // Display the question prompt and its point value
                    Console.WriteLine($"{question.prompt}\tPoints: {question.points}");

                    // Initialize a letter to represent the options (A, B, C, etc.)
                    char optionLetter = 'A';

                    // Iterate through each choice in the multiple-response question
                    foreach (var choice in mrq.choices)
                    {
                        // Display each choice with its corresponding option letter and indication of whether it's correct or not
                        Console.WriteLine($"{optionLetter}. {choice.text}\t({(choice.isCorrect ? "yes" : "no")})");
                        optionLetter++;
                    }

                    // Display the correct answer(s) for multiple-response questions
                    Console.Write("Correct answer(s): ");
                    var correctAnswers = mrq.choices.Where(choice => choice.isCorrect).Select(choice => (char)('A' + mrq.choices.IndexOf(choice))).ToList();
                    Console.WriteLine(string.Join(", ", correctAnswers));
                    Console.WriteLine();
                }
            }
        }

        // If no multiple-response questions were found, display a message
        if (!foundQuestions)
        {
            Console.WriteLine("No multiple-response questions found");
        }
    }


    static void ListAllMultipleChoiceQuestions(List<Quiz> questions)
    {
        bool foundQuestions = false;

        // Iterate through each quiz in the list of ques
        foreach (var quiz in questions)
        {
            // Iterate through each question in the current quiz's list of questions
            foreach (var question in quiz.questions)
            {
                // Check if the current question is a multiple-choice question
                if (question is MultipleChoiceQuestion mcq)
                {
                    foundQuestions = true;
                    Console.WriteLine("Multiple choice question :");

                    // Display the question prompt and its point value
                    Console.WriteLine($"{question.prompt}\tPoints: {question.points}");

                    // Initialize a letter to represent the options (A, B, C, etc.)
                    char optionLetter = 'A';

                    // Iterate through each choice in the multiple-choice question
                    foreach (string choice in mcq.choices)
                    {
                        // Display each choice with its corresponding option letter
                        Console.WriteLine($"{optionLetter}. {choice}");
                        optionLetter++;
                    }

                    // Calculate the correct option letter based on the index
                    char correctOption = (char)('A' + mcq.correctAnswerIndex);

                    // Display the correct option letter
                    Console.WriteLine($"Correct option: {correctOption}\n");
                }
            }
        }

        // If no multiple-choice questions were found, display a message
        if (!foundQuestions)
        {
            Console.WriteLine("No multiple-choice questions found");
        }
    }
}