# RiskGame.API
This is ostensibly a game to explore risk, but it's really a good way to practice working with the full stack. The back end is written in **C#** and uses **MongoDB** for storage and is served up by IIS Express.
So far there is a database set up with three collections:
> - Players
> - Assets
> - Shares

The **Players** have a portfolio of **Shares** and a portfolio of cash. Each **Share** is one of a limited number to represent an **Asset**.
Each of these has its own service to handle database transactions.
The Player and the Asset both have their own controller to handle endpoint requests. Additionally there is a Transactions controller which does not speak to the database directly but instead handles transaction requests to trade shares among players through player, asset and share services.

Each Asset has a unique Guid
Each Share also has a unique Guid
Cash is an asset and follows the same rules involving shares

Updated 3/9/2021
