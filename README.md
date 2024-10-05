# Mad Libs Game

## Description

The Mad Libs Game is a fun and interactive text-based game where players fill in the blanks to create humorous stories. The game prompts users for various inputs, which are then inserted into a randomly selected story template, resulting in a unique and often hilarious narrative.

## Features

- **Theme Selection**: Players can choose from various themes to customize their story.
- **Random Story Selection**: The game randomly selects a story based on the chosen theme.
- **User Input Collection**: Players are prompted to provide inputs for placeholders in the story, ensuring a personalized experience.
- **Story Generation**: The game generates a final story by replacing placeholders with user inputs and displays it in a formatted manner.
- **Graceful Exit**: The game handles interruptions (like CTRL+C) gracefully, allowing for a smooth exit.

## Getting Started

### Prerequisites

- .NET SDK (version 5.0 or later)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/jlchafardet/mad-libs.git
   cd mad-libs
   ```

2. Restore the dependencies:

   ```bash
   dotnet restore
   ```

### Running the Game

To run the Mad Libs game, use the following command:

```bash
dotnet run
```

### How to Play

1. Upon starting the game, you will see the title and available themes.
2. Select a theme by entering the corresponding number.
3. Follow the prompts to fill in the blanks for the story.
4. After providing all necessary inputs, the game will display the final story.
5. Enjoy the humorous results!

## Author

- **Name**: José Luis Chafardet G.
- **Email**: jose.chafardet@icloud.com
- **GitHub**: [jlchafardet](https://github.com/jlchafardet)

## License

This project is licensed under the GNU General Public License (GPL) Version 3. You can redistribute and modify this software under the terms of this license. For more details, see the [LICENSE](LICENSE) file.

## Acknowledgments

- Thanks to all contributors and resources that helped in the development of this game.
