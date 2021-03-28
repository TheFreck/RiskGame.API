using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Engine
{
    public class Agenda
    {
        public bool isOn { get; set; }
        public bool isRunning { get; set; }
        public CompanyAsset[] Assets;
        public List<EconMetrics> Economies;
        public Agenda()
        {
            Economies = new List<EconMetrics> {
                new EconMetrics
                {
                    Red = 1,
                    Orange = 1,
                    Yellow = 1,
                    Green = 1,
                    Blue = 1,
                    Violet = 1,
                    RedDirection = Direction.Up,
                    OrangeDirection = Direction.Up,
                    YellowDirection = Direction.Up,
                    GreenDirection = Direction.Up,
                    BlueDirection = Direction.Up,
                    VioletDirection = Direction.Up
                }
            };
        }

        // generate economic metrics
        // grow Company Assets
        // process players' turns
    }
}
