import React, { useState, useEffect } from 'react';
import InputGroup from 'react-bootstrap/InputGroup';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import ButtonGroup from 'react-bootstrap/ToggleButtonGroup';
import ToggleButton from 'react-bootstrap/ToggleButton';
import API from './../../API';
import TradeTicket from './../../assets/TradeTicket';
import { ModelReference } from '../../assets/ModelReference';

export const Transaction = props => {

    const [player, SETplayer] = useState({});
    const [playerCash, SETplayerCash] = useState([]);
    const [playerShares, SETplayerShares] = useState([]);
    const [tradeShares, SETtradeShares] = useState(0);
    const [shareCost, SETshareCost] = useState(0);
    const [tradeCost, SETtradeCost] = useState(0);
    const [tradeSharesDisplay, SETtradeSharesDisplay] = useState(false);
    const [tradeCostDisplay, SETtradeCostDisplay] = useState(false);
    const [submitDisplay, SETsubmitDisplay] = useState(false);
    const [tradeTotalDisplay, SETtradeTotalDisplay] = useState(false);
    const [isBuySwitch, SETisBuySwitch] = useState(true);
    const [tradeSharesMessage, SETtradeSharesMessage] = useState("How many shares would you like to trade?");
    const [tradeCostMessage, SETtradeCostMessage] = useState("How much per share?");
    const [submitMessage, SETsubmitMessage] = useState("");

    useEffect(() => {
        SETplayer(getPlayer());
        SETplayerCash(getCash());
        SETplayerShares(getShares());
        console.log("used Effect");
    }, [
        props.retrieveState.player,
        props.retrieveState.asset,
    ]);

    const getPlayer = () => {
        if (props.retrieveState.player.id) {
            API.player.getPlayer(props.retrieveState.player.id)
                .then(player => SETplayer(player.data));
        }
        else {
            return null;
        }
    }
    const getShares = () => {
        if (props.retrieveState.player.id) {
            API.asset.getPlayerShares({id: props.retrieveState.player.id, type: 1})
                .then(shares => {
                    SETplayerShares(shares.data)
                });
        }
        else {
            return null;
        }
    }
    const getCash = () => {
        if (props.retrieveState.player.id) {
            API.asset.getPlayerShares({ id: props.retrieveState.player.id, type: 3 })
                .then(cash => {
                    SETplayerCash(cash.data)
                });
        
        }
        else {
            return null;
        }
    }

    const addCashFromWallet = () => {
        debugger;
        let cash = [];
        if (tradeCost * tradeShares > playerCash.length) return "Error: Not enough cash";
        for (let i = 0; i < shareCost * parseInt(tradeShares); i++) {
            cash.push(playerCash.pop());
        }
        return cash;
    }
    const addSharesFromPortfolio = () => {
        debugger;
        let shares = [];
        if (tradeShares > playerShares) return "Error: Not enough shares";
        for (let i = 0; i < tradeShares; i++) {
            shares.push(playerShares.pop());
        }
        return shares;
    }

    const handleSubmit = e => {
        e.preventDefault();
        let playerRef = <ModelReference
            name={player[0].name}
            id={player[0].id}
            type={player[0].type}
        />
        let tradeTicket = <TradeTicket
            buyer={isBuySwitch ? playerRef.props : null}
            seller={isBuySwitch ? null : playerRef.props}
            cash={isBuySwitch ? addCashFromWallet() : null}
            shares={isBuySwitch ? null : addSharesFromPortfolio()}
            cashCount={shareCost * tradeShares}
            sharesCount={parseInt(tradeShares)}
        />
        API.transactions.submitTrade(tradeTicket.props).then(outcome => {
            SETplayer(getPlayer());
            SETplayerShares(getShares());
            SETplayerCash(getCash());
            handleReset();
        });
    }
    const handleCashChange = event => {
        event.preventDefault();
        SETshareCost(event.target.value);
        if (tradeCost > 0)  SETtradeCostDisplay(true) 
        else  SETtradeCostDisplay(false);
    }
    const handleShareChange = event => {
        event.preventDefault();
        SETtradeShares(event.target.value);
        SETtradeCost(event.target.value * shareCost);
        if (shareCost > 0) SETtradeSharesDisplay(true)
        else SETtradeSharesDisplay(false);
    }
    const handleReset = () => {
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
        SETsubmitDisplay(true);
        SETtradeShares();
        SETshareCost();
        SETtradeCost();
        setTimeout(() => SETsubmitDisplay(false), 3333);
    };
    const handleBlur = event => {
        event.preventDefault();
        const target = event.target;
        const eventName = target.name;
        const value = target.value;
    };
    const handleBuySell = isBuy => {
        let switchNumber = `${isBuy}Switch`;
        this.setState({ [switchNumber]: isBuy });
    }
    const calculatedTotalCost = () => tradeShares && tradeCost ? tradeShares * tradeCost : "";
    const buySellToggle = e => {
        e.preventDefault();
        SETisBuySwitch(!isBuySwitch);
    }

    return (
        <div>
            <ButtonGroup toggle name="name" className="mb-2">
            <ToggleButton
                    type="checkbox"
                    variant={isBuySwitch ? "secondary" : "light"}
                    checked={isBuySwitch}
                    value="isBuy"
                    name="isBuy"
                    onClick={(e) => buySellToggle(e)}
            >
                {isBuySwitch ? "Buy" : "Sell"}
                </ToggleButton>
                </ButtonGroup>
            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="trade-shares">Shares: </InputGroup.Text>
                </InputGroup.Prepend>
                <Form.Control
                    placeholder="Number of shares"
                    aria-label="TradeShares"
                    aria-describedby="trade-shares"
                    onChange={event => handleShareChange(event)}
                    name="tradeShares"
                    onBlur={event => handleBlur(event)}
                />
            </InputGroup>
            <p className={tradeSharesDisplay ? 'show tradeShares-message' : 'hide tradeShares-message'}>{tradeSharesMessage}</p>

            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="share-cost">Price per share: </InputGroup.Text>
                </InputGroup.Prepend>
                <Form.Control
                    placeholder="Price Per Share"
                    aria-label="ShareCost"
                    aria-describedby="share-cost"
                    onChange={event => handleCashChange(event)}
                    name="shareCost"
                    onBlur={event => handleBlur(event)}
                />
            </InputGroup>
            <p className={tradeTotalDisplay ? 'show tradeTotal-message' : 'hide tradeTotal-message'}>{"Total cost of trade: " + calculatedTotalCost()}</p>
            <br />
            <p className={tradeCostDisplay ? 'show tradeCash-message' : 'hide tradeCash-message'}>{tradeCostMessage}</p>
            <br />
            <button id="submit" onClick={(e) => handleSubmit(e)}>Submit</button>
            <p className={submitDisplay ? 'show submit-message' : 'hide submit-message'}>{submitMessage}</p>

            <hr />

            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="portfolio">Portfolio: </InputGroup.Text>
                </InputGroup.Prepend>
                <div>{playerShares ? playerShares.length : 0}</div>
            </InputGroup>

            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="wallet">Wallet: </InputGroup.Text>
                </InputGroup.Prepend>
                <div>{playerCash ? playerCash.length : 0}</div>
            </InputGroup>
        </div>
    );
}

export default Transaction;