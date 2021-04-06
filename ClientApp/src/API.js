import axios from "axios";

export default {
    asset: {
        // Get
        getAssets: () => axios.get('api/asset'),
        getCash: query => {
            //console.log(`api/asset/cash/${query}`);
            return axios.get(`api/asset/cash/${query}`);
        },
        getAsset: query => {
            //console.log("getAsset query: ", query);
            if (!query) return this.messages.badInput;
            return axios.get(`api/asset/${query}`)
        },
        getShares: query => {
            //console.log("getShares query: ", query ? query : "cash");
            return axios.get(query ? `api/asset/shares/${query}` : 'api/asset/shares');
        },
        getPlayerShares: query => {
            //console.log("getPlayerShares query: ", query);
            if (!query) return this.messages.badInput;
            return axios.get(`api/asset/player-shares/${query.id}/${query.type}/${query.qty}`);
        },
        // Post
        createAsset: query => {
            //console.log("createAsset query: ", query);
            if (!query) return this.messages.badInput;
            return axios.post(`api/asset`, query);
        },
        addShares: query => {
            //console.log("addShares query: ", query);
            //console.log("API: ", `api/asset/add-shares/${query.id}/${query.qty}/${query.type}`);
            if (!query) return this.messages.badInput;
            return axios.post(`api/asset/add-shares/${query.id}/${query.qty}/${query.type}`)
        },
        // Put
        updateAsset: query => {
            //console.log("updateAsset query: ", query);
            if (!query) return this.messages.badInput;
            return axios.put(`api/asset/${query.id}`, query.changeSet);
        },
        // Delete
        deleteAsset: query => {
            //console.log("deleteAsset query: ", query);
            if (!query) return this.messages.badInput;
            return axios.delete(`api/asset/${query}`);
        }
    },
    player: {
        // Get
        getPlayers: query => {
            //console.log(`api/player/${query}`);
            return axios.get(`api/player/${query}`)
        },
        getPlayer: query => {
            //console.log("getPlayer query: ", query);
            if (!query) return this.messages.badInput;
            return axios.get(`api/player/${query}`);
        },
        // Post
        createPlayer: query => {
            //console.log("createPlayer query: ", query);
            if (!query) return this.messages.badInput;
            return axios.post('api/player/new-player', query);
        },
        addSharesToPlayer: query => {
            //console.log("add shares query: ", query);
            if (!query) return this.messages.badInput;
            return axios.post(`api/player/${query.playerId}/${query.assetId}/${query.qty}`, query.changeSet);
        },
        // Put
        updatePlayer: query => {
            //console.log("updatePlayer query: ", query);
            if (!query) return this.messages.badInput;
            return axios.put(`api/player/${query.id}`, query.changeSet);
        },
        clearPortfolio: query => {
            //console.log("clearPortfolio query: ", query);
            if (!query) return this.messages.badInput;
            return axios.put(`api/player/${query}`);
        },
        clearWallet: query => {
            //console.log("clearWallet query: ", query);
            if (!query) return this.messages.badInput;
            return axios.put(`api/player/${query}`);
        },
        // Delete
        deletePlayer: query => {
            //console.log("deletePlayer query: ", query);
            if (!query) return this.messages.badInput;
            return axios.delete(`api/player/${query}`);
        }
    },
    transactions: {
        submitTrade: query => {
            //console.log("submit trade query: ", query);
            if (!query) return this.messages.badInput;
            return axios.post('api/transaction', query);
        }
    },
    gamePlay: {
        initialize: secretCode => {
            axios.delete(`api/asset/game/start/initialize/${secretCode}`).then(assetDrop => {
                console.log(assetDrop);
                if (assetDrop.status == 200) {
                    return (axios.delete(`api/player/game/start/initialize/${secretCode}`).then(playerDrop => {
                        console.log(playerDrop.data);
                    }));
                }
                else {
                    return assetDrop;
                }
            })
        },
        newGame: () => axios.get("api/game/new-game"),
        onOff: gameId => axios.post(`api/game/on-off/${gameId}`),
        isGameOn: gameId => axios.get(`api/game/get-game-status/${gameId}`),
        getData: gameId => {
            console.log(`api/game/get-records/${gameId}`);
            console.log("axios.get(`api/game/get-records/${gameId}`): ", axios.get(`api/game/get-records/${gameId}`));
            return axios.get(`api/game/get-records/${gameId}`);
        },
        next: query => {
            console.log(`api/game/next/${query.frames}/${query.trendiness}`);
            return axios.get(`api/game/next/${query.frames}/${query.trendiness}`);
        },
        addAssets: () => axios.get("api/game/add-assets"),
    },
    messages: {
        badInput: "Huh? Didn't hear that"
    }
};
