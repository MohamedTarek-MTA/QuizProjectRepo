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

            match question.QuestionType with
            | "MultipleChoice" ->
                let selectedOption =
                    optionsPanel.Controls
                    |> Seq.cast<Control>
                    |> Seq.tryFind (fun c -> (c :?> RadioButton).Checked)
                    |> Option.map (fun c -> c.Text)

                // Track the user's answer
                userAnswers <- userAnswers.Add(currentIndex, selectedOption |> Option.defaultValue "")

                if selectedOption = Some question.CorrectAnswer then
                    score <- score + question.Score
                    updateScoreDisplay ()

            | "Written" ->
                userAnswers <- userAnswers.Add(currentIndex, answerTextBox.Text.Trim())

                if answerTextBox.Text.Trim() = question.CorrectAnswer then
                    score <- score + question.Score
                    updateScoreDisplay ()

            | _ -> ()

            answerTextBox.Clear()
            currentIndex <- currentIndex + 1

            if questions.ContainsKey(currentIndex) then
                displayQuestion currentIndex
            else
                scoreLabel.Text <- sprintf "Your score: %d" score
                nextButton.Enabled <- false)

    backButton.Click.Add(fun _ ->
        if currentIndex > 1 then
            currentIndex <- currentIndex - 1
            displayQuestion currentIndex)
