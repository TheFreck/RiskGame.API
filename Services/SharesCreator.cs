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
        public List<Guid> Shares;
        public SharesCreator(ShareInputs inputs, IShareRepo shareRepo)
        {
            _asset = inputs.Asset;
            _qty = inputs.Qty/20;
            _owner = inputs.Owner;
            _type = inputs.ModelType;
            _shareRepo = shareRepo;
            Shares = new List<Guid>();
        }
        public List<Guid> CreateShares()
        {
            var threads = new List<Thread>();
            for(var i=0; i<20; i++)
            {
                threads.Add(new Thread(new ThreadStart(ShareCreation)));
                Console.WriteLine($"Starting thread {i}");
            }
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.ExecutionContext.Dispose());
            
            Console.WriteLine("Finishing threads");
            return Shares;
        }
        private void ShareCreation()
        {
            var listOut = new List<Guid>();
            for (var i = 0; i < _qty; i++)
            {
                var shareId = Guid.NewGuid();
                listOut.Add(shareId);
                _shareRepo.CreateOne(new ShareResource()
                {
                    _assetId = _asset.Id,
                    Name = $"Share of {_asset.Name}",
                    ShareId = shareId,
                    CurrentOwner = _owner,
                    ModelType = _type
                });
                Shares.Add(shareId);
            }
            Console.WriteLine($"finished creating {_qty} shares");
            
        }
    }
}
