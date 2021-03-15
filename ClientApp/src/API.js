import axios from "axios";

export default {
    asset: {
        // Get
        getAssets: () =>  axios.get('api/asset'),
        getAsset: query => {
            console.log("query: ", query);
            return axios.get(`api/asset/${query}`);
        },
        getShares: query => {
            console.log("query: ", query);
            return axios.get(`api/asset/shares/${query}`);
        },
        // Post
        createAsset: query => {
            console.log("query: ", query);
            return axios.post(`api/asset`, query);
        },
        addShares: query => {
            console.log("query: ", query);
            return axios.post(`api/asset/${query.id}/${query.qty}`)
        },
        // Put
        updateAsset: query => {
            console.log("query: ", query);
            return axios.put(`api/asset/${query.id}`, query.changeSet);
        },
        // Delete
        deleteAsset: query => {
            console.log("query: ", query);
            return axios.delete(`api/asset/${query}`);
        }
    },
    player: {
        // Get
        getPlayers: () => axios.get('api/player'),
        getPlayer: query => {
            console.log("query: ", query);
            return axios.get(`api/player/${query}`);
        },
        // Post
        createPlayer: query => {
            console.log("query: ", query);
            return axios.post('api/player/', query);
        },
        addSharesToPlayer: query => {
            console.log("query: ", query);
            return axios.post(`api/player/${query.playerId}/${query.assetId}/${query.qty}`, query.changeSet);
        },
        // Put
        updatePlayer: query => {
            console.log("query: ", query);
            return axios.put(`api/player/${query.id}`, query.changeSet);
        },
        clearPortfolio: query => {
            console.log("query: ", query);
            return axios.put(`api/player/${query}`);
        },
        clearWallet: query => {
            console.log("query: ", query);
            return axios.put(`api/player/${query}`);
        },
        // Delete
        deletePlayer: query => {
            console.log("query: ", query);
            return axios.delete(`api/player/${query}`);
        }
    },
    transactions: {
        submitTrade: query => {
            console.log("query: ", query);
            return axios.post('api/transaction', query);
        }
    }
};
