# WinBot

This is a version of WinBot that is specifically designed for Microsoft Windows; that is, it removes the detector to see if it is running on it.

## Instructions

### Downloading

#### Code

Firstly, you will have to go click ``Code``, and then ``Download ZIP``. After that, once it is downloaded, you will have to extract it using the file extractor that you use. 

#### Tools

Now, go to https://www.dotnet.microsoft.com/download and select "Download .NET SDK x64" to download the software development kit required to compile this bot.

### Installing

#### Tools

After you download the tools, run the installer, and follow the steps that the installer guides you through.

### Building

Now, open up a command prompt and go to the directory that you extracted WinBot to and then run ``dotnet build -c Release -r win-x64``.

### Running

Firstly, go to https://www.discord.com/developers and then click on "New Application" and then specify the name of your bot. 

Now, go to `Bot` and click `Add Bot`. 

Now, you have to change in to the directory where the binaries are located (which in this case it is ``cd bin/Release/net5.0/win-x64/``). And then, type `WinBot` and then it will create a blank configuration file (which is called `config.json`). 

Now, edit the file with the text editor you use. Now, you have to make it match the settings of your bot. After that, run the bot; now, click on ``OAuth2`` and copy the client ID. After that, go to https://discord.com/api/oauth2/authorize?client_id=ID&scope=bot. 

You will obviously have to replace the "ID" with your client ID that you just copied. 

### Testing

After that, authorize the bot to go in to your server, and you should be ready to use it! To make sure everything is working, run `.ping` and you should get a response from the bot showing the latency.
