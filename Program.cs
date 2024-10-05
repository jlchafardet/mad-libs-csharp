﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MadLibsGame
{
    class Program
    {
        static void Main(string[] args)
        {
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

            // Select the theme based on the user's input
            string selectedTheme = themes[themeIndex - 1]; // Adjust for zero-based index

            // Select a random story from the chosen theme
            var story = SelectRandomStory(stories, selectedTheme);

            // Collect user inputs for placeholders
            var userInputs = CollectUserInputs(story);

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

        static void DisplayTitle(string title)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("╔" + new string('═', 58) + "╗");
            Console.WriteLine("║" + title.PadLeft((60 + title.Length) / 2).PadRight(60) + "║");
            Console.WriteLine("╚" + new string('═', 58) + "╝");
            Console.ResetColor();
        }

        static dynamic LoadStories(string path)
        {
            // Load and parse the JSON file
            string json = File.ReadAllText(path);
            var stories = JsonConvert.DeserializeObject<dynamic>(json);
            
            // Check if stories is null
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
            if (stories["themes"][selectedTheme] == null)
            {
                throw new Exception($"Theme '{selectedTheme}' does not exist.");
            }

            var themeStories = stories["themes"][selectedTheme]["stories"];
            Random random = new Random();
            int randomIndex = random.Next(themeStories.Count);
            return themeStories[randomIndex];
        }

        static List<string> CollectUserInputs(dynamic story)
        {
            var userInputs = new List<string>();
            var placeholders = story["placeholders"];

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
                finalStory = finalStory.Replace("___", userInputs[i], StringComparison.Ordinal); // Use StringComparison
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