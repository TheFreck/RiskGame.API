import React, { useState } from 'react';
import InputGroup from 'react-bootstrap/InputGroup';
import Form from 'react-bootstrap/Form';
import API from './../../API';
import ListResults from './ListResults';
import { useEffect } from 'react';

export const AssetCreate = props => {
    const [gameId, SETgameId] = useState({ gameId: props.gameId });
    const [assetName, SETassetName] = useState("asset");
    const [shareCount, SETshareCount] = useState(100);
    const [cashCount, SETcashCount] = useState({ cashCount: 0 });
    const [playerCount, SETplayerCount] = useState({ player: 0 });
    const [asset, SETasset] = useState({ asset: {}});
    const [cash, SETcash] = useState({ cash: {} });
    const [resultsData, SETresultsData] = useState([]);

    const [assetNameMessage, SETassetNameMessage] = useState("");
    const [shareCountMessage, SETshareCountMessage] = useState("");
    const [playerCountMessage, SETplayerCountMessage] = useState("");
    const [cashCountMessage, SETcashCountMessage] = useState("");
    const [submitMessage, SETsubmitMessage] = useState("");

    const [assetNameDisplay, SETassetNameDisplay] = useState(false);
    const [shareCountDisplay, SETshareCountDisplay] = useState(false);
    const [playerCountDisplay, SETplayerCountDisplay] = useState(false);
    const [submitDisplay, SETsubmitDisplay] = useState(false);
    const [assetCreated, SETassetCreated] = useState(false);
    const [cashCreated, SETcashCreated] = useState(false);
    const [cashCountDisplay, SETcashCountDisplay] = useState(false);

    const modelTypes = {
        Asset: 0,
        Share: 1,
        Player: 2,
        Cash: 3
    }

    useEffect(() => {
        if (props.gameId !== "" && props.gameId !== undefined && props.gameId !== null) SETgameId(props.gameId);
    },[props.gameId]);

    // **********
    // GO GETTERS
    // **********
    const getShares = (id, cb) => API.asset.getShares(id).then(shares => cb(shares));

    // **********
    // GO SETTERS
    // **********
    const submitAsset = asset => API.asset.createAsset(asset).then(result => console.log(result));

    // ********
    // SERVICES
    // ********
    const setGlobalAsset = assets => {
        props.state.assets[1]({ assets: assets });
    }
    const createAsset = (asset, cb) => API.asset.createAsset(asset).then(data => cb(data));
    const ListResultsComponent = () => {
        if (resultsData && resultsData.length > 0) {
            return <ListResults
                columns={resultsColumns}
                items={resultsData}
                tableName="assets-table"
                style={{
                    background: "rgb(252, 252, 252)"
                }}
            />;
        }
        else {
            return "";
        }
    }

    // **************
    // EVENT HANDLING
    // **************
    const handleSubmit = event => {
        if (assetName && shareCount) {
            createAsset({ Name: assetName, SharesOutstanding: parseInt(shareCount), GameId: gameId }, result => {
                if (result.status === 200) {
                    SETassetCreated(true);
                    //SETsubmitMessage("submitted successfully");
                    //SETsubmitDisplay(true);
                    SETasset(result.data);
                    let newResults = resultsData;
                    newResults.push({
                        body: [result.data.name, result.data.sharesOutstanding, result.data.id]
                    });
                    SETresultsData(newResults);
                    setGlobalAsset(newResults);
                    SETassetName("");
                }
                else {
                    SETassetCreated(false );
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
    const handleFinished = event => {
        props.playerButtonClick(gameId);
    }

    const resultsColumns = ["Name", "Shares Outstanding", "Asset Id"];

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

            <button id="submit" onClick={handleSubmit}>Submit Asset</button>
            <p className={submitDisplay ? 'show submit-message' : 'hide submit-message'}>{submitMessage}</p>

            <button id="assets-finshed" onClick={handleFinished}>Finished Adding Assets</button>

            <ListResultsComponent />
            
        </div>
    );
}

export default AssetCreate;
