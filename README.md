# RiskGame.API
**3/9/2021
This is** ostensibly a game to explore risk, but it's really a good way to practice working with the full stack. The back end is written in **C#** and uses **MongoDB** for storage and is served up by IIS Express.  
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


**3/14/21
Added controller** functionality. Assets can be created and their shares are automatically created.  
Creating a player requires a {string} Name and the amount of {int} Cash to start with. If this is the first player then it will also create an asset for cash and the number of shares the player starts with. It then adds them to the player's wallet.  
Wired up a React front  

3/15/21
**Worked through** the logic to add a player called **HAUS** in PlayerService following similar logic to Cash. As new assets are created the shares are added to HAUS's portfolio.
Ironed out the logic to Trade between players.
Built onto the front end and wired up state transfer between the components. 

3/21/21
**Separated out** some processes and cleaned up some server code. The front end consists of a form to create an **Asset**, the number of **Shares** and the amount of **Cash** for the house.  
There is also a form to create a **Player** and an amount of cash for them.  
Once both an **Asset** and a **Player** have been created the **Place a Trade** button becomes active. This opens the **Trading Component**.  
Trading is basically done between the player and the House who acts as the "Specialist Post" like in the NY Stock Exchange.

3/24/21
**Muches and** muches of time spent troubleshooting the front end components. Muches less time spent troubleshooting the back.  
I added a cool new component that takes an arbitrary number of column headings and an array of objects with the data and it presents the data. Currently it is pinned to the bottom of the Player Create form. The idea is to present a dropdown list of player options in case I decide not to create new players.  
