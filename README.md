# Project Readme

## Repository Link and Commit History
Please refer to the following link for access to the project repository and commit history, which documents each step of the development process and shows the progression towards a functioning prototype:
[**Repository Link**](https://your-repo-link-here)

## Frontend-Only Components
The following components are primarily for display purposes and do not have full backend integration:
- **Respondent Registration**: This part of the app currently simulates registration functionality and does not yet interact with the database.
- **Staff Search**: This interface element is also for front-end demonstration only, without backend search capabilities implemented at this stage.

## Project Structure and Development Notes
Due to time constraints, the focus was placed on building the core functionality of the questionnaire system. Some areas of the code may lack organization and refinement, as this prototype was aimed at delivering the essential features first. Future iterations will address code organization, optimization, and more robust project structure. Thank you for understanding that this prototype prioritizes functionality over final polish.

## Database Schema Overview
The **database schema** has been designed to fulfill all outlined test cases without requiring modification. Below is a breakdown of how key columns in the **Question** table operate, which is essential to understanding the functionality and flow of the questionnaire:

- **OrderPos**: This column sets the sequence in which questions are displayed. By adjusting this value, you can control the order of questions in the questionnaire.

- **Question Type**: Each question type plays a critical role, especially in cases with sub-questions. For example, if a main question has a type `"Bank"`, related sub-questions will have types like `"Bank_Service"`. This ensures that sub-questions are dynamically linked and displayed only when relevant.

- **MinAnswers and MaxAnswers**: These columns also serve to help define whether a question is mandatory. If `MinAnswers` is set to 1, the question will be treated as required. Optional questions would be defined by a `MinAnswers` value of 0.

- **PossibleAnswers**: This column is used to define the options available for checkbox-based questions. Data in this column is structured in a comma-separated format like `"yes,no"`, which renders as multiple checkbox choices.

- **CustomAnswer**: This boolean value specifies whether the question allows for a custom answer entry (text input) instead of predefined options. If set to `true`, the answer field will display a free-text input for the respondent.

Please take a moment to review these details to get an understanding of how each component influences the overall project.

---

Thank you for your time in reviewing this prototype. I am eager for feedback and any suggestions for refining the next iteration.
