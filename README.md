# WinBot
WinBot is a Discord bot designed specifically for use in the WinWorld Discord server.

## Usage/Build instructions
1. Clone the repository: ``https://github.com/CamK06/WinBot.git`` and move into the project directory: ``cd WinBot``
2. Build the source code: ``dotnet build -c Release -r linux-x64``
3. Change into the build directory: ``cd bin/Release/net6.0/linux-x64/``
4. Run the bot: ``./WinBot`` or just ``WinBot`` for Windows. This will generate a blank configuration file for you.
5. Edit the ``config.json``  file and add your token into the token field, as well as your client ID and log channel if you want one
6. Run the bot once more, as before. Once it has started up (It'll output "Ready" to the terminal), you should be good to go into Discord and use it.

## Post-install instructions
For the level rank cards to work you'll need to install Roboto. Due to some annoyances with System.Drawing you have to install it twice in a way.
Steps:

1. Download the font family https://fonts.google.com/specimen/Roboto
2. Extract Roboto-Regular.ttf into the bot's working directory
3. Install Roboto-Regular.ttf on your system

*If you're on Windows there's some changes you need to make to the build process but I'm sure you can figure that out, as you do.*
