import React, { useState, useEffect } from 'react';
import InputGroup from 'react-bootstrap/InputGroup';
import Form from 'react-bootstrap/Form';
import API from './../../API';
import ListResults from './ListResults';

export const PlayerCreate = props => {

    const [playerName, SETplayerName] = useState("");
    const [playerCash, SETplayerCash] = useState(0);
    const [playerNameMessage, SETplayerNameMessage] = useState("Required: All players must have a name.");
    const [playerCashMessage, SETplayerCashMessage] = useState("Required: Must have cash to play?");
    const [submitMessage, SETsubmitMessage] = useState("");

    const [playerNameDisplay, SETplayerNameDisplay] = useState(false);
    const [playerCashDisplay, SETplayerCashDisplay] = useState(false);
    const [submitDisplay, SETsubmitDisplay] = useState(false);
    const [gotPlayers, SETgotPlayers] = useState(false);
    const [player, SETplayer] = useState({});
    const [cash, SETcash] = useState({});
    const [cashShares, SETcashShares] = useState([]);
    const [shares, SETshares] = useState([]);
    const [allPlayers, SETallPlayers] = useState([]);
    const [resultsColumns, SETresultsColumns] = useState(["Name", "Cash", "Shares", "Id"]);
    const [resultsItems, SETresultsItems] = useState([]);
    const [resultsStyle, SETresultsStyle] = useState({});

    useEffect(() => {
        getPlayers();
    },[])

    // **********
    // GO GETTERS
    // **********
    const getCash = () => {
        API.asset.getPlayerShares({ id: player.id, type: 3, qty: 0 }).then(cash => {
            SETcashShares(cash.data);
            setPlayerAndCash(cash.data[0]);
            getShares();
        })
    };
    const getShares = () => {
        API.asset.getPlayerShares({ id: player.id, type: 1, qty: 0 }).then(shares => {
            SETshares(shares.data);
        })
    };
    const getPlayers = () => {
        API.player.getPlayers().then(players => {
            let items = [];
            for (let plyr of players.data) {
                console.log("player of get players data: ", plyr);
                items.push({ body: [plyr.name, plyr.cash ? plyr.cash : 0, plyr.shares ? plyr.shares : 0, plyr.id] });
            }
            SETallPlayers(items);
            SETgotPlayers(true);
            console.log("got players: ", items);
        });
    }

    // ********
    // SERVICES
    // ********
    const addCash = (id, qty) => {
        API.player.getPlayer(id).then(player => {
            SETcashShares(player.data.wallet);
        })
    };
    const setPlayerAndCash = cash => {
        props.state.SETcash(cash);
        props.state.SETplayer(player);
    };

    // **************
    // EVENT HANDLING
    // **************
    const handleSubmit = event => {
        if (playerName && playerCash) {
            API.player.createPlayer({
                Name: playerName,
                Cash: playerCash
            }).then(result => {
                SETsubmitMessage("submitted successfully");
                SETsubmitDisplay(true);
                SETplayer(result.data);
                getPlayers();
                setTimeout(() => SETsubmitDisplay(false), 3333);
                addCash(result.data.id, playerCash);
                getCash();
                setPlayerAndCash();
            });
        }
        else {
            SETsubmitMessage("fill in the missing information");
            SETsubmitDisplay(true);
        }
        setTimeout(() => SETsubmitDisplay(false), 3333);
    }
    const handleBlur = event => {
        event.preventDefault();
        const target = event.target;
        const eventName = target.name;
        const value = target.value;

        switch (eventName) {
            case "playerName":
                // validate name
                if (!playerName) {
                    SETplayerNameDisplay(true);
                }
                else {
                    SETplayerNameDisplay(false);
                }
                break;
            case "playerCash":
                // validate cash
                if (!playerCash) {
                    SETplayerCashDisplay(true);
                }
                else {
                    SETplayerCashDisplay(false);
                }
                break;
            default:
            // don't do anything
        }
    };


    return (
        <div>
            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="playerName">Player Name: </InputGroup.Text>
                </InputGroup.Prepend>
                <Form.Control
                    placeholder="Player Name"
                    aria-label="PlayerName"
                    aria-describedby="playerName"
                    onChange={(event) => SETplayerName(event.target.value)}
                    name="playerName"
                    onBlur={handleBlur}
                    type="text"
                />
            </InputGroup>
            <p className={playerNameDisplay ? 'show playerName-message' : 'hide playerName-message'}>{playerNameMessage}</p>

            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="playerCash">Player Cash: </InputGroup.Text>
                </InputGroup.Prepend>
                <Form.Control
                    placeholder="Player Cash"
                    aria-label="PlayerCash"
                    aria-describedby="playerCash"
                    onChange={(event) => SETplayerCash(event.target.value)}
                    name="playerCash"
                    onBlur={handleBlur}
                    type="number"
                />
            </InputGroup>
            <p className={playerCashDisplay ? 'show playerCash-message' : 'hide playerCash-message'}>{playerCashMessage}</p>

            <button id="submit" onClick={handleSubmit}>Submit</button>
            <p className={submitDisplay ? 'show submit-message' : 'hide submit-message'}>{submitMessage}</p>

            {
                gotPlayers ?
                    <div>
                        <h3>Players</h3>
                        <ListResults
                            columns={resultsColumns}
                            items={allPlayers}
                            tableName="players-table"
                            style={resultsStyle}
                        />
                    </div> : <p>nuttin to see here</p>
            }
        </div>
    );
}

export default PlayerCreate;

//let theProps = {
//    columns: ["string"],
//    items: [{
//        body: ["string"]
//    }]
//}