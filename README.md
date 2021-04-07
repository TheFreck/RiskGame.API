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

**3/15/21
Worked through** the logic to add a player called **HAUS** in PlayerService following similar logic to Cash. As new assets are created the shares are added to HAUS's portfolio.
Ironed out the logic to Trade between players.
Built onto the front end and wired up state transfer between the components. 

**3/21/21
Separated out** some processes and cleaned up some server code. The front end consists of a form to create an **Asset**, the number of **Shares** and the amount of **Cash** for the house.  
There is also a form to create a **Player** and an amount of cash for them.  
Once both an **Asset** and a **Player** have been created the **Place a Trade** button becomes active. This opens the **Trading Component**.  
Trading is basically done between the player and the House who acts as the "Specialist Post" like in the NY Stock Exchange.

**3/23/21
Muches and** muches of time spent troubleshooting the front end components. Muches less time spent troubleshooting the back.  
I added a cool new component that takes an arbitrary number of column headings and an array of objects with the data and it presents the data. Currently it is pinned to the bottom of the Player Create form. The idea is to present a dropdown list of player options in case I decide not to create new players.  

**3/24/21
Started working** on the economy today. Essentially there are 6 industries: ROY G BV. Each asset now has a company asset which is the Enterprise company. This Enterprise company is in one of the 6 industries. It also has a Cyclicality metric between -1 and 1 which tells how strongly correlated this company is with its industry. The economy has a SET metric called MarketTrendiness which is a value from 0-9. Each round the economy calculates a growth rate for each industry and also calculates a random number from 0-10. If the random number is greater than the Market Trendiness then that particular industry will grow in the opposite direction it had previously been growing.
I also fixed some stuff with the Generalized Results template to make it more generalizeable.

**3/26/21
Created a** few new entities and a new controller to turn the game on and off. One new entity is called Agenda which basically progresses the game forward. Right now it takes all the assets and applies a growth formula on them depending on their industry, cyclicality and random numbers. So far it generates the data. Next step is to capture that data and do sumpin widit.

**3/27/21
Built a** time series chart today. Right now it lacks axis labels of any kind but it displays the value of a single asset over time.

**3/29/21
Reworked a** lot of the logic of the progression of the market. Added Thread.Sleep to control the speed of the progression. Renamed the Agenda class to Economy and Economy to Market. Agenda was a dumb name from the start! So a new game is a new instance of an Economy. It sends its id back to the client so that assets, players and transactions can occur within a game. Made a better start and stop switch and also cleaned up some obsolete methods.

**4/3/21
The backend** game loop is working well. In the front I am using two funcitons to handle the game loop. One checks with the endpoint to see if the game is running on the server. If so it calls the other which sets a timeout, gets the game data and then calls the other function to restart the cycle. I worked on syncronizing front and back until I started seeing double. But it wasn't until I was seeing multiples of double vision that I turned it off for the night.

**4/5/21
I've been** workin on the game loop. All the live long day. I've gotten it to where I can start and stop the front from grabbing data. The back still starts and stops with Postman. Of course I hit every bug imaginable and unimaginable and then when I was so proud I realized it gets to 101 cycles and then re-renders the same data over again. I'm still feeling pretty good but I know what I'm working on tomorrow.

**4/6/21
This has gotten big enough that I created an Azure task board to keep track of everything. The current Epic involves Charting. Next will come Trading and finally AI Players. I synchronized the game loop between front and back. Started refactoring so that it sends a packet of Open, Close, High, Low and Volume from all the server processes since the last request. So far so simple on the server side. Started making a candlestick chart pixel and began refactoring the chart rendering itself. So far so much work but nothing yet to show for it. I should be able to fix it tomorrow.