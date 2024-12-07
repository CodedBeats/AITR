# AIT Research (AITR)

## Repository Link and Commit History
Please refer to the following link for access to the project repository and commit history.
[**Repository Link**](https://github.com/CodedBeats/AITR)

## Frontend-Only Components
I know front end style gives no marks, but I hope you enjoy it nonetheless.

## Database Schema Overview
The **database schema** has been designed to fulfill all outlined test cases without requiring modification. Below is a breakdown of how key columns in the **Question** and **QuestionType** tables operate, which is essential to understanding the functionality and flow of the questionnaire:

- **OrderPos**: This column sets the sequence in which questions are displayed. By adjusting this value, you can control the order of questions in the questionnaire.

- **Question Type**: Each question type plays a critical role, especially in cases with sub-questions. For example, if a main question has a type `"Bank"`, related sub-questions will have types like `"Bank_Service"`. This ensures that sub-questions are dynamically linked and displayed only when relevant.

- **PossibleAnswers**: This column is used to define the options available for checkbox and dropdown based questions. Data in this column is structured in a symbol-seperated format like `"yes,no"` or `"x|y|z"`. This renders as checkbox options and dropdown options respectively.

Please take a moment to review these details to get an understanding of how each component influences the overall project.

---

I hope you have a nice day.
