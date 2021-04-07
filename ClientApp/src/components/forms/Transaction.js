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

    const [player, SETplayer] = useState({ player: {} });
    const [playerCash, SETplayerCash] = useState({ playerCash: [] }); // The cash from the player's wallet used to make the trade
    const [playerShares, SETplayerShares] = useState({ playerShares: [] }); // The shares from the player's portfolio used to make the trade
    const [tradeShares, SETtradeShares] = useState({ tradeShares: 0 }); // The number of shares being traded
    const [shareCost, SETshareCost] = useState({ shareCost: 0 }); // The cost per share
    const [tradeCost, SETtradeCost] = useState({ tradeCost: 0 }); // The total cost of the trade
    const [tradeSharesDisplay, SETtradeSharesDisplay] = useState(false);
    const [tradeCostDisplay, SETtradeCostDisplay] = useState(false);
    const [submitDisplay, SETsubmitDisplay] = useState(false);
    const [isBuySwitch, SETisBuySwitch] = useState(true);
    const [tradeSharesMessage, SETtradeSharesMessage] = useState("How many shares would you like to trade?");
    const [submitMessage, SETsubmitMessage] = useState("");

    const modelTypes = {
        Asset: 0,
        Share: 1,
        Player: 2,
        Cash: 3
    }

    useEffect(() => {
        getPlayer();
    }, [
        props.retrieveState.player,
        props.retrieveState.asset,
    ]);

    // **********
    // GO GETTERS
    // **********
    const getPlayer = () => {
        if (props.retrieveState.player.id) {
            API.player.getPlayer(props.retrieveState.player.id)
                .then(player => {
                    console.log("get player: player: ", player);
                    SETplayer({ player: player.data[0] })
                });
        }
        else {
            return null;
        }
    }
    const getShares = cb => {
        API.asset.getPlayerShares({
            id: props.retrieveState.player.id,
            type: modelTypes.Share,
            qty: tradeShares
        }).then(shares => {
            SETplayerShares({ playerShares: shares.data })
            cb(shares.data);
        });
    }
    const getCash = cb => {
        console.log("transaction get cash");
        API.asset.getPlayerShares({
            id: props.retrieveState.player.id,
            type: modelTypes.Cash,
            qty: tradeCost.tradeCost
        }).then(cash => {
            console.log("transaction got cash: ", cash);
            SETplayerCash({ playerCash: cash.data });
            cb(cash.data);
        });
    }

    // ********
    // SERVICES
    // ********
    const addCashFromWallet = cb => {
        getCash(cash => {
            console.log("add cash from wallet: ", cash);
            if (tradeCost.tradeCost * tradeShares.tradeShares > cash.length) return "Error: Not enough cash";
            for (let i = 0; i < shareCost.shareCost * tradeShares.tradeShares; i++) {
                cash.push(cash.pop());
            }
            cb(cash);
        });
    }
    const addSharesFromPortfolio = (cb) => {
        getShares(shares => {
            console.log("add shares from portfolio: ", shares);
            if (tradeShares > shares.length) return "Error: Not enough shares";
            for (let i = 0; i < tradeShares; i++) {
                shares.push(shares.pop());
            }
            SETplayerShares(shares);
            cb(shares);
        });
    }
    const calculatedTotalCost = () => tradeShares.tradeShares && tradeCost.tradeCost ? tradeShares.tradeShares * tradeCost.tradeCost : "";
    const getCashOrShares = cb => {
        if (isBuySwitch) addCashFromWallet(cash => {
            cb(cash);
        })
        else addSharesFromPortfolio(shares => {
            cb(shares);
        })
    }
    const updateTradeCost = () => SETtradeCost({ tradeCost: tradeShares.tradeShares * shareCost.shareCost });

    // ********
    // SWITCHES
    // ********
    const buySellToggle = e => {
        e.preventDefault();
        SETisBuySwitch(!isBuySwitch);
    }

    // **************
    // EVENT HANDLING
    // **************
    const handleSubmit = e => {
        e.preventDefault();
        console.log("handling submit: player: ", player);
        let playerRef = <ModelReference
            name={player.player.name}
            id={player.player.id}
            type={modelTypes.Player}
        />
        // get cash
        getCashOrShares(items => {
            let tradeTicket = <TradeTicket
                buyer={isBuySwitch ? playerRef.props : null}
                seller={isBuySwitch ? null : playerRef.props}
                cash={isBuySwitch ? items : null}
                shares={isBuySwitch ? null : items}
                cashCount={tradeCost.tradeCost * tradeShares.tradeShares}
                sharesCount={parseInt(tradeShares.tradeShares)}
            />
            console.log("tradeTicket.props: ", tradeTicket.props);
            API.transactions.submitTrade(tradeTicket.props).then(outcome => {
                getPlayer();
                SETplayerShares({ playerShares: getShares() });
                SETplayerCash({ playerCash: getCash() });
                SETsubmitMessage("Trade Submitted");
                SETsubmitDisplay(true);
                handleReset();
                setTimeout(SETsubmitDisplay(false), 3333);
            });
        })

    }
    const handleCashChange = event => {
        event.preventDefault();
        console.log("event target value: ", parseInt(event.target.value));
        console.log("shareCost: ", shareCost.shareCost);
        console.log("cash::SETtradeCost", parseInt(event.target.value) * shareCost.shareCost);
        SETshareCost({ shareCost: parseInt(event.target.value) });
        if (tradeCost.tradeCost > 0) SETtradeCostDisplay(true)
        else SETtradeCostDisplay(false);
    }
    const handleShareChange = event => {
        event.preventDefault();
        console.log(parseInt(event.target.value));
        console.log("shares::SETtradeCost", parseInt(event.target.value) * shareCost.shareCost);
        SETtradeShares({ tradeShares: parseInt(event.target.value) });
        if (shareCost.shareCost > 0) SETtradeSharesDisplay(true)
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
        console.log("unblur: ", shareCost.shareCost * tradeShares.tradeShares);
        updateTradeCost();
    };

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
            <p className={tradeCostDisplay ? 'show tradeTotal-message' : 'hide tradeTotal-message'}>{"Total cost of trade: " + calculatedTotalCost()}</p>
            <br />
            <button id="submit" onClick={(e) => handleSubmit(e)}>Submit</button>
            <p className={submitDisplay ? 'show submit-message' : 'hide submit-message'}>{submitMessage}</p>

            <hr />

            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="portfolio">Portfolio: </InputGroup.Text>
                </InputGroup.Prepend>
                <div>{playerShares.playerShares ? playerShares.playerShares.length : 0}</div>
            </InputGroup>

            <InputGroup className="mb-3">
                <InputGroup.Prepend>
                    <InputGroup.Text id="wallet">Wallet: </InputGroup.Text>
                </InputGroup.Prepend>
                {/*<div>{playerCash ? playerCash.playerCash.length : 0}</div>*/}
            </InputGroup>
        </div>
    );
}

export default Transaction;