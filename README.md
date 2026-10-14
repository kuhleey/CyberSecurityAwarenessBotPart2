# Cybersecurity Awareness Bot — Part 1 + Part 2

A Windows Forms C# application combining Part 1's original voice greeting, ASCII-art logo, user greeting, input validation, chatbot responses, and Part 2's GUI, keyword recognition, varied responses, follow-up context, memory, sentiment detection and error handling.

## Setup
1. Use **Windows** and **Visual Studio 2022 (17.8+)** with **.NET desktop development** and the **.NET 8 SDK**.
2. Extract the ZIP, then open `CyberSecurityAwarenessBot.sln`.
3. Build > Rebuild Solution, then press F5.
4. The GUI is coded in `MainForm.cs`; a separate Designer file is not required.

## Features and where to find them
- `MainForm.cs`: GUI, chat controls, name input, ASCII logo display, voice button and validation messages.
- `AsciiArt.cs`: original Part 1 ASCII logo adapted for a label.
- `Greeting.cs`: Part 1 welcome message, invoked by **Greet me**.
- `Validation.cs`: Part 1 input checks.
- `ChatBot.cs` + `Responses.cs`: Part 1 general responses, used for general questions.
- `VoiceGreeting.cs` + `Audio/VoiceGreeting.wav`: Part 1 recorded greeting with portable audio path.
- `ChatBotEngine.cs`: Part 2 keyword dictionary, randomized answers, conversation topic tracking, name/interest memory, sentiment and fallback responses.
- `Assets/Part1Screenshot.png`: original Part 1 screenshot asset included for reference.

## Test checklist
1. Click **Play greeting** (check laptop volume).
2. Type a name and click **Greet me**; ask `what is my name?`.
3. Ask `what is your purpose?` (Part 1 response).
4. Ask `Tell me about password safety`, `How can I avoid scams?`, and `How do I protect my privacy?`.
5. Ask `Give me a phishing tip`, then `another tip`, then `tell me more`.
6. Say `I'm interested in privacy`, then `what am I interested in?`.
7. Say `I'm worried about scams` — expect supportive wording **and** a scam tip.
8. Press Send with empty input and type an unknown word — expect helpful error responses.

## Submission reminders
Your brief asks for the **GitHub repository URL** on ARC (not a ZIP), plus an **unlisted YouTube presentation**. It also requests a README, the WAV and ASCII art image, at least six meaningful commits, and a tag with at least two releases. Create genuine commits and releases as you develop and test the project; do not fabricate history.

## Verification limitation
Source files, ZIP integrity, and structural references were checked in the preparation environment. **Windows GUI compilation and runtime playback were not tested here** because the .NET SDK/Windows Forms runtime is unavailable. Please build and test in Visual Studio before submission.
