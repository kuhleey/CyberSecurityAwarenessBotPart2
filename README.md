# Cybersecurity Awareness Bot Part2

A Windows Forms C# application combining Part 1's original voice greeting, ASCII-art logo, user greeting, input validation, chatbot responses, and Part 2's GUI, keyword recognition, varied responses, follow-up context, memory, sentiment detection and error handling.


## Features and where to find them
- `MainForm.cs`: GUI, chat controls, name input, ASCII logo display, voice button and validation messages.
- `AsciiArt.cs`: original Part 1 ASCII logo adapted for a label.
- `Greeting.cs`: Part 1 welcome message, invoked by **Greet me**.
- `Validation.cs`: Part 1 input checks.
- `ChatBot.cs` + `Responses.cs`: Part 1 general responses, used for general questions.
- `VoiceGreeting.cs` + `Audio/VoiceGreeting.wav`: Part 1 recorded greeting with portable audio path.
- `ChatBotEngine.cs`: Part 2 keyword dictionary, randomized answers, conversation topic tracking, name/interest memory, sentiment and fallback responses.
- `Assets/Part1Screenshot.png`: original Part 1 screenshot asset included for reference.

