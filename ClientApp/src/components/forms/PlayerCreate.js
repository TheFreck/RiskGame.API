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
            cashShares: []
        };
    }

    handleSubmit = event => {
        if (this.state.playerName && this.state.shareCount) {
            API.player.createPlayer({
                "Name": this.state.playerName,
                "Cash": this.state.playerCash
            }).then(result => {
                console.log("result: ", result);
                console.log("id: ", result.data.id);
                this.setState({ submitMessage: "submitted successfully", submitDisplay: true, player: result.data })
                setTimeout(() => this.setState({ submitDisplay: false }), 3333);
                this.addCash(result.data.id, this.state.playerCash);
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
    handleReset = () => {
        //Array.from(document.querySelectorAll("input")).forEach(
        //    input => {
        //        (input.value = "")
        //    }
        //);
        //Array.from(document.querySelectorAll("textarea")).forEach(
        //    textarea => {
        //        (textarea.value = "")
        //    }
        //);
        //this.setState({
        //    itemvalues: [{}],
        //    SubmitDisplay: true
        //});
        //setTimeout(() => this.setState({ SubmitDisplay: false }), 3333);
    };
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
    addCash = (id, qty) => {
        console.log("get shares: ", id);
        console.log("the player: ", this.state.player);
        API.asset.addShares({ id, qty }).then(cash => {
            console.log("cash: ", cash);
            console.log("cash data: ", cash.data);
            this.setState({ cashShares: cash.data });
        })
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
                    />
                </InputGroup>
                <p className={this.state.playerCashDisplay ? 'show playerCash-message' : 'hide playerCash-message'}>{this.state.playerCashMessage}</p>

                <button id="submit" onClick={this.handleSubmit}>Submit</button>
                <p className={this.state.submitDisplay ? 'show submit-message' : 'hide submit-message'}>{this.state.submitMessage}</p>

                <div className="player-create-header">
                    <h2>Name: <span><h3>{this.state.player.name}</h3></span></h2>
                    <h2>Cash: <span><h3>{this.state.player.cash}</h3></span></h2>
                </div>
            </div>
        );
    }
}

export default PlayerCreate;