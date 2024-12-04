open System
open System.IO
open System.Drawing
open System.Windows.Forms

type UserInfo = { Name: string; ID: string }
type Question = {
    Text: string
    Options: string list option
    CorrectAnswer: string
    QuestionType: string
    Score: int
}

let questions =
    Map.ofList [
        0, { Text = "What is the capital of France?"; Options = Some ["Paris"; "London"; "Berlin"; "Madrid"]; CorrectAnswer = "Paris"; QuestionType = "MultipleChoice"; Score = 1 }
        1, { Text = "Solve: 5 + 3 = ?"; Options = None; CorrectAnswer = "8"; QuestionType = "Written"; Score = 2 }
        2, { Text = "Which language is functional?"; Options = Some ["F#"; "Python"; "Java"; "C#"]; CorrectAnswer = "F#"; QuestionType = "MultipleChoice"; Score = 1 }
        3, { Text = "What is your favorite programming language?"; Options = None; CorrectAnswer = "F#"; QuestionType = "Written"; Score = 2 }
        4, { Text = "Explain the concept of Polymorphism in OOP."; Options = None; CorrectAnswer = "Polymorphism allows objects of different classes to be treated as objects of a common superclass. It is one of the key features of Object-Oriented Programming."; QuestionType = "Written"; Score = 3 }
        5, { Text = "Describe the difference between a stack and a queue."; Options = None; CorrectAnswer = "A stack follows Last In, First Out (LIFO), while a queue follows First In, First Out (FIFO)."; QuestionType = "Written"; Score = 3 }
        6, { Text = "Which of the following is a functional programming language?"; Options = Some ["F#"; "Java"; "C#"; "Ruby"]; CorrectAnswer = "F#"; QuestionType = "MultipleChoice"; Score = 1 }
        7, { Text = "Which of these is not an OOP concept?"; Options = Some ["Inheritance"; "Encapsulation"; "Recursion"; "Abstraction"]; CorrectAnswer = "Recursion"; QuestionType = "MultipleChoice"; Score = 1 }
    ]

let createQuizForm (userInfo: UserInfo) (questions: Map<int, Question>) =
    let form = new Form(Text = sprintf "Quiz - %s" userInfo.Name, Size = Size(500, 450), StartPosition = FormStartPosition.CenterScreen)
    let userId = userInfo.ID
    let Name = userInfo.Name

    
    let mutable userAnswers = Map.empty<int, string>
    let mutable currentIndex = 1
    let mutable score = 0

    
    let scoreTrackerLabel = new Label(Location = Point(20, 20), AutoSize = true, Font = new Font("Arial", 14.0f, FontStyle.Bold), Text = "Score: 0")
    let questionLabel = new Label(Location = Point(20, 50), AutoSize = true, Font = new Font("Arial", 12.0f))
    let optionsPanel = new FlowLayoutPanel(Location = Point(20, 100), Size = Size(460, 100))
    let answerTextBox = new TextBox(Location = Point(20, 220), Size = Size(200, 30))
    let nextButton = new Button(Text = "Next", Location = Point(250, 220), BackColor = Color.LightBlue)
    let backButton = new Button(Text = "Back", Location = Point(340, 220), BackColor = Color.LightGray)
    let finishButton = new Button(Text = "Finish", Location = Point(430, 220), BackColor = Color.LightGreen)
    let scoreLabel = new Label(Location = Point(20, 270), AutoSize = true, Font = new Font("Arial", 12.0f))


    let updateScoreDisplay () =
        scoreTrackerLabel.Text <- sprintf "Score: %d" score