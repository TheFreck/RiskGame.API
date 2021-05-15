using RiskGame.API.Entities.Enums;
using RiskGame.API.Models;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Persistence.Repositories;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RiskGame.API.Services
{
    public class SharesCreator
    {
        private readonly IShareRepo _shareRepo;
        private readonly ModelReference _asset;
        private readonly int _qty;
        private readonly ModelReference _owner;
        private readonly ModelTypes _type;
        private readonly int _threadCount;
        private readonly int _perThreadCount;
        private readonly int _lastThread;
        private readonly Guid _gameId;
        public List<Guid> Shares;
        public SharesCreator(ShareInputs inputs, IShareRepo shareRepo)
        {
            _gameId = inputs.GameId;
            _asset = inputs.Asset;
            _qty = inputs.Qty;
            _owner = inputs.Owner;
            _type = inputs.ModelType;
            _shareRepo = shareRepo;
            _threadCount = (int)Math.Sqrt(_qty);
            _perThreadCount = _qty / _threadCount;
            _lastThread = _qty % _perThreadCount;
            Shares = new List<Guid>();
        }
        public List<Guid> CreateShares()
        {
            // MEASURE THE TIME EACH THREAD RUNS TO DETERMINE HOW MANY THREADS TO CREATE
            var threads = new List<Thread>();
            for(var i=0; i<_threadCount; i++)
            {
                threads.Add(new Thread(new ThreadStart(ShareCreation)));
            }
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.ExecutionContext.Dispose());
            // last thread to handle the remainder
            for (var i = 0; i < _lastThread; i++)
            {
                var shareId = Guid.NewGuid();
                _shareRepo.CreateOne(new ShareResource()
                {
                    _assetId = _asset.Id,
                    GameId = _gameId,
                    Name = $"Share of {_asset.Name}",
                    ShareId = shareId,
                    CurrentOwner = _owner,
                    ModelType = _type
                });
                Shares.Add(shareId);
            }
            return Shares;
        }
        private void ShareCreation()
        {
            var listOut = new List<Guid>();
            for (var i = 0; i < _perThreadCount; i++)
            {
                var shareId = Guid.NewGuid();
                listOut.Add(shareId);
                _shareRepo.CreateOne(new ShareResource()
                {
                    _assetId = _asset.Id,
                    GameId = _gameId,
                    Name = $"Share of {_asset.Name}",
                    ShareId = shareId,
                    CurrentOwner = _owner,
                    ModelType = _type
                });
                Shares.Add(shareId);
            }
            Console.WriteLine($"finished creating {_perThreadCount} shares");
        }
    }
}
