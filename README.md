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
This has** gotten big enough that I created an Azure task board to keep track of everything. The current Epic involves Charting. Next will come Trading and finally AI Players. I synchronized the game loop between front and back. Started refactoring so that it sends a packet of Open, Close, High, Low and Volume from all the server processes since the last request. So far so simple on the server side. Started making a candlestick chart pixel and began refactoring the chart rendering itself. So far so much work but nothing yet to show for it. I should be able to fix it tomorrow.

**4/7/21
Today I** made great progress on the chart. On the front end I made a candlestick chart pattern to represent the OHLC data and replace the plain blue pixel that only showed one value. I was able to render the candlestick with dummy data but then after connecting it back up to the server to get game data I spent the rest of my time chasing bugs. It's close but I ran out of time today.

**4/13/21
The past** few days I've been working on the charting. I've hit something strange where the server loop has been set to run once every 100 milliseconds and the client runs once every 1000 milliseconds. I expected each round of the client to pick up roughly 10 packets of data each time. What's actually happening is that it gets 2-3. When I run the server without the client it run very close to once every 100 milliseconds. It seems strange that the process of asking for data would slow it down that much. I've been working on other things too but this has been a constant simmer. Anyhow, today I wrote the logic to pay dividends.  
I'm working through in my mind the decision making of the Players. Firstly each AI player has a risk tolerance and if their portfolio is too risky they will sell stock and if it's too safe they will buy. Additionally they will evaluate information about the stocks themselves. In terms of what information is available to them I have a few thoughts:  
> - rolling average of each industry's growth rate  
> - rolling average of the company's earnings (the value of the **CompanyAsset** held by the asset multiplied by the equity multiplier (Assets/Equity))  
> - perhaps down the road I can make certain information available only to some and other available only to others so none of the players have the complete picture  
> - perhaps some players can be more sophisticated than others and can incorporate more data points into their decisions  
> - each player should be limited to a certain number of data points of the data available. Perhaps selecting which points is handled by a random process  
> - each player calculates the expected growth of the stock according to the data they have. If it is greater than the StockPrice-CompanyAsset*EquityMultiplier then they want to buy. The opposite is true to sell. A built in margin around the values will handle hold situations. In other words, the difference must be greater than a certain threshhold for a trade to be triggered
I'll probably update this list but the key is that there is no market real or imagined with an even distribution of information. One of the unexpected virtues of market dynamics is that out of this imperfect knowledge rises a "market knowledge" which incorporates all the bits of knowledge 

**4/14/21
Part of** the issue with the chart loop is that it takes more time to run each time it runs. So I'm doing some refactoring of the process and separating out a lot of functionality. This is a win independent of progress on the bug.

**4/15/21
With additional** testing using the StopWatch class the loop is running at roughly the same speed each time which is around 430 milliseconds. That actually explains a lot of the server/chart sync issues I was hitting. In the end, though, this loop doesn't need to run as frequently as it is right now so no worries. I've just been using this data stream to create the chart, but the actual data displayed on the chart will be the result of trades made by the human and AI players.

**4/19/21
Made some** really great progress on the Player decision making and the player loop. In fact, I'm ready to start testing and refining it. So instead of doing that I ran into the reason we do a Repository Class for each model instead of just letting the model's service class handle it. CIRCULAR REFERENCES!!! when the services use each other to access their models GRRRRRRRR... So I created a Player Repo and tomorrow will follow suit with the other models and hopefully I'll be able to test the player loop tomorrow.

**4/20/21
Spent all day refactoring everything around the Repo classes. That allowed me to sever the ties between the service classes which was causing the circular reference. The server is a much cleaner, more efficient machine but the client is feeling jealous of all the attention the server got today and stopped creating new players and geting cash. So tomorrow I debug that...  

