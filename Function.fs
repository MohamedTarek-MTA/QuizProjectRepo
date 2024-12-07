finishButton.Click.Add(fun _ ->
    let finalMessage = 
        let answersMessage =
            questions
            |> Map.fold (fun acc index question ->
                let userAnswer = userAnswers |> Map.tryFind index |> Option.defaultValue "No Answer"
                let correctAnswer = question.CorrectAnswer
                let userAnswerMessage = sprintf "Q%d: %s\nYour Answer: %s\nCorrect Answer: %s\n\n" (index + 1) question.Text userAnswer correctAnswer
                acc + userAnswerMessage) ""

        let message = sprintf "Your final score is %d \n Quiz Finished \n Name: %s \n ID: %s \n\n%s" score Name userId answersMessage
        let res = $"Name: {Name}\tID: {userId}\tScore: {score}"
        // Call the function to append the message to the file
        appendToFile @"C:\Results\res.txt" res
        MessageBox.Show(message) |> ignore
    form.Close())

displayQuestion currentIndex
form.Controls.AddRange([| scoreTrackerLabel; questionLabel; optionsPanel; answerTextBox; nextButton; backButton; finishButton; scoreLabel |])
form.ShowDialog() |> ignore