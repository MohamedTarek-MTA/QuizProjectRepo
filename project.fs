let createMainForm () =
    let form = new Form(Text = "Quiz Application", Size = Size(500, 450), StartPosition = FormStartPosition.CenterScreen)
    let welcome = new Label(Text = "Welcome" , Location = Point(200,10),AutoSize = true,Font = new Font("Arial", 20.0f))
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

    form.Controls.AddRange([| welcome;nameLabel; nameTextBox; idLabel; idTextBox; startButton |])
    form

[<EntryPoint>]
let main _ =
    Application.Run(createMainForm())
    04