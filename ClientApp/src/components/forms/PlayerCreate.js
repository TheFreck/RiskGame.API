import React, { useState, useEffect } from 'react';
import InputGroup from 'react-bootstrap/InputGroup';
import Form from 'react-bootstrap/Form';
import API from './../../API';
import ListResults from './ListResults';

export const PlayerCreate = props => {
    //console.log("player props: ", props);
    const [gameId, SETgameId] = useState(props.gameId);
    const [playerName, SETplayerName] = useState("Charlie");
    const [playerCash, SETplayerCash] = useState(1000);
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
        if (props.gameId !== "" && props.gameId !== undefined && props.gameId !== null) SETgameId(props.gameId);
        getPlayers(props.gameId);
    }, [props.gameId]);

    // **********
    // GO GETTERS
    // **********
    const getCash = id => {
        API.asset.getPlayerShares({ id, type: 3, qty: 0 }).then(cash => {
            SETcashShares(cash.data);
            setPlayerAndCash(cash.data[0]);
            getShares(id);
        })
    };
    const getShares = id => {
        API.asset.getPlayerShares({ id, type: 1, qty: 0 }).then(shares => {
            SETshares(shares.data);
        })
    };
    const getPlayers = gameid => {
        API.player.getPlayers(gameid).then(players => {
            let items = [];
            for (let plyr of players.data) {
                items.push({ body: [plyr.name, plyr.cash ? plyr.cash : 0, plyr.shares ? plyr.shares : 0, plyr.id] });
            }
            SETallPlayers(items);
            SETgotPlayers(true);
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
    const setPlayerAndCash = player => {
        //props.state.SETcash(cash);
        props.state.player[1]({ player: player });
    };

    // **************
    // EVENT HANDLING
    // **************
    const handleSubmit = event => {
        if (playerName && playerCash) {
            API.player.createPlayer({
                Name: playerName,
                Cash: playerCash,
                GameId: gameId
            }).then(result => {
                //SETsubmitMessage("submitted successfully");
                //SETsubmitDisplay(true);
                SETplayer(result.data);
                getPlayers(result.data.gameId);
                //setTimeout(() => SETsubmitDisplay(false), 3333);
                addCash(result.data.id, playerCash);
                getCash(result.data.id);
                setPlayerAndCash(result.data);
                props.chartButtonClick(result.data.gameId, false);
            });
        }
        else {
            //SETsubmitMessage("fill in the missing information");
            //SETsubmitDisplay(true);
        }
        //setTimeout(() => SETsubmitDisplay(false), 3333);
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