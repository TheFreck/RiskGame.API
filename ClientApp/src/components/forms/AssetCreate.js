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
            assetIncome: 0,
            shareCount: 0,

            assetNameMessage: "Required: All assets must have a name.",
            assetIncomeMessage: "Optional: Would you like this asset to pay income?",
            shareCountMessage: "Required: How many shares of this asset are there?",
            submitMessage: "",

            assetNameDisplay: false,
            assetIncomeDisplay: false,
            shareCountDisplay: false,
            submitDisplay: false,

            asset: {},
            shares: []
        };
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
                    />
                </InputGroup>
                <p className={this.state.assetNameDisplay ? 'show assetName-message' : 'hide assetName-message'}>{this.state.assetNameMessage}</p>

                <InputGroup className="mb-3">
                    <InputGroup.Prepend>
                        <InputGroup.Text id="assetIncome">Asset Income: </InputGroup.Text>
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
                        <InputGroup.Text id="shareCount">Number of Shares: </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                        placeholder="How many shares?"
                        aria-label="ShareCount"
                        aria-describedby="shareCount"
                        onChange={this.handleChange}
                        name="shareCount"
                        onBlur={this.handleBlur}
                    />
                </InputGroup>
                <p className={this.state.shareCountDisplay ? 'show shareCount-message' : 'hide shareCount-message'}>{this.state.shareCountMessage}</p>

                <button id="submit" onClick={this.handleSubmit}>Submit</button>
                <p className={this.state.submitDisplay ? 'show submit-message' : 'hide submit-message'}>{this.state.submitMessage}</p>

                <div className="asset-create-header">
                    <h2>Asset Name: <span><h3>{this.state.asset.name}</h3></span></h2>
                    <h2>Income: <span><h3>{this.state.asset.rateOfReturn}</h3></span></h2>
                    <h2>Shares Outstanding: <span><h3>{this.state.asset.sharesOutstanding}</h3></span></h2>
                </div>
                <div>
                    <SharesResults
                        shares={this.state.shares}
                    />
                </div>
            </div>
        );
    }
}

export default AssetCreate;