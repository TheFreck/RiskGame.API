import axios from "axios";

export default {
    asset: {
        // Get
        getAssets: () =>  axios.get('api/asset'),
        getAsset: query => {
            console.log("getAsset query: ", query);
            if (!query) return this.messages.badInput;
            return axios.get(`api/asset/${query}`);
        },
        getShares: query => {
            console.log("getShares query: ", query ? query : "cash");
            return axios.get(query ? `api/asset/shares/${query}` : 'api/asset/shares');
        },
        getPlayerShares: query => {
            console.log("getPlayerShares query: ", query);
            if (!query) return this.messages.badInput;
            return axios.get(`api/asset/player-shares/${query.id}/${query.type}`);
        },
        // Post
        createAsset: query => {
            console.log("createAsset query: ", query);
            if (!query) return this.messages.badInput;
            return axios.post(`api/asset`, query);
        },
        addShares: query => {
            console.log("addShares query: ", query);
            if (!query) return this.messages.badInput;
            return axios.post(`api/asset/${query.id}/${query.qty}`)
        },
        // Put
        updateAsset: query => {
            console.log("updateAsset query: ", query);
            if (!query) return this.messages.badInput;
            return axios.put(`api/asset/${query.id}`, query.changeSet);
        },
        // Delete
        deleteAsset: query => {
            console.log("deleteAsset query: ", query);
            if (!query) return this.messages.badInput;
            return axios.delete(`api/asset/${query}`);
        }
    },
    player: {
        // Get
        getPlayers: () => axios.get('api/player'),
        getPlayer: query => {
            console.log("getPlayer query: ", query);
            if (!query) return this.messages.badInput;
            return axios.get(`api/player/${query}`);
        },
        // Post
        createPlayer: query => {
            console.log("createPlayer query: ", query);
            if (!query) return this.messages.badInput;
            return axios.post('api/player/', query);
        },
        addSharesToPlayer: query => {
            console.log("add shares query: ", query);
            if (!query) return this.messages.badInput;
            return axios.post(`api/player/${query.playerId}/${query.assetId}/${query.qty}`, query.changeSet);
        },
        // Put
        updatePlayer: query => {
            console.log("updatePlayer query: ", query);
            if (!query) return this.messages.badInput;
            return axios.put(`api/player/${query.id}`, query.changeSet);
        },
        clearPortfolio: query => {
            console.log("clearPortfolio query: ", query);
            if (!query) return this.messages.badInput;
            return axios.put(`api/player/${query}`);
        },
        clearWallet: query => {
            console.log("clearWallet query: ", query);
            if (!query) return this.messages.badInput;
            return axios.put(`api/player/${query}`);
        },
        // Delete
        deletePlayer: query => {
            console.log("deletePlayer query: ", query);
            if (!query) return this.messages.badInput;
            return axios.delete(`api/player/${query}`);
        }
    },
    transactions: {
        submitTrade: query => {
            console.log("submit trade query: ", query);
            if (!query) return this.messages.badInput;
            return axios.post('api/transaction', query);
        }
    },
    initialize: secretCode => {
        console.log("secretCode query: ", secretCode);
        return axios.post('api/asset/game/start/initialize', secretCode);
    },
    messages: {
        badInput: "Huh? Didn't hear that"
    }
};
