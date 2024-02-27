# Quiz and Question Management System

This program is designed to manage quizzes and questions. It allows users to create, edit, and publish quizzes, add questions to quizzes, start quizzes, and view quiz scores.

## Features

- **Multiple Choice Questions**: Users can add, list, and manage multiple-choice questions.
- **Multiple Response Questions**: Users can add, list, and manage multiple-response questions.
- **Quiz Management**: Users can add, list, and publish quizzes.
- **Question Management**: Users can edit questions, remove questions, and randomize questions within a quiz.
- **Quiz Attempt Tracking**: The system records quiz attempts along with scores and submission dates.

## Classes

### Quiz
- Represents a quiz with an ID, title, maximum score, and a list of questions.
- Supports methods to add questions and display quiz details.

### QuizAttempt
- Represents an attempt made by a user to complete a quiz.
- Records the total score, date created, and individual question scores.

### Question
- Base class for different types of questions.
- Contains properties for question ID, prompt, and points.

### MultipleChoiceQuestion
- Inherits from Question.
- Represents a multiple-choice question with a list of choices and a correct answer index.

### MultipleResponseQuestion
- Inherits from Question.
- Represents a multiple-response question with a list of choices, some of which may be correct.

### Choice
- Represents a choice for a multiple-choice or multiple-response question.
- Contains text and a flag indicating whether it is correct.

### Program
- Main class containing the entry point and main menu functionality.
- Allows users to interact with quizzes and questions through console commands.

## Usage

1. Run the program.
2. Choose an option from the main menu to perform the desired action.
3. Follow the prompts to add, edit, or manage quizzes and questions.

## Main Menu Options

1. List all multiple-choice questions
2. List all multiple-response questions
3. Add multiple-choice question
4. Add multiple-response question
5. Load questions from file
6. List all quizzes
7. Add quiz
8. Add question to quiz
9. Publish quiz
10. Start quiz
11. List quiz scores
12. Exit the program
13. Edit a question
14. Randomize Questions within a quiz
15. Remove a Question from a quiz

