import axios from "axios";

export default {
    asset: {
        // Get
        getAssets: () => axios.get('api/asset'),
        getCash: query => {
            return axios.get(`api/asset/cash/${query}`);
        },
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
            //debugger;
            return axios.post('api/player/new-player', query);
        },
        createPlayers: query => {
            if (!query) return this.messages.badInput;
            console.log("create players: ", query);
            return axios.post(`api/player/create-players`, query);
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
        isGameOn: gameId => axios.get(`api/game/get-game-status/${gameId}`),
        getData: query => axios.get(`api/game/get-records/${query.gameId}/${query.lastFrame}`),
        next: query => axios.get(`api/game/next/${query.frames}/${query.trendiness}`),
        addAssets: () => axios.get("api/game/add-assets"),
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
