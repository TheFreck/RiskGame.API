import React, { Component } from 'react';
import InputGroup from 'react-bootstrap/InputGroup';
import Form from 'react-bootstrap/Form';
import API from './../../API';

export class Transaction extends Component {
    static displayName = Transaction.name;

    constructor(props) {
        super(props);
        this.state = {
            player: {},

            tradeShares: 0,
            tradeCost: 0,
            totalCost: 0,

            playerCash: [],
            playerShares: [],
            
            tradeSharesDisplay: false,
            tradeCostDisplay: false,
            submitDisplay: false,

            tradeSharesMessage: "How many shares would you like to trade?",
            tradeCostMessage: "How much per share are you willing to go?",
            submitMessage: ""
        };
    }
    componentDidMount = () => {
        if (this.props.retrieveState.player !== null) {
            API.player.getPlayer(this.props.retrieveState.player.id).then(player => {
                this.setState({ player: player });

            });
        }
    }

    handleSubmit = event => {
        if (this.state.assetName && this.state.shareCount) {
            API.asset.createAsset({
                "Name": this.state.assetName,
                "SharesOutstanding": this.state.shareCount,
                "RateOfReturn": this.state.assetIncome
            }).then(result => {
                console.log("result: ", result);
                console.log("id: ", result.data.id);
                this.setState({ submitMessage: "submitted successfully", submitDisplay: true, asset: result.data })
                setTimeout(() => this.setState({ submitDisplay: false }), 3333);
                this.getShares(result.data.id);
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
            case "assetName":
                // validate name
                if (!this.state.assetName) {
                    this.setState({
                        assetNameDisplay: true
                    });
                }
                else {
                    this.setState({
                        assetNameDisplay: false
                    })
                }
                break;
            case "assetIncome":
                // if no incom ask if they wanna add income
                if (!this.state.assetIncome) {
                    this.setState({
                        assetIncomeDisplay: true
                    });
                }
                else {
                    this.setState({
                        assetIncomeDisplay: false
                    });
                }
                break;
            case "shareCount":
                // make sure there's a shareCount
                if (!this.state.shareCount) {
                    this.setState({
                        shareCountDisplay: true
                    });
                }
                else {
                    this.setState({
                        shareCountDisplay: false
                    });
                }
                break;
            default:
            // don't do anything
        }
    };
    getShares = id => {
        console.log("get shares: ", id);
        console.log("the asset: ", this.state.asset);
        API.asset.getShares(this.state.asset.id).then(shares => {
            console.log("shares: ", shares);
            console.log("shares data: ", shares.data);
            this.setState({ shares: shares.data });
        })
    };
    totalCost = () => this.state.tradeShares * this.state.tradeCost;

    render() {
        return (
            <div>
                <InputGroup className="mb-3">
                    <InputGroup.Prepend>
                        <InputGroup.Text id="trade-shares">Shares: </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                        placeholder="Number of shares"
                        aria-label="TradeShares"
                        aria-describedby="trade-shares"
                        onChange={this.handleChange}
                        name="tradeShares"
                        onBlur={this.handleBlur}
                    />
                </InputGroup>
                <p className={this.state.tradeSharesDisplay ? 'show tradeShares-message' : 'hide tradeShares-message'}>{this.state.tradeSharesMessage}</p>

                <InputGroup className="mb-3">
                    <InputGroup.Prepend>
                        <InputGroup.Text id="trade-cash">Cash: </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                        placeholder="Price Per Share"
                        aria-label="TradeCash"
                        aria-describedby="trade-cash"
                        onChange={this.handleChange}
                        name="tradeCash"
                        onBlur={this.handleBlur}
                    />
                </InputGroup>
                <p className={this.state.tradeCashDisplay ? 'show tradeCash-message' : 'hide tradeCash-message'}>{this.state.tradeCashMessage}</p>

                <button id="submit" onClick={this.handleSubmit}>Submit</button>
                <p className={this.state.submitDisplay ? 'show submit-message' : 'hide submit-message'}>{this.state.submitMessage}</p>

                <hr />

                <InputGroup className="mb-3">
                    <InputGroup.Prepend>
                        <InputGroup.Text id="assetIncome">Cash: </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                        placeholder="Asset Income"
                        aria-label="AssetIncome"
                        aria-describedby="assetIncome"
                        onChange={this.handleChange}
                        name="assetIncome"
                        onBlur={this.handleBlur}
                    />
                </InputGroup>
                <p className={this.state.assetIncomeDisplay ? 'show assetIncome-message' : 'hide assetIncome-message'}>{this.state.assetIncomeMessage}</p>

                <InputGroup className="mb-3">
                    <InputGroup.Prepend>
                        <InputGroup.Text id="assetIncome">Cash: </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                        placeholder="Asset Income"
                        aria-label="AssetIncome"
                        aria-describedby="assetIncome"
                        onChange={this.handleChange}
                        name="assetIncome"
                        onBlur={this.handleBlur}
                    />
                </InputGroup>
                <p className={this.state.assetIncomeDisplay ? 'show assetIncome-message' : 'hide assetIncome-message'}>{this.state.assetIncomeMessage}</p>
                
            </div>
        );
    }
}

export default Transaction;