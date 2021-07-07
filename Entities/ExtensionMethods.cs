using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.SharesFolder;

namespace RiskGame.API.Entities
{
    public static class ExtensionMethods
    {
        // Players
        public static ModelReference ToRef(this Player player) => new ModelReference(player.Name, player.PlayerId, player.ModelType);
        public static ModelReference ToRef(this PlayerResource player) => new ModelReference(player.Name, player.PlayerId, player.ModelType);
        public static ModelReference ToRef(this PlayerIn player) => new ModelReference(player.Name, player.PlayerId, ModelTypes.Player);
        // Assets
        public static ModelReference ToRef(this Asset asset) => new ModelReference(asset.Name, asset.AssetId, asset.ModelType);
        public static ModelReference ToRef(this AssetResource asset) => new ModelReference(asset.Name, asset.AssetId, asset.ModelType);
        public static ModelReference ToRef(this AssetIn asset) => new ModelReference(asset.Name, asset.Id, ModelTypes.Asset);
        // Shares
        public static ModelReference ToRef(this Share share) => new ModelReference(share.Name, share.Id, share.ModelType);
        public static ModelReference ToRef(this ShareResource share) => new ModelReference(share.Name, share.Id, share.ModelType);

    }
}
