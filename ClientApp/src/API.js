import axios from "axios";

export default {
    asset: {
        // Get
        getAssets: () => axios.get('api/asset'),
        getAsset: query => {
            if (!query) return this.messages.badInput;
            return axios.get(`api/asset/${query}`)
        },
        getShares: query => {
            return axios.get(query ? `api/asset/shares/${query}` : 'api/asset/shares');
        },
        getPlayerShares: query => {
            if (!query) return this.messages.badInput;
            return axios.get(`api/asset/player-shares/${query.id}/${query.type}/${query.qty}`);
        },
        // Post
        createAsset: query => {
            if (!query) return this.messages.badInput;
            return axios.post(`api/asset`, query);
        },
        addShares: query => {
            if (!query) return this.messages.badInput;
            return axios.post(`api/asset/add-shares/${query.id}/${query.qty}/${query.type}`)
        },
        // Put
        updateAsset: query => {
            if (!query) return this.messages.badInput;
            return axios.put(`api/asset/${query.id}`, query.changeSet);
        },
        deleteAsset: query => {
            if (!query) return this.messages.badInput;
            return axios.post(`api/asset/delete-asset`, query);
        },
        // Delete
        deleteGameAssets: query => axios.delete(`api/asset/game-assets/${query}`),
    },
    player: {
        // Get
        getPlayers: query => {
            return axios.get(`api/player/${query}`)
        },
        getPlayer: query => {
            if (!query) return this.messages.badInput;
            return axios.get(`api/player/${query}`);
        },
        // Post
        createPlayer: query => {
            if (!query) return this.messages.badInput;
            return axios.post('api/player/new-player', query);
        },
        createPlayers: query => {
            if (!query) return this.messages.badInput;
            //console.log("create players: ", query);
            //console.log("create players url: ", `api/player/create-players/${query.gameid}`);
            return axios.post(`api/player/create-players/${query.gameid}`, query.players);
        },
        addSharesToPlayer: query => {
            if (!query) return this.messages.badInput;
            return axios.post(`api/player/${query.playerId}/${query.assetId}/${query.qty}`, query.changeSet);
        },
        // Put
        updatePlayer: query => {
            if (!query) return this.messages.badInput;
            return axios.put(`api/player/${query.id}`, query.changeSet);
        },
        clearPortfolio: query => {
            if (!query) return this.messages.badInput;
            return axios.put(`api/player/${query}`);
        },
        clearWallet: query => {
            if (!query) return this.messages.badInput;
            return axios.put(`api/player/${query}`);
        },
        // Delete
        deletePlayer: query => {
            if (!query) return this.messages.badInput;
            return axios.delete(`api/player/${query}`);
        }
    },
    transactions: {
        submitTrade: query => {
            if (!query) return this.messages.badInput;
            return axios.post('api/transaction', query);
        }
    },
    gamePlay: {
        // Get
        newGame: qty => axios.get(`api/game/new-game/${qty}`),
        getData: query => query.gameId && query.assetId && query.since ? axios.get(`api/transaction/get-trades/${query.gameId}/${query.assetId}/${query.since}`) : "Something went wrong",
        // Post
        onOff: query => axios.post(`api/game/on-off/${query.gameId}/${query.isRunning}`),
        tradingOnOff: query => axios.post(`api/game/trading-on-off/${query.gameId}/${query.isRunning}`),
        // Put
        initialize: secretCode => axios.put(`api/game/blowup-the-inside-world`, `"${secretCode}"`, { headers: { "content-type": "application/json" } }),
        // Delete
        gameOver: gameId => axios.delete(`api/game/end-game`, gameId),
    },
    messages: {
        badInput: "Huh? Didn't hear that"
    }
};
