import React, { Component } from 'react';
import InputGroup from 'react-bootstrap/InputGroup';
import Form from 'react-bootstrap/Form';
import API from './../../API';
import SharesResults from './SharesResults';

export class PlayerCreate extends Component {
    static displayName = PlayerCreate.name;

    constructor(props) {
        super(props);
        this.state = {
            playerName: "",
            playerCash: 0,

            playerNameMessage: "Required: All players must have a name.",
            playerCashMessage: "Required: Must have cash to play?",
            submitMessage: "",

            playerNameDisplay: false,
            playerCashDisplay: false,
            submitDisplay: false,

            player: {},
            cash: {},
            cashShares: [],
            shares: [],
            allPlayers: [],
            resultsCollumns: ["Name", "Cash", "Shares", "Id"],
            resultsItems: []
        };
    }

    componentDidMount = () => {
        this.getPlayers();
        console.log("got players: ", this.state.allPlayers);
    }
    // **********
    // GO GETTERS
    // **********
    getCash = () => {
        API.asset.getPlayerShares({ id: this.state.player.id, type: 3, qty: 0 }).then(cash => {
            this.setState({ cashShares: cash.data });
            this.setPlayerAndCash(cash.data[0]);
            this.getShares();
        })
    };
    getShares = () => {
        API.asset.getPlayerShares({ id: this.state.player.id, type: 1, qty: 0 }).then(shares => {
            this.setState({ shares: shares.data });
        })
    }
    getPlayers = () => {
        API.player.getPlayers().then(players => {
            let items = [];
            for (let player of players.data) {
                items.push({ body: [player.name, player.cash, player.shares, player.id] });
            }
            this.setState({
                allPlayers: items
            });
            console.log("got players' headers: ", this.state.resultsCollumns);
            console.log("got players: ", items);
        });
    }

    // ********
    // SERVICES
    // ********
    addCash = (id, qty) => {
        API.player.getPlayer(id).then(player => {
            this.setState({ cashShares: player.data.wallet });
        })
    };
    setPlayerAndCash = cash => {
        this.props.updateState({
            cash: cash,
            player: this.state.player
        });
    };

    // **************
    // EVENT HANDLING
    // **************
    handleSubmit = event => {
        if (this.state.playerName && this.state.playerCash) {
            API.player.createPlayer({
                Name: this.state.playerName,
                Cash: this.state.playerCash
            }).then(result => {
                this.setState({
                    submitMessage: "submitted successfully",
                    submitDisplay: true,
                    player: result.data
                })
                this.getPlayers();
                setTimeout(() => this.setState({ submitDisplay: false }), 3333);
                this.addCash(result.data.id, this.state.playerCash);
                this.getCash();
                this.setPlayerAndCash();
            });
        }
        else {
            this.setState({ submitMessage: "fill in the missing information", submitDisplay: true });
            setTimeout(() => this.setState({ submitDisplay: false }), 3333);
        }
    }
    handleChange = event => {
        event.preventDefault();
        const target = event.target;
        const eventName = target.name;
        const value = target.value;

        this.setState({
            [eventName]: value
        });
    }
    handleBlur = event => {
        event.preventDefault();
        const target = event.target;
        const eventName = target.name;
        const value = target.value;

        switch (eventName) {
            case "playerName":
                // validate name
                if (!this.state.playerName) {
                    this.setState({
                        playerNameDisplay: true
                    });
                }
                else {
                    this.setState({
                        playerNameDisplay: false
                    })
                }
                break;
            case "playerCash":
                // validate cash
                if (!this.state.playerCash) {
                    this.setState({
                        playerCashDisplay: true
                    });
                }
                else {
                    this.setState({
                        playerCashDisplay: false
                    });
                }
                break;
            default:
            // don't do anything
        }
    };


    render() {
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
                        onChange={this.handleChange}
                        name="playerName"
                        onBlur={this.handleBlur}
                        type="text"
                    />
                </InputGroup>
                <p className={this.state.playerNameDisplay ? 'show playerName-message' : 'hide playerName-message'}>{this.state.playerNameMessage}</p>

                <InputGroup className="mb-3">
                    <InputGroup.Prepend>
                        <InputGroup.Text id="playerCash">Player Cash: </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                        placeholder="Player Cash"
                        aria-label="PlayerCash"
                        aria-describedby="playerCash"
                        onChange={this.handleChange}
                        name="playerCash"
                        onBlur={this.handleBlur}
                        type="number"
                    />
                </InputGroup>
                <p className={this.state.playerCashDisplay ? 'show playerCash-message' : 'hide playerCash-message'}>{this.state.playerCashMessage}</p>

                <button id="submit" onClick={this.handleSubmit}>Submit</button>
                <p className={this.state.submitDisplay ? 'show submit-message' : 'hide submit-message'}>{this.state.submitMessage}</p>

                <h3>Players</h3>
                <SharesResults
                    columns={this.state.resultsCollumns}
                    items={this.state.allPlayers}
                />
            </div>
        );
    }
}

export default PlayerCreate;

//let theProps = {
//    columns: ["string"],
//    items: [{
//        body: ["string"]
//    }]
//}