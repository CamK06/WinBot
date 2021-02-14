# WinBot
WinBot is a Discord bot designed specifically for use in the [WinWorld Discord server](https://discord.gg/HepVSYH).

## Usage/Build instructions
1. Clone the repository: ``https://github.com/Starman0620/WinBot.git`` and move into the project directory: ``cd WinBot``
2. Build the source code: ``dotnet build -c Release -r linux-x64`` (If you're building on Windows, the -r option should be win-x64) 
3. Change into the build directory: ``cd bin/Release/net5.0/linux-x64/`` (If you're on Windows, you'll once again need to replace linux-x64 with win-x64)
4. Run the bot: ``./WinBot`` or just ``WinBot`` for Windows. This will generate a blank configuration file for you.
5. Edit the ``config.json``  file and add your token into the token field, aswell as your client ID and log channel if you want one
6. Run the bot once more, as before. Once it has started up (It'll output "Ready" to the terminal), you should be good to go into Discord and use it.
