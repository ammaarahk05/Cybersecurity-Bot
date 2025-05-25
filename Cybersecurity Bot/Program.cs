using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Threading;

namespace CyberSecurity_Bot
{
    internal class Program
    {
        static List<string> chatHistory = new List<string>();
        static string currentTopic = null;
        static string favoriteTopic = null;
        static SpeechSynthesizer synth = new SpeechSynthesizer

        {
            Volume = 100,
            Rate = 0
        };

        static Random random = new Random();

        static void Main()
        {
            PlayGreetingAudio("Cyber Chatbot Audio 2.wav");

            Console.Title = "Cybersecurity Awareness Chatbot";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(new string('=', Console.WindowWidth));

            string[] asciiArtLines = new string[]
{
    "  ░▒▓██████▓▒░   ░▒▓███████▓▒░        ░▒▓███████▓▒░   ░▒▓██████▓▒░▒▓████████▓▒░  ",
    "  ░▒▓█▓▒░░▒▓█▓▒░ ▒▓█▓▒░                ░▒▓█▓▒░░▒▓█▓▒░ ▒▓█▓▒░░▒▓█▓▒░ ░▒▓█▓▒░      ",
    "  ░▒▓█▓▒░    ░▒▓█▓▒░                    ░▒▓█▓▒░░▒▓█▓▒░ ▒▓█▓▒░░▒▓█▓▒░ ░▒▓█▓▒░      ",
    "  ░▒▓█▓▒░     ░▒▓██████▓▒░              ░▒▓███████▓▒░  ░▒▓█▓▒░░▒▓█▓▒░ ░▒▓█▓▒░      ",
    "  ░▒▓█▓▒░           ░▒▓█▓▒░             ░▒▓█▓▒░░▒▓█▓▒░ ▒▓█▓▒░░▒▓█▓▒░ ░▒▓█▓▒░      ",
    "  ░▒▓█▓▒░░▒▓█▓▒░    ░▒▓█▓▒░▒▓██▓▒░       ░▒▓█▓▒░░▒▓█▓▒░ ▒▓█▓▒░░▒▓█▓▒░ ░▒▓█▓▒░      ",
    "   ░▒▓██████▓▒░░▒▓███████▓▒░▒▓██▓▒░       ▒▓███████▓▒░   ░▒▓██████▓▒░  ░▒▓█▓▒░    ",
    "                                     "
};

            int consoleWidth = Console.WindowWidth;
            foreach (string line in asciiArtLines)
            {
                int padding = (consoleWidth - line.Length) / 2;
                Console.WriteLine(new string(' ', Math.Max(0, padding)) + line);
            }

            Console.WriteLine(new string('=', Console.WindowWidth));

            TypingEffect("Hello! Welcome to your Cybersecurity Awareness Assistant Bot!\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("What is your name? ");
            Console.ForegroundColor = ConsoleColor.White;
            string userName = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("What's your favorite topic in cybersecurity (e.g., phishing, passwords, malware)? ");
            Console.ForegroundColor = ConsoleColor.White;
            favoriteTopic = Console.ReadLine()?.ToLower().Trim();

            if (!string.IsNullOrEmpty(favoriteTopic))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                RespondWithSpeech($"Got it! I'll remember that your favorite topic is {favoriteTopic}.");
            }

            LoadingEffect();
            Console.ForegroundColor = ConsoleColor.Magenta;
            RespondWithSpeech($"Hi, {userName}! I'm here to help you stay safe online.\n");

            DisplayTipOfTheDay();

            Console.WriteLine(new string('—', 50));
            Console.WriteLine(" You can ask about: ");
            foreach (var topic in topicKeywords.Keys)
            {
                Console.WriteLine($" - {topic}");
            }
            Console.WriteLine(" - Or type 'exit' to quit.");
            Console.WriteLine(new string('-', 50));

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{userName}: ");
                string userInput = Console.ReadLine()?.ToLower().Trim();

                if (string.IsNullOrEmpty(userInput))
                {
                    LoadingEffect();
                    Console.ForegroundColor = ConsoleColor.Red;
                    RespondWithSpeech("Please enter a valid question.");
                    continue;
                }

                if (userInput == "exit")
                {
                    LoadingEffect();
                    Console.ForegroundColor = ConsoleColor.Green;
                    RespondWithSpeech("Stay safe and think before you click! Goodbye!");
                    break;
                }

                HandleUserQuery(userInput, userName);
            }

            SaveChatHistory();
        }

        static void PlayGreetingAudio(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                if (File.Exists(fullPath))
                {
                    SoundPlayer player = new SoundPlayer(fullPath);
                    player.PlaySync();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: The file '{filePath}' was not found.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error playing audio: {ex.Message}");
            }
        }

        static Dictionary<string, List<string>> topicKeywords = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
{
    { "safe browsing", new List<string> { "safe browsing", "secure browsing", "https", "vpn", "public wifi", "browser security" } },
    { "phishing", new List<string> { "phishing", "email scam", "fake email", "suspicious link", "phish", "email fraud" } },
    { "strong passwords", new List<string> { "strong password", "secure password", "weak password", "password tips", "good password" } },
    { "two-factor authentication", new List<string> { "two-factor", "2fa", "authentication", "verification code", "security code" } },
    { "malware protection", new List<string> { "malware", "virus", "antivirus", "malicious software", "trojan", "ransomware" } },
    { "social media", new List<string> { "social media", "facebook", "instagram", "privacy settings", "oversharing" } },
    { "cookies", new List<string> { "cookies", "tracking", "browser cookies", "cookie settings", "web tracking" } }
};

        static Dictionary<string, List<string>> topicResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
{
     {
        "safe browsing", new List<string>
        {
            "Use HTTPS websites for secure communication.",
            "Avoid entering personal info on unsecured public Wi-Fi.",
            "Consider using a VPN when accessing sensitive information on public networks.",
            "Browser extensions can add an extra layer of security—just be sure to install from trusted sources.",
            "Regularly clear cookies and cached data to reduce tracking risks."
        }
    },
    {
        "phishing", new List<string>
        {
            "Be cautious of emails urging urgent action, especially those with attachments or links.",
            "Legitimate companies never ask for personal info through email.",
            "Check URLs by hovering over links before clicking them.",
            "If in doubt, contact the organization directly using official contact details.",
            "Spelling errors and generic greetings are common red flags in phishing emails."
        }
         },
    {
        "strong passwords", new List<string>
        {
            "Use a mix of uppercase, lowercase, numbers, and symbols in your password.",
            "Avoid using easily guessed passwords like '123456' or your birthdate.",
            "Use a password manager to store and generate secure passwords.",
            "Never reuse passwords across multiple accounts.",
            "Longer passwords—at least 12 characters—are much harder to crack."
        }
    },
    {
        "two-factor authentication", new List<string>
        {
            "2FA adds an extra layer of security by requiring a second verification step.",
            "Even if someone gets your password, they can't access your account without the second factor.",
            "Authenticator apps are more secure than SMS-based codes.",
            "Enable 2FA on important accounts like email, banking, and social media.",
            "Backup codes are essential in case you lose access to your 2FA device."
        }
         },
    {
        "malware protection", new List<string>
        {
            "Keep your antivirus software updated regularly.",
            "Avoid downloading software from unknown or unverified sources.",
            "Don't click pop-up ads that claim your computer is infected.",
            "Schedule regular scans of your system for malware.",
            "Watch out for slow performance or unexpected behavior—it might be a malware sign."
        }
    },
    {
        "social media", new List<string>
        {
            "Avoid posting personal info like your location, school, or workplace publicly.",
            "Review your privacy settings to control who sees your content.",
            "Be careful who you accept friend requests from—fake accounts are common.",
            "Don't share photos that reveal sensitive details like your address.",
            "Use strong passwords and 2FA for your social accounts too."
        }
    },
    {
        "cookies", new List<string>
        {
            "Cookies store data like login info and site preferences.",
            "Some cookies track your activity across websites for advertising purposes.",
            "Clear cookies regularly or use private browsing modes.",
            "Adjust your browser settings to block third-party cookies.",
            "Cookie banners on websites let you choose what data you're okay sharing."
        }
    }

};
        static List<(string[] phrases, string response)> contextualSentiments = new List<(string[], string)>
{
    (new[] { "i’m really worried", "i’m scared", "i feel anxious", "i'm nervous" },
     "It’s totally okay to feel this way. Cybersecurity can be intimidating, but don't worry! you're taking the right steps."),

    (new[] { "i'm confused", "i don’t get this", "this doesn’t make sense" },
     "Hey, no worries—this stuff can be tricky. Let’s break it down together."),

    (new[] { "this is frustrating", "i’m tired of", "i hate dealing with passwords" },
     "I understand—cybersecurity can feel quite frustrating sometimes. Let me help make it simpler."),



    (new[] { "i’m curious", "i want to know", "i’m interested in", "tell me more about" },
     "That’s a great question! Curiosity is key in learning. Let’s dive right into it, shall we?"),

    (new[] { "i feel overwhelmed", "this is too much", "i don’t know where to start" },
     "You're not alone—many people feel this way. Let's take it step by step."),
};

        static Dictionary<string, List<string>> givenResponses = new Dictionary<string, List<string>>();

        static void HandleUserQuery(string input, string userName)
        {
            try
            {
                chatHistory.Add($"{userName}: {input}");
                // Check if input matches general small talk questions
                string[] greetings = { "how are you", "how are you doing", "what's up", "what is up" };
                string[] purposeQuestions = { "what's your purpose", "what do you do", "what are you", "who are you" };
                string[] creatorQuestions = { "who made you", "who created you", "who built you" };
                string[] thanks = { "thank you", "thanks", "thx", "ty" };

                if (greetings.Any(g => input.Contains(g)))
                {
                    RespondWithSpeech("I'm doing great, thank you for asking! I'm here to help you stay safe online.");
                    return;
                }

                if (purposeQuestions.Any(p => input.Contains(p)))
                {
                    RespondWithSpeech("I'm your cybersecurity assistant, here to help you understand online threats and stay protected.");
                    return;
                }

                if (creatorQuestions.Any(c => input.Contains(c)))
                {
                    RespondWithSpeech("I was created by a student developer as part of a cybersecurity project.");
                    return;
                }

                if (thanks.Any(t => input.Contains(t)))
                {
                    RespondWithSpeech("You're very welcome! Let me know if you have more questions.");
                    return;
                }

                // Check for emotional or sentiment-based responses
                if (TryDetectContextualSentiment(input, out string sentimentResponse))
                {
                    LoadingEffect();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    RespondWithSpeech(sentimentResponse);
                    return;
                }

                // Check if the user asks about their favorite topic
                if (input.Contains("favourite") || input.Contains("favorite"))
                {
                    if (!string.IsNullOrEmpty(favoriteTopic))
                    {
                        RespondWithSpeech($"You mentioned earlier that your favorite topic is {favoriteTopic}. Would you like to explore it now?");
                    }
                    else
                    {
                        RespondWithSpeech("You haven’t told me your favorite topic yet!");
                    }
                    return;
                }

                // Handle follow-up based on current topic
                if (IsFollowUp(input) && !string.IsNullOrEmpty(currentTopic))
                {
                    RespondWithFollowUp(currentTopic);
                    return;
                }

                // Match a new topic
                if (TryMatchTopic(input, out string matchedTopic))
                {
                    if (currentTopic != matchedTopic)
                    {
                        RespondWithSpeech($"Great! Let's talk about {matchedTopic}.");
                        currentTopic = matchedTopic;
                        if (!givenResponses.ContainsKey(currentTopic))
                            givenResponses[currentTopic] = new List<string>();
                    }
                    RespondWithTopic(currentTopic);
                    return;
                }

                // General fallback response
                if (TryGeneralResponse(input, out string generalResponse))
                {
                    RespondWithSpeech(generalResponse);
                    return;
                }

                // If no match is found
                RespondWithSpeech("Hmm, I’m not sure about that. Try asking about a topic like phishing or safe browsing.");
                DisplayAvailableTopics();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Helpers:

        static bool IsFollowUp(string input)
        {
            string[] followUps = { "more", "tell me more", "explain", "details" };
            return followUps.Any(f => input.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0);
        }


        static void RespondWithFollowUp(string topic)
        {
            if (topicResponses.TryGetValue(topic, out List<string> followUps))
            {
                var newFollowUps = followUps.Where(f => !givenResponses[topic].Contains(f)).ToList();
                if (newFollowUps.Count == 0)
                {
                    givenResponses[topic].Clear();
                    newFollowUps = followUps;
                }
                string response = newFollowUps[random.Next(newFollowUps.Count)];
                givenResponses[topic].Add(response);
                RespondWithSpeech($"Sure! {response}");
            }
        }

        static bool HandleGeneralQuestions(string userInput)
        {
            if (userInput.Contains("how are you"))
            {
                RespondWithSpeech("I'm just a cybersecurity bot, but I'm here and ready to help you stay cyber safe!");
                return true;
            }
            else if (userInput.Contains("what is your purpose") || userInput.Contains("why were you created"))
            {
                RespondWithSpeech("My purpose is to help you understand and practice cybersecurity in a simple and friendly way.");
                return true;
            }
            else if (userInput.Contains("what do you do") || userInput.Contains("what can you do"))
            {
                RespondWithSpeech("I can answer questions about cybersecurity topics like phishing, malware, strong passwords, and more.");
                return true;
            }
            else if (userInput.Contains("who created you") || userInput.Contains("who made you"))
            {
                RespondWithSpeech("I was created by a BCAD student to assist with cybersecurity awareness and education.");
                return true;
            }
            return false;
        }

        static bool TryMatchTopic(string input, out string matchedTopic)
        {
            input = input.ToLower();

            foreach (var pair in topicKeywords)
            {
                foreach (string keyword in pair.Value)
                {
                    if (input.Contains(keyword.ToLower()))
                    {
                        matchedTopic = pair.Key;
                        return true;
                    }

                    // Check for similar keywords (e.g., "passwords" instead of "password")
                    string[] similarKeywords = keyword.Split(' ');
                    foreach (string similarKeyword in similarKeywords)
                    {
                        if (input.Contains(similarKeyword.ToLower()))
                        {
                            matchedTopic = pair.Key;
                            return true;
                        }
                    }
                }
            }

            matchedTopic = null;
            return false;
        }
        static void DisplayAvailableTopics()
        {
            Console.WriteLine(new string('—', 50));
            Console.WriteLine(" You can ask about: ");
            foreach (var topic in topicKeywords.Keys)
            {
                Console.WriteLine($" - {topic}");
            }
            Console.WriteLine(" - Or type 'exit' to quit.");
            Console.WriteLine(new string('-', 50));
        }

        static void RespondWithTopic(string topic)
        {
            if (topicResponses.TryGetValue(topic, out List<string> responses))
            {
                var newResponses = responses.Where(f => !givenResponses[topic].Contains(f)).ToList();
                if (newResponses.Count == 0)
                {
                    givenResponses[topic].Clear();
                    newResponses = responses;
                }
                string response = newResponses[random.Next(newResponses.Count)];
                givenResponses[topic].Add(response);
                RespondWithSpeech(response);
            }
        }


        static bool TryGeneralResponse(string input, out string response)
        {
            foreach (var pair in topicResponses)
            {
                foreach (string phrase in pair.Value)
                {
                    if (input.IndexOf(phrase, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        response = pair.Value[random.Next(pair.Value.Count)];
                        return true;
                    }
                }
            }
            response = null;
            return false;
        }


        static bool TryDetectContextualSentiment(string input, out string response)
        {
            input = input.Replace("’", "'").ToLower();

            foreach (var (phrases, sentimentResponse) in contextualSentiments)
            {
                foreach (var phrase in phrases)
                {
                    if (input.Contains(phrase))
                    {
                        response = sentimentResponse;
                        return true;
                    }
                }
            }

            response = null;
            return false;
        }
        static void RespondWithDefault()
        {
            RespondWithSpeech("I'm not sure I understand. Can you try rephrasing?");
        }

        static void DisplayTipOfTheDay()
        {
            string[] tips = new string[]
            {
                "Tip: Use multi-factor authentication (MFA) wherever possible to enhance security!",
                "Tip: Always verify the sender's email address before clicking on any link!",
                "Tip: Avoid using public Wi-Fi for accessing sensitive information like banking!"
            };

            int tipIndex = random.Next(tips.Length);
            LoadingEffect();
            Console.ForegroundColor = ConsoleColor.Green;
            RespondWithSpeech($"Security Tip of the Day: {tips[tipIndex]}");
        }

        static void TypingEffect(string message, int delay = 30)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        static void LoadingEffect()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ChatBot");
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(400);
                Console.Write(".");
            }
            Console.WriteLine();
        }

        static void SaveChatHistory()
        {
            string path = "chat_history.txt";
            File.WriteAllLines(path, chatHistory);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Chat history saved to {path}");
        }

        static void RespondWithSpeech(string response)
        {
            LoadingEffect(); // loads animation before speaking
            Console.ForegroundColor = ConsoleColor.Blue;
            TypingEffect($"ChatBot: {response}\n"); // Simulates typing 

            try
            {
                synth.Speak(response); // Text-to-speech
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[TTS Error]: {ex.Message}");
            }

            chatHistory.Add($"ChatBot: {response}"); // Logs a response
            Console.ResetColor(); // Code to reset color after response
        }
    }
}

