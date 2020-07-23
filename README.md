![Build CoraBot](https://github.com/Community-Operations-Resource-Agent/CoraBot/workflows/Build%20CoraBot/badge.svg) ![Deploy Bot to Azure Function App](https://github.com/Community-Operations-Resource-Agent/CoraBot/workflows/Deploy%20Bot%20to%20Azure%20Function%20App/badge.svg?branch=master)

# About CoraBot
Welcome to the open source implementation of the Resource Connector for Nonprofits - a bot-based Azure Function that handles incoming & outgoing messages.  The bot currently leverages SMS (via Twilio) for connecting people.  This is currently an ACTIVE and OPEN SOURCE project.

# Architecture
TODO: architecture diagram

# Getting Started for Developers
## Local Development
1. Download and install [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) with the following components
    * .NET desktop development (Workload)
    * ASP.NET and web development (Workload)
    * .NET Core 2.2 Runtime (EOL) (Individual Component)
2. Clone this repo and open [CORA.sln](https://github.com/Community-Operations-Resource-Agent/CoraBot/blob/master/CORA.sln) file in Visual Studio
3. Install and run [Azure Cosmos DB emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator#installation) using the default settings.
4. Install the [Bot Framework Emulator v4](https://github.com/Microsoft/BotFramework-Emulator/releases/tag/v4.9.0)
5. Build and run the bot using the "Bot" project
6. Start the Bot Framework Emulator and connect to the bot by clicking "Open Bot" and entering the Bot URL as `http://localhost:5001/api/messages`
7. You should now be able to interact with the bot via the emulator

# Source Policies & Procedures
TODO:  Information about how to fork the repo, submit pull requests/improvements and continue improving the implementation
Also include information about the github actions (automatic build/publish)

# License
TODO: Need to decide on which license and put info here
