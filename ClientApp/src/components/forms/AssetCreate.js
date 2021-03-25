import React, { useState } from 'react';
import InputGroup from 'react-bootstrap/InputGroup';
import Form from 'react-bootstrap/Form';
import API from './../../API';
import SharesResults from './ListResults';
import { useEffect } from 'react';

export const AssetCreate = props => {
    const [assetName, SETassetName] = useState("");
    const [shareCount, SETshareCount] = useState(0);
    const [cashCount, SETcashCount] = useState(0);
    const [playerCount, SETplayerCount] = useState(0);
    const [asset, SETasset] = useState({});
    const [cash, SETcash] = useState({});

    const [assetNameMessage, SETassetNameMessage] = useState("Required: All assets must have a name.");
    const [shareCountMessage, SETshareCountMessage] = useState("Required: How many shares of this asset are there?");
    const [playerCountMessage, SETplayerCountMessage] = useState("How many AI players?");
    const [cashCountMessage, SETcashCountMessage] = useState("How much cash for the House?");
    const [submitMessage, SETsubmitMessage] = useState("");

    const [assetNameDisplay, SETassetNameDisplay] = useState(false);
    const [shareCountDisplay, SETshareCountDisplay] = useState(false);
    const [playerCountDisplay, SETplayerCountDisplay] = useState(false);
    const [submitDisplay, SETsubmitDisplay] = useState(false);
    const [assetCreated, SETassetCreated] = useState(false);
    const [cashCreated, SETcashCreated] = useState(false);
    const [cashCountDisplay, SETcashCountDisplay] = useState(false);


    useEffect(() => {
        API.asset.getCash().then(cashReturn => {
            SETcash(cashReturn.data.asset);
        })
    },[]);

    // **********
    // GO GETTERS
    // **********
    const getShares = (id, cb) => API.asset.getShares(id).then(shares => cb(shares));

    // ********
    // SERVICES
    // ********
    const setGlobalAsset = (asst, csh) => props.updateState({ asst, csh });
    const createAsset = (asset, cb) => {
        API.asset.createAsset({
            "Name": assetName,
            "SharesOutstanding": shareCount
        }).then(result => cb(result));
    }

    // **************
    // EVENT HANDLING
    // **************
    const handleSubmit = event => {
        if (cashCount > 0) {
            API.asset.addShares({ id: cash.id, qty: cashCount }).then(result => {
                SETcashCreated(true);
            });
        }
        if (assetName && shareCount && !assetCreated) {
                    setTimeout(() => SETsubmitDisplay(false), 3333);
            createAsset(assetName, result => {
                if (result.status === 200) {
                    SETassetCreated(true);
                    SETsubmitMessage("submitted successfully");
                    SETsubmitDisplay(true);
                    SETasset(result.data);
                    setGlobalAsset(result.data,cash);
                }
                else {
                    SETassetCreated(false );
                    console.log("didn't get it");
                }
            })
        }
        else {
            SETsubmitMessage("fill in the missing information");
            SETsubmitDisplay(true);
            setTimeout(() => SETsubmitDisplay(false), 3333);
        }
    }

    const handleNameChange = event => {
        event.preventDefault();
        SETassetName(event.target.value);
    }
    const handleSharesChange = event => {
        event.preventDefault();
        SETshareCount(event.target.value);
    }
    const handlePlayersChange = event => {
        event.preventDefault();
        SETplayerCount(event.target.value);
    }
    const handleCashChange = event => {
        event.preventDefault();
        SETcashCount(event.target.value);
    }
    const handleBlur = event => {
        event.preventDefault();
        const target = event.target;
        const eventName = target.name;
        const value = target.value;

        switch (eventName) {
            case "assetName":
                // validate name
                if (!assetName) SETassetNameDisplay(true);
                else SETassetNameDisplay(false);
                break;
            case "shareCount":
                // make sure there's a shareCount
                if (!shareCount) SETshareCountDisplay(true);
                else SETshareCountDisplay(false);
                break;
            case "cashCount":
                // how much cash for the house?
                if (!cashCount) SETcashCountDisplay(true);
                else SETcashCountDisplay(false);
            case "playersCount":
                // how many AI players?
                if (!playerCount) SETplayerCountDisplay(true);
                else SETplayerCountDisplay(false);
            default:
            // don't do anything
        }
    };

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
                    onChange={handleNameChange}
                    name="assetName"
                    onBlur={handleBlur}
                    type="text"
                />
            </InputGroup>
            <p className={assetNameDisplay ? 'show assetName-message' : 'hide assetName-message'}>{assetNameMessage}</p>

            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="shareCount">Number of Shares: </InputGroup.Text>
                </InputGroup.Prepend>
                <Form.Control
                    placeholder="How many shares?"
                    aria-label="ShareCount"
                    aria-describedby="shareCount"
                    onChange={handleSharesChange}
                    name="shareCount"
                    onBlur={handleBlur}
                    type="number"
                />
            </InputGroup>
            <p className={shareCountDisplay ? 'show shareCount-message' : 'hide shareCount-message'}>{shareCountMessage}</p>

            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="playerCount">Number of Players: </InputGroup.Text>
                </InputGroup.Prepend>
                <Form.Control
                    placeholder="How many computer players?"
                    aria-label="playerCount"
                    aria-describedby="playerCount"
                    onChange={handlePlayersChange}
                    name="playerCount"
                    onBlur={handleBlur}
                    type="number"
                />
            </InputGroup>
            <p className={playerCountDisplay ? 'show shareCount-message' : 'hide shareCount-message'}>{playerCountMessage}</p>

            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="cashCount">House Cash: </InputGroup.Text>
                </InputGroup.Prepend>
                <Form.Control
                    placeholder="How much cash for the house?"
                    aria-label="cashCount"
                    aria-describedby="cashCount"
                    onChange={handleCashChange}
                    name="cashCount"
                    onBlur={handleBlur}
                    type="number"
                />
            </InputGroup>
            <p className={cashCountDisplay ? 'show shareCount-message' : 'hide shareCount-message'}>{cashCountMessage}</p>

            <button id="submit" onClick={handleSubmit}>Submit</button>
            <p className={submitDisplay ? 'show submit-message' : 'hide submit-message'}>{submitMessage}</p>

        </div>
    );
}

export default AssetCreate;