using MongoDB.Driver;
using RiskGame.API.Models;
using RiskGame.API.Persistence;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models.SharesFolder;
using AutoMapper;

namespace RiskGame.API.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IShareService _shareService;
        private readonly IMapper _mapper;
        private readonly IMongoCollection<PlayerResource> _players;
        private readonly PlayerResource HAUS;
        private readonly IDatabaseSettings dbSettings; // remove this when you remove Initialize
        public PlayerService(IDatabaseSettings settings, IShareService shareService, IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            database.DropCollection(settings.PlayerCollectionName);
            dbSettings = settings;
            _players = database.GetCollection<PlayerResource>(settings.PlayerCollectionName);
            _shareService = shareService;
            _mapper = mapper;

            HAUS = Create(new Player("HAUS",Guid.NewGuid()));
        }
        public string Initialize()
        {
            try
            {
                _players.Database.DropCollection(dbSettings.PlayerCollectionName);
                var cash = Create(_mapper.Map<PlayerResource, Player>(HAUS));
                return "Player Tabula Rasa";
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }

        }
        //
        // Get the HAUS player
        public Player GetHAUS()
        {
            return _mapper.Map<PlayerResource,Player>(HAUS);
        }
        public ModelReference GetHAUSRef()
        {
            return ResToRef(HAUS);
        }
        //
        // Gets a list of all players in the DB
        public async Task<IAsyncCursor<PlayerResource>> GetAsync() =>
            await _players.FindAsync(player => true);
        //
        // Gets the player attached to the given id
        public async Task<IAsyncCursor<PlayerResource>> GetAsync(Guid id)
        {
            var anything = await _players.FindAsync(player => player.PlayerId == id.ToString());
            return anything;
        }
        //
        // Creates a new player from the JSON
        public PlayerResource Create(Player player)
        {
            var newPlayer = _mapper.Map<Player, PlayerResource>(player);
            _players.InsertOne(newPlayer);
            return newPlayer;
        }
        //
        // Updates attributes of the Player
        // including updates to Cash and Portfolio
        public void Update(Guid id, PlayerResource playerIn)
        {
            var it = _players.ReplaceOne(player => player.PlayerId == id.ToString(), playerIn);
            var that = it;
        }
        //
        // Deletes the player in the db
        public void Remove(Player playerIn) =>
            _players.DeleteOne(player => player.PlayerId == playerIn.Id.ToString());
        //
        // Deletes the player in the db
        public void Remove(Guid id) =>
            _players.DeleteOne(player => player.PlayerId == id.ToString());
        public ModelReference ToRef(Player player) =>
            _mapper.Map<Player,ModelReference>(player);
        public ModelReference ResToRef(PlayerResource player) =>
            _mapper.Map<PlayerResource, ModelReference>(player);
    }
    public interface IPlayerService
    {
        string Initialize();
        Player GetHAUS();
        ModelReference GetHAUSRef();
        Task<IAsyncCursor<PlayerResource>> GetAsync();
        Task<IAsyncCursor<PlayerResource>> GetAsync(Guid id);
        PlayerResource Create(Player player);
        void Update(Guid id, PlayerResource playerIn);
        void Remove(Player playerIn);
        void Remove(Guid id);
        ModelReference ToRef(Player player);
        ModelReference ResToRef(PlayerResource player);
    }
}
