# AI ChatBots

A C# .NET 8.0 solution demonstrating three different AI chatbot architectures using the OpenAI API. Built for the BOTLAB 2026 AI Workshop.

---

## What is this project?

This solution contains three independent console chatbot projects, each showing a different approach to building AI assistants:

| Project | What it does |
|---|---|
| **AIBotSimple.Console** | A chatbot with a custom personality and sensitive word filtering |
| **Agent** | An AI agent that can read/write files and run shell commands |
| **ModeratedBot** | A two-stage chatbot where a second AI moderates the first AI's responses |

---

## Project Breakdown

### 1. AIBotSimple.Console — Simple Bot with Personality
A conversational chatbot that you can give a persona (name, greeting, style). It filters sensitive words from both user input and bot responses, replacing them with `[redacted]`. Conversation memory can be toggled on or off.

### 2. Agent — Autonomous AI Agent
An AI agent that can use **tools** to interact with your computer. It calls the OpenAI API and automatically decides when to use tools like listing directories, reading/writing files, or running shell commands. It keeps looping until the task is fully complete.

**Available tools:**
- `list_directory` — list files and folders
- `read_file` — read a file's contents
- `write_file` — write content to a file
- `file_exists` — check if a path exists
- `execute_command` — run a shell command (e.g. `ls`, `pwd`)

### 3. ModeratedBot — Two-Stage Content Moderation
A chatbot with a built-in moderation pipeline:
1. **Stage 1** — The primary model generates a response to the user
2. **Stage 2** — A moderator model checks the response for sensitive categories (e.g. political, religious content)

If the response is flagged, it is either **blocked** or **rewritten** by the moderator depending on the `ForceCorrection` setting.

---

## Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An [OpenAI API key](https://platform.openai.com/api-keys)

---

## Setup

### 1. Clone or download the project

```bash
git clone <repository-url>
cd "AI ChatBot with features"
```

### 2. Add your API key

A `.env` file is already created at the root of the solution. Open it and replace the placeholder with your actual key:

```
OPENAI_API_KEY=your-openai-api-key-here
```

> The `.env` file is listed in `.gitignore` and will not be committed to git.

### 3. Restore dependencies

```bash
dotnet restore
```

---

## How to Run

Each project is a separate console application. Run whichever one you want to try:

### Run the Simple Bot
```bash
cd AIBotSimple.Console
dotnet run
```

### Run the Agent
```bash
cd Agent
dotnet run
```

### Run the Moderated Bot
```bash
cd ModeratedBot
dotnet run
```

Or run from the solution root by specifying the project:

```bash
dotnet run --project AIBotSimple.Console
dotnet run --project Agent
dotnet run --project ModeratedBot
```

---

## Project Structure

```
AIBot.sln
.env                          ← your API key (never commit this)
.gitignore

Agent/
├── Program.cs
├── AgentBuilder.cs
├── SimpleAgent.cs
├── AgentTools.cs
└── Capabilities/
    ├── FileSystemAccess.cs
    ├── CommandExecutor.cs
    └── ToolExecutor.cs

AIBotSimple.Console/
├── Program.cs
├── SimpleBotBuilder.cs
├── SimpleBot.cs
└── Feature/
    └── Personality.cs

ModeratedBot/
├── Program.cs
├── ModeratedBotBuilder.cs
└── ModeratedBot.cs
```

---

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| `OpenAI` | 2.9.1 | OpenAI Chat API client |
| `DotNetEnv` | 3.1.1 | Load API key from `.env` file |
