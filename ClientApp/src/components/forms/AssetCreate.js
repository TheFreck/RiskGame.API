import React, { Component } from 'react';
import InputGroup from 'react-bootstrap/InputGroup';
import Form from 'react-bootstrap/Form';
import API from './../../API';
import SharesResults from './SharesResults';

export class AssetCreate extends Component {
    static displayName = AssetCreate.name;

    constructor(props) {
        super(props);
        this.state = {
            assetName: "",
            shareCount: 0,
            cashCount: 0,

            assetNameMessage: "Required: All assets must have a name.",
            assetIncomeMessage: "Optional: Would you like this asset to pay income?",
            shareCountMessage: "Required: How many shares of this asset are there?",
            submitMessage: "",

            assetNameDisplay: false,
            assetIncomeDisplay: false,
            shareCountDisplay: false,
            submitDisplay: false,
            assetCreated: false,
            cashCreated: false,

            asset: {},
            cash: {},
            assetShares: [],
            cashShares: [],

        };
    }

    componentDidMount = () => {
        this.getShares(null, gotShares => {
            this.setState({ cash: gotShares.data.asset });
        });
    }

    handleSubmit = event => {
        if (this.state.cashCount > 0) {
            console.log("submit cash Id: ", this.state.cash.id);
            console.log("submit cash count: ", this.state.cashCount);
            API.asset.addShares(this.state.cash.id, this.state.cashCount).then(result => {
                console.log("house cash: ", result.data.shares.length)
            });
        }
        if (this.state.assetName && this.state.shareCount && !this.state.assetCreated) {
                    setTimeout(() => this.setState({ submitDisplay: false }), 3333);
            this.createAsset(this.state.assetName, result => {
                console.log("result: ", result);
                if (result.status === 200) {
                    this.setState({ assetCreated: true });
                    this.setState({ submitMessage: "submitted successfully", submitDisplay: true, asset: result.data })
                    this.getShares(result.data.id, sharesResults => {
                        console.log("shares: ", sharesResults.data);
                    });
                }
                else {
                    this.setState({ assetCreated: false });
                    console.log("didn't get it");
                }
            })
        }
        else {
            this.setState({ submitMessage: "fill in the missing information", submitDisplay: true });
            setTimeout(() => this.setState({ submitDisplay: false }), 3333);
        }
    }
    createAsset = (asset, cb) => {
        API.asset.createAsset({
            "Name": this.state.assetName,
            "SharesOutstanding": this.state.shareCount
        }).then(result => cb(result));
    }
    setGlobalAsset = () => this.props.updateState({ asset: this.state.asset });

    handleChange = event => {
        event.preventDefault();
        const target = event.target;
        const eventName = target.name;
        const value = target.value;
        console.log(eventName, value);
        this.setState({
            [eventName]: value
        });
    }
    handleReset = () => {
        Array.from(document.querySelectorAll("input")).forEach(
            input => {
                (input.value = "")
            }
        );
        Array.from(document.querySelectorAll("textarea")).forEach(
            textarea => {
                (textarea.value = "")
            }
        );
        this.setState({
            itemvalues: [{}],
            SubmitDisplay: true
        });
        setTimeout(() => this.setState({ SubmitDisplay: false }), 3333);
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
                case "cashCount":
                if (!this.state.cashCount) {
                    this.setState({
                        cashCountDisplay: true
                    });
                }
                else {
                    this.setState({
                        cashCountDisplay: false
                    });
                }
            default:
            // don't do anything
        }
    };
    getShares = (id, cb) => API.asset.getShares(id).then(shares => cb(shares));

    render() {
        return (
            <div>
                <InputGroup className="mb-3">
                    <InputGroup.Prepend>
                        <InputGroup.Text id="assetName">Asset Name: </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                        placeholder="Asset Name"
                        aria-label="AssetName"
                        aria-describedby="assetName"
                        onChange={this.handleChange}
                        name="assetName"
                        onBlur={this.handleBlur}
                        type="text"
                    />
                </InputGroup>
                <p className={this.state.assetNameDisplay ? 'show assetName-message' : 'hide assetName-message'}>{this.state.assetNameMessage}</p>

                <InputGroup className="mb-3">
                    <InputGroup.Prepend>
                        <InputGroup.Text id="shareCount">Number of Shares: </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                        placeholder="How many shares?"
                        aria-label="ShareCount"
                        aria-describedby="shareCount"
                        onChange={this.handleChange}
                        name="shareCount"
                        onBlur={this.handleBlur}
                        type="number"
                    />
                </InputGroup>
                <p className={this.state.shareCountDisplay ? 'show shareCount-message' : 'hide shareCount-message'}>{this.state.shareCountMessage}</p>

                <InputGroup className="mb-3">
                    <InputGroup.Prepend>
                        <InputGroup.Text id="cashCount">House Cash: </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                        placeholder="How much cash for the house?"
                        aria-label="cashCount"
                        aria-describedby="cashCount"
                        onChange={this.handleChange}
                        name="cashCount"
                        onBlur={this.handleBlur}
                        type="number"
                    />
                </InputGroup>
                <p className={this.state.shareCountDisplay ? 'show shareCount-message' : 'hide shareCount-message'}>{this.state.shareCountMessage}</p>

                <button id="submit" onClick={this.handleSubmit}>Submit</button>
                <p className={this.state.submitDisplay ? 'show submit-message' : 'hide submit-message'}>{this.state.submitMessage}</p>

            </div>
        );
    }
}

export default AssetCreate;