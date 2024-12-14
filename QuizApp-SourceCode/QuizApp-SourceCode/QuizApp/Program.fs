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
        3, { Text = "Solve: 100 - 2 = ?"; Options = None; CorrectAnswer = "98"; QuestionType = "Written"; Score = 2 }
        4, { Text = "What is Recursion Key Word In F# ?"; Options = None; CorrectAnswer = "rec"; QuestionType = "Written"; Score = 3 }
        5, { Text = "The ... keyword is used in binding expressions to define values or function values for one or more names."; Options = None; CorrectAnswer = "let"; QuestionType = "Written"; Score = 3 }
        6, { Text = "Which of the following is a functional programming language?"; Options = Some ["F#"; "Java"; "C#"; "Ruby"]; CorrectAnswer = "F#"; QuestionType = "MultipleChoice"; Score = 1 }
        7, { Text = "Which of these is not an OOP concept?"; Options = Some ["Inheritance"; "Encapsulation"; "Recursion"; "Abstraction"]; CorrectAnswer = "Recursion"; QuestionType = "MultipleChoice"; Score = 1 }
        8, { Text = "Solve: 2 x 5 = ?"; Options = None; CorrectAnswer = "10"; QuestionType = "Written"; Score = 2 }
        9, { Text = "Solve: 10 - 3 = ?"; Options = None; CorrectAnswer = "7"; QuestionType = "Written"; Score = 2 }
        10, { Text = "How To Define Mutable Variable in F# ?"; Options = Some ["var";"let";"let mutable";"rec"]; CorrectAnswer = "let mutable"; QuestionType = "MultipleChoice"; Score = 2 }

    ]

let createQuizForm (userInfo: UserInfo) (questions: Map<int, Question>) =
    let form = new Form(Text = sprintf "Quiz - %s-%s { 20 Marks }" userInfo.Name userInfo.ID, Size = Size(550, 450), StartPosition = FormStartPosition.CenterScreen)
    let userId = userInfo.ID
    let Name = userInfo.Name

    // Store user answers
    let mutable userAnswers = Map.empty<int, string>
    let mutable currentIndex = 0
    let mutable score = 0

    // Track if a question is answered
    let mutable answeredQuestions = Map.empty<int, bool>

    // UI elements
    //let scoreTrackerLabel = new Label(Location = Point(20, 20), AutoSize = true, Font = new Font("Arial", 14.0f, FontStyle.Bold), Text = "Score: 0")
    let timeLabel = new Label(Location = Point(40, 20), AutoSize = true, Font = new Font("Arial", 15.0f, FontStyle.Bold), ForeColor = Color.Red)
    let questionLabel = new Label(Location = Point(20, 50), AutoSize = true, Font = new Font("Arial", 12.0f))
    let optionsPanel = new FlowLayoutPanel(Location = Point(20, 100), Size = Size(460, 100))
    let answerTextBox = new TextBox(Location = Point(20, 220), Size = Size(200, 30))
    let nextButton = new Button(Text = "Next", Location = Point(250, 220), BackColor = Color.LightBlue)
    let backButton = new Button(Text = "Back", Location = Point(340, 220), BackColor = Color.LightGray)
    let finishButton = new Button(Text = "Finish", Location = Point(430, 220), BackColor = Color.LightGreen)
    let scoreLabel = new Label(Location = Point(20, 270), AutoSize = true, Font = new Font("Arial", 12.0f))
    let progressBar = new ProgressBar(Location=Point(20,300), BackColor = Color.White , Size = Size(300, 30))
    let timer = new Timer(Interval = 1000) // Timer with 1-second interval

    let mutable remainingTime = 3 * 60 // 3 minutes in seconds

    timer.Tick.Add(fun _ ->
        if remainingTime > 0 then
            remainingTime <- remainingTime - 1
            let minutes = remainingTime / 60
            let seconds = remainingTime % 60
            timeLabel.Text <- sprintf "%02d:%02d" minutes seconds // Update a label with the remaining time
        else
            timer.Stop() // Stop the timer when time is over
            finishButton.PerformClick() // Simulate clicking the finish button
        
    )

    timer.Start() // Start the timer

    // Update score display
    (*let updateScoreDisplay () =
        scoreTrackerLabel.Text <- sprintf "Score: %d" score*)

    // Display the current question
    let displayQuestion index =
        if questions.ContainsKey(index) then
            let question = questions.[index]
            questionLabel.Text <- sprintf "%s (Score: %d)" question.Text question.Score
            optionsPanel.Controls.Clear()

            match question.QuestionType with
            | "MultipleChoice" ->
                answerTextBox.Visible <- false
                question.Options
                |> Option.iter (fun options ->
                    options |> List.iter (fun option ->
                        let radioButton = new RadioButton(Text = option, AutoSize = true)
                        optionsPanel.Controls.Add(radioButton)))
            | "Written" ->
                answerTextBox.Visible <- true
            | _ -> answerTextBox.Visible <- false

    nextButton.Click.Add(fun _ ->
    if questions.ContainsKey(currentIndex) then
        let question = questions.[currentIndex]

        // Only process if the question has not been answered
        if not (answeredQuestions |> Map.tryFind currentIndex |> Option.defaultValue false) then
            match question.QuestionType with
            | "MultipleChoice" ->
                let selectedOption =
                    optionsPanel.Controls
                    |> Seq.cast<Control>
                    |> Seq.tryFind (fun c -> (c :?> RadioButton).Checked)
                    |> Option.map (fun c -> c.Text)

                // Track the user's answer
                userAnswers <- userAnswers.Add(currentIndex, selectedOption |> Option.defaultValue "")

                if selectedOption |> Option.exists (fun option -> 
                    not (String.IsNullOrWhiteSpace(option)) && option = question.CorrectAnswer) then
                    score <- score + question.Score
                    //updateScoreDisplay ()

            | "Written" ->
                userAnswers <- userAnswers.Add(currentIndex, answerTextBox.Text.Trim())
                
                if not (String.IsNullOrWhiteSpace(answerTextBox.Text)) && answerTextBox.Text.Trim().ToLower() = question.CorrectAnswer then
                    score <- score + question.Score
                    //updateScoreDisplay ()


            | _ -> ()
            progressBar.Value <- Math.Min(progressBar.Value + 10, 100)
            // Mark question as answered
            answeredQuestions <- answeredQuestions.Add(currentIndex, true)

        answerTextBox.Clear()
        currentIndex <- currentIndex + 1
        

        if questions.ContainsKey(currentIndex) then
            displayQuestion currentIndex
        else
            scoreLabel.Text <- sprintf "Your score: %d" score
            nextButton.Enabled <- false
            backButton.Enabled <- false
            )
            


    backButton.Click.Add(fun _ ->
    if currentIndex > 0 then
        progressBar.Value <- Math.Min(progressBar.Value - 10, 100)

        // Update current index to the previous question
        currentIndex <- currentIndex - 1

        // Check if the question exists
        if questions.ContainsKey(currentIndex) then
            let question = questions.[currentIndex]

            // Check if the question was answered
            if userAnswers.ContainsKey(currentIndex) then
                let userAnswer = userAnswers.[currentIndex]

                // Adjust score based on the correctness of the answer
                match question.QuestionType with
                | "MultipleChoice" ->
                    if userAnswer = question.CorrectAnswer then
                        score <- score - question.Score

                | "Written" ->
                    if userAnswer.ToLower() = question.CorrectAnswer.ToLower() then
                        score <- score - question.Score

                | _ -> ()

                // Ensure the score remains non-negative
                if score < 0 then
                    score <- 0

                // Update the score display
                //updateScoreDisplay ()

                // Mark the question as unanswered to allow recalculation
                answeredQuestions <- answeredQuestions.Remove(currentIndex)

            // Display the previous question
            displayQuestion currentIndex)

    let appendToFile filePath (text: string) =
        try
            // Ensure the directory exists
            let directory = Path.GetDirectoryName(filePath)
            if not (Directory.Exists(directory)) then
                Directory.CreateDirectory(directory) |> ignore

            // Append the text to the file
            use writer = new StreamWriter(filePath, append = true)
            writer.WriteLine(text)
        with
        | ex -> Console.WriteLine("An error occurred: " + ex.Message)

    finishButton.Click.Add(fun _ ->
        let finalMessage = 
            let answersMessage =
                questions
                |> Map.fold (fun acc index question ->
                    let userAnswer = userAnswers |> Map.tryFind index |> Option.defaultValue "No Answer"
                    let correctAnswer = question.CorrectAnswer
                    let userAnswerMessage = sprintf "Q%d: %s\nYour Answer: %s\nCorrect Answer: %s\n\n" (index + 1) question.Text userAnswer correctAnswer
                    acc + userAnswerMessage) ""
            timer.Stop()
            let message = sprintf "Your final score is %d \n Quiz Finished \n Name: %s \n ID: %s \n\n%s" score Name userId answersMessage
            let res = $"Name: {Name}\tID: {userId}\tScore: {score}"
            // Call the function to append the message to the file
            appendToFile @"C:\Results\res.txt" res
            MessageBox.Show(message) |> ignore
            Application.Exit()
            
        form.Close())

    displayQuestion currentIndex
    form.Controls.AddRange([| (*scoreTrackerLabel;*) timeLabel;progressBar;questionLabel; optionsPanel; answerTextBox; nextButton; backButton; finishButton; scoreLabel |])
    form.ShowDialog() |> ignore

let createMainForm () =
    let form = new Form(Text = "Quiz Application", Size = Size(500, 450), StartPosition = FormStartPosition.CenterScreen)
    let nameLabel = new Label(Text = "Name:", Location = Point(20, 100), AutoSize = true, Font = new Font("Arial", 12.0f))
    let nameTextBox = new TextBox(Location = Point(120, 100), Size = Size(300, 30))
    let idLabel = new Label(Text = "ID:", Location = Point(20, 150), AutoSize = true, Font = new Font("Arial", 12.0f))
    let idTextBox = new TextBox(Location = Point(120, 150), Size = Size(300, 30))
    let startButton = new Button(Text = "Start", Location = Point(200, 250), Font = new Font("Arial", 10.0f, FontStyle.Bold), BackColor = Color.GreenYellow)

    startButton.Click.Add(fun _ ->
        if not (String.IsNullOrWhiteSpace(nameTextBox.Text) || String.IsNullOrWhiteSpace(idTextBox.Text)) then
            let userInfo = { Name = nameTextBox.Text; ID = idTextBox.Text }
            createQuizForm userInfo questions |> ignore
        else
            MessageBox.Show("Please enter your name and ID.", "Error") |> ignore)

    form.Controls.AddRange([| nameLabel; nameTextBox; idLabel; idTextBox; startButton |])
    form

[<EntryPoint>]
let main _ =
    Application.Run(createMainForm())
    0
