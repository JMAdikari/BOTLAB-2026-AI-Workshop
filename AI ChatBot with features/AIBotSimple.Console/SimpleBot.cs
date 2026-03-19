using OpenAI.Chat;

namespace AIBotSimple.Console
{
    public class SimpleBot
    {
        private readonly ChatClient _chatClient;
        private readonly List<ChatMessage> _memory;
        private readonly string _systemPrompt;
        private readonly List<string> _sensitiveWords;
        
        internal SimpleBot(
            ChatClient chatClient,
            string systemPrompt,
            List<ChatMessage>? memory = null,
            List<string>? sensitiveWords = null)
        {
            _chatClient = chatClient;
            _systemPrompt = systemPrompt;
            _memory = memory ?? new List<ChatMessage>();
            _sensitiveWords = sensitiveWords ?? new List<string>();

            if (!string.IsNullOrWhiteSpace(systemPrompt))
            {
                _memory.Add(new SystemChatMessage(systemPrompt));
            }
        }

        //with sensitive words
        public async Task RunAsync()
        {
            System.Console.WriteLine("🤖 ChatGPT Console Bot");
            System.Console.WriteLine("Type 'exit' to quit.\n");

            while (true)
            {
                System.Console.Write("You: ");
                var input = System.Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                // Check user input for sensitive words
                if (ContainsSensitiveWord(input))
                {
                    System.Console.WriteLine("⚠️ Your input contains a sensitive word and will not be processed.");
                    continue;
                }

                _memory.Add(new UserChatMessage(input));

                var result = await _chatClient.CompleteChatAsync(_memory);

                var reply = result.Value.Content[0].Text;

                // Filter bot reply for sensitive words
                reply = FilterSensitiveWords(reply);

                _memory.Add(new AssistantChatMessage(reply));

                System.Console.WriteLine("Bot: " + reply);
            }
        }
        private string FilterSensitiveWords(string text)
        {
            foreach (var word in _sensitiveWords)
            {
                if (!string.IsNullOrWhiteSpace(word))
                    text = text.Replace(word, "[redacted]", StringComparison.OrdinalIgnoreCase);
            }
            return text;
        }

        private bool ContainsSensitiveWord(string text)
        {
            return _sensitiveWords.Any(word => !string.IsNullOrWhiteSpace(word) &&
                                               text.Contains(word, StringComparison.OrdinalIgnoreCase));
        }
        
        //basic bot
        /*public async Task RunAsync()
        {
            System.Console.WriteLine("🤖 ChatGPT Console Bot");
            System.Console.WriteLine("Type 'exit' to quit.\n");

            while (true)
            {
                System.Console.Write("You: ");
                var input = System.Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                _memory.Add(new UserChatMessage(input));

                var result = await _chatClient.CompleteChatAsync(_memory);

                var reply = result.Value.Content[0].Text;

                _memory.Add(new AssistantChatMessage(reply));

                System.Console.WriteLine("Bot: " + reply);
            }
        }*/
    }
}