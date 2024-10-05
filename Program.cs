using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MadLibsGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Handle CTRL+C gracefully
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);

            // Display the title
            DisplayTitle("Mad Libs Game");

            // Load stories from JSON
            var stories = LoadStories("assets/stories.json");

            // Display available themes
            var themes = DisplayThemes(stories);

            // Prompt user to select a theme by number
            Console.Write("Select a theme (enter the number): ");
            if (!int.TryParse(Console.ReadLine(), out int themeIndex) || themeIndex < 1 || themeIndex > themes.Count)
            {
                Console.WriteLine("Invalid selection. Please select a valid theme number.");
                return; // Exit the program or re-prompt for a valid theme
            }

            // Add a blank line after theme selection
            Console.WriteLine();

            // Select the theme based on the user's input
            string selectedTheme = themes[themeIndex - 1]; // Adjust for zero-based index

            // Select a random story from the chosen theme
            var story = SelectRandomStory(stories, selectedTheme);

            // Collect user inputs for placeholders
            var userInputs = CollectUserInputs(story);

            // Add a blank line before displaying the title of the story
            Console.WriteLine();

            // Generate and display the final story
            string finalStory = GenerateStory(story, userInputs);

            // Display the title of the story
            DisplayTitle(story["title"].ToString());
            Console.WriteLine(); // Blank line after the title
            Console.WriteLine(finalStory); // Print the formatted story
            Console.WriteLine(); // Blank line after the story

            // Exit message
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Hope you had a laugh and enjoyed the mad libs!");
            Console.ResetColor();
        }

        static void OnExit(object sender, ConsoleCancelEventArgs e)
        {
            // Set the Cancel property to true to prevent the process from terminating.
            e.Cancel = true;

            // Display a message and perform any cleanup if necessary
            Console.WriteLine("\nGame interrupted. Exiting gracefully...");
            // You can add any additional cleanup code here if needed

            // Exit the application
            Environment.Exit(0);
        }

        static void DisplayTitle(string title)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            int totalWidth = 60; // Total width of the title box
            int titleLength = title.Length;
            int padding = (totalWidth - titleLength) / 2; // Calculate padding for centering

            Console.WriteLine("╔" + new string('═', totalWidth - 2) + "╗");
            Console.WriteLine("║" + new string(' ', padding) + title + new string(' ', totalWidth - titleLength - padding - 2) + "║");
            Console.WriteLine("╚" + new string('═', totalWidth - 2) + "╝");
            Console.ResetColor();
        }

        static dynamic LoadStories(string path)
        {
            // Load and parse the JSON file
            string json = File.ReadAllText(path);
            var stories = JsonConvert.DeserializeObject<dynamic>(json);

            // Check if stories or themes are null
            if (stories == null || stories["themes"] == null)
            {
                throw new Exception("Failed to load stories from JSON.");
            }

            return stories;
        }

        static List<string> DisplayThemes(dynamic stories)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Available Themes:");
            var themes = new List<string>();
            int index = 1; // To keep track of the theme number

            foreach (var theme in stories["themes"].Properties())
            {
                // Print the theme number in red
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{index}. ");

                // Print the theme name in green
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(theme.Name);

                themes.Add(theme.Name);
                index++; // Increment the index for the next theme
            }

            Console.ResetColor(); // Reset color to default
            return themes;
        }

        static dynamic SelectRandomStory(dynamic stories, string selectedTheme)
        {
            // Check if the selected theme exists in the stories
            if (stories["themes"] == null || stories["themes"][selectedTheme] == null)
            {
                throw new Exception($"Theme '{selectedTheme}' does not exist.");
            }

            var themeStories = stories["themes"][selectedTheme]["stories"];
            if (themeStories == null || themeStories.Count == 0)
            {
                throw new Exception($"No stories found for theme '{selectedTheme}'.");
            }

            Random random = new Random();
            int randomIndex = random.Next(themeStories.Count);
            return themeStories[randomIndex];
        }

        static List<string> CollectUserInputs(dynamic story)
        {
            var userInputs = new List<string>();
            var placeholders = story["placeholders"];

            // Ensure placeholders is not null
            if (placeholders == null)
            {
                throw new Exception("No placeholders found in the story.");
            }

            foreach (var placeholder in placeholders)
            {
                Console.Write($"{placeholder["prompt"]}: ");
                string input = Console.ReadLine();

                // Strict validation: re-prompt if input is empty
                while (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty. Please try again.");
                    Console.Write($"{placeholder["prompt"]}: ");
                    input = Console.ReadLine();
                }

                userInputs.Add(input);
            }

            return userInputs;
        }

        static string GenerateStory(dynamic story, List<string> userInputs)
        {
            string finalStory = string.Join(" ", story["story"]);
            for (int i = 0; i < userInputs.Count; i++)
            {
                // Find the first occurrence of the placeholder
                int index = finalStory.IndexOf("___");
                if (index != -1)
                {
                    // Replace it with the user input in green
                    finalStory = finalStory.Remove(index, 3) // Remove the placeholder
                                           .Insert(index, $"\u001b[32m{userInputs[i]}\u001b[0m"); // Insert the user input
                }
            }

            // Format the story to fit within the specified width
            return FormatStory(finalStory, 60);
        }

        static string FormatStory(string story, int width)
        {
            var words = story.Split(' ');
            var formattedStory = new System.Text.StringBuilder();
            var currentLine = new System.Text.StringBuilder();

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 1 > width) // +1 for space
                {
                    formattedStory.AppendLine(currentLine.ToString());
                    currentLine.Clear();
                }
                currentLine.Append(word + " ");
            }

            // Append any remaining words
            if (currentLine.Length > 0)
            {
                formattedStory.AppendLine(currentLine.ToString());
            }

            return formattedStory.ToString();
        }
    }
}