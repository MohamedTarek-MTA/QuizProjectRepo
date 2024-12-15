open System
open System.IO
open System.Drawing
open System.Windows.Forms

type UserInfo = { Name: string; ID: string ; ProfName:string }
type Question = {
    Text: string
    Options: string list option
    CorrectAnswer: string
    QuestionType: string
    Score: int
}

let questions =
    Map.ofList [
        0, { Text = "What is F# primarily known for?"; Options = Some ["Object-Oriented Programming"; "Functional Programming"; "Low-Level Programming"; "Data Science"]; CorrectAnswer = "Functional Programming"; QuestionType = "MultipleChoice"; Score = 1 }
        1, { Text = "What is the immutable collection type in F#?"; Options = None; CorrectAnswer = "list"; QuestionType = "Written"; Score = 2 }
        2, { Text = "Which language is Declarative Programming?"; Options = Some ["F#"; "Python"; "Java"; "C#"]; CorrectAnswer = "F#"; QuestionType = "MultipleChoice"; Score = 1 }
        3, { Text = "What is the data structure used for key-value pairs in F#?"; Options = None; CorrectAnswer = "map"; QuestionType = "Written"; Score = 2 }
        4, { Text = "What is Recursion Key Word In F# ?"; Options = None; CorrectAnswer = "rec"; QuestionType = "Written"; Score = 3 }
        5, { Text = "How do you define a function in F#?"; Options = None; CorrectAnswer = "let"; QuestionType = "Written"; Score = 3 }
        6, { Text = "What is the correct extension for F# files?"; Options = Some [".cs"; ".fs"; ".fsharp"; ".f#"]; CorrectAnswer = ".fs"; QuestionType = "MultipleChoice"; Score = 1 }
        7, { Text = "Which of the following is a feature of F#?"; Options = Some ["Static typing"; "Weak typing"; "Dynamic typing"; "No typing"]; CorrectAnswer = "Static typing"; QuestionType = "MultipleChoice"; Score = 1 }
        8, { Text = "What is the default data type for decimal numbers in F#?"; Options = None; CorrectAnswer = "float"; QuestionType = "Written"; Score = 2 }
        9, { Text = "Does F# use curly braces {} or indentation to define scope?"; Options = None; CorrectAnswer = "indentation"; QuestionType = "Written"; Score = 2 }
        10, { Text = "How To Define Mutable Variable in F# ?"; Options = Some ["var";"let";"let mutable";"rec"]; CorrectAnswer = "let mutable"; QuestionType = "MultipleChoice"; Score = 2 }

    ]

let createQuizForm (userInfo: UserInfo) (questions: Map<int, Question>) =
    let form = new Form(Text = sprintf "Quiz - %s-%s DR.%s { 20 Marks }" userInfo.Name userInfo.ID userInfo.ProfName, Size = Size(800, 450), StartPosition = FormStartPosition.CenterScreen , BackColor = Color.Aqua)
    let userId = userInfo.ID
    let Name = userInfo.Name
    let profName = userInfo.ProfName

    
    let mutable userAnswers = Map.empty<int, string>
    let mutable currentIndex = 0
    let mutable score = 0

    
    let mutable answeredQuestions = Map.empty<int, bool>

   
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
    let timer = new Timer(Interval = 1000) 
    let Qnum = new Label(Text = $"{currentIndex+1}\11",Location = Point(180, 20), AutoSize = true, Font = new Font("Arial", 15.0f, FontStyle.Bold), ForeColor = Color.DarkGoldenrod)
    let mutable remainingTime = 3 * 60 

    timer.Tick.Add(fun _ ->
        if remainingTime > 0 then
            remainingTime <- remainingTime - 1
            let minutes = remainingTime / 60
            let seconds = remainingTime % 60
            timeLabel.Text <- sprintf "%02d:%02d" minutes seconds 
        else
            timer.Stop() 
            finishButton.PerformClick() 
        
    )

    timer.Start() 

    
    (*let updateScoreDisplay () =
        scoreTrackerLabel.Text <- sprintf "Score: %d" score*)

   
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

        
        if not (answeredQuestions |> Map.tryFind currentIndex |> Option.defaultValue false) then
            match question.QuestionType with
            | "MultipleChoice" ->
                let selectedOption =
                    optionsPanel.Controls
                    |> Seq.cast<Control>
                    |> Seq.tryFind (fun c -> (c :?> RadioButton).Checked)
                    |> Option.map (fun c -> c.Text)

                
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
            
            answeredQuestions <- answeredQuestions.Add(currentIndex, true)

        answerTextBox.Clear()
        currentIndex <- currentIndex + 1
        Qnum.Text <- $"{currentIndex+1}\11"

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

        
        currentIndex <- currentIndex - 1
        Qnum.Text <- $"{currentIndex+1}\11"

        
        if questions.ContainsKey(currentIndex) then
            let question = questions.[currentIndex]

            
            if userAnswers.ContainsKey(currentIndex) then
                let userAnswer = userAnswers.[currentIndex]

                
                match question.QuestionType with
                | "MultipleChoice" ->
                    if userAnswer = question.CorrectAnswer then
                        score <- score - question.Score

                | "Written" ->
                    if userAnswer.ToLower() = question.CorrectAnswer.ToLower() then
                        score <- score - question.Score

                | _ -> ()

                
                if score < 0 then
                    score <- 0

                
                //updateScoreDisplay ()

                
                answeredQuestions <- answeredQuestions.Remove(currentIndex)

            
            displayQuestion currentIndex)

    let appendToFile filePath (text: string) =
        try
            
            let directory = Path.GetDirectoryName(filePath)
            if not (Directory.Exists(directory)) then
                Directory.CreateDirectory(directory) |> ignore

            
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
            let message = sprintf "Your final score is %d \n Quiz Finished \n Name: %s \n ID: %s \nDR: %s\n\n%s" score Name userId profName answersMessage
            let res = $"Name: {Name}\tID: {userId}\tDR: {profName}\tScore: {score}"
            
            appendToFile @"C:\Results\res.txt" res
            MessageBox.Show(message) |> ignore
            Application.Exit()
            
        form.Close())

    displayQuestion currentIndex
    form.Controls.AddRange([| Qnum;(*scoreTrackerLabel;*) timeLabel;progressBar;questionLabel; optionsPanel; answerTextBox; nextButton; backButton; finishButton; scoreLabel |])
    form.ShowDialog() |> ignore

let createMainForm () =
    let form = new Form(Text = "F# Quiz", Size = Size(500, 450), StartPosition = FormStartPosition.CenterScreen , BackColor = Color.Aqua)
    let nameLabel = new Label(Text = "Name:", Location = Point(20, 100), AutoSize = true, Font = new Font("Arial", 12.0f))
    let nameTextBox = new TextBox(Location = Point(120, 100), Size = Size(300, 30))
    let idLabel = new Label(Text = "ID:", Location = Point(20, 150), AutoSize = true, Font = new Font("Arial", 12.0f))
    let idTextBox = new TextBox(Location = Point(120, 150), Size = Size(300, 30))
    let profLabel = new Label(Text = "Prof Name:", Location = Point(20, 200), AutoSize = true, Font = new Font("Arial", 12.0f))
    let profTextBox = new TextBox(Location = Point(120, 200), Size = Size(300, 30))
    let startButton = new Button(Text = "Start", Location = Point(200, 250), Font = new Font("Arial", 10.0f, FontStyle.Bold), BackColor = Color.GreenYellow)
    let mainMessageLabel = new Label(Text="F# Quiz",Location = Point(180, 15), AutoSize = true, Font = new Font("Arial", 15.0f,FontStyle.Underline),ForeColor = Color.BlueViolet)
    startButton.Click.Add(fun _ ->
        if not (String.IsNullOrWhiteSpace(nameTextBox.Text) || String.IsNullOrWhiteSpace(idTextBox.Text) || String.IsNullOrWhiteSpace(profTextBox.Text)) then
            let userInfo = { Name = nameTextBox.Text; ID = idTextBox.Text ; ProfName = profTextBox.Text}
            createQuizForm userInfo questions |> ignore
        else
            MessageBox.Show("Please enter your Name,  your ID and your Prof Name.", "Error") |> ignore)

    form.Controls.AddRange([| profLabel;profTextBox;mainMessageLabel;nameLabel; nameTextBox; idLabel; idTextBox; startButton |])
    form

[<EntryPoint>]
let main _ =
    Application.Run(createMainForm())
    0
