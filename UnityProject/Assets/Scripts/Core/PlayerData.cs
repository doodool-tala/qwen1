using UnityEngine;
using System;

namespace CoCGame.Core
{
    [Serializable]
    public class PlayerData
    {
        // Identity
        public string playerId;
        public string username;
        public int level;
        public long experience;
        
        // Trophies & League
        public int trophies;
        public int highestTrophies;
        public int leagueId;
        
        // Resources
        public long gold;
        public long elixir;
        public long darkElixir;
        public int gems;
        
        // Timers
        public DateTime lastLoginTime;
        public DateTime shieldEndTime;
        public DateTime guardEndTime;
        
        // Village
        public string villageLayoutJson;
        
        // Clan
        public string clanId;
        public string clanName;
        public int clanRole; // 0=None, 1=Member, 2=Elder, 3=Co-Leader, 4=Leader
        
        // Battle Stats
        public int battlesWon;
        public int battlesLost;
        public int attacksAvailable;
        public DateTime attackRefreshTime;
        
        public PlayerData()
        {
            playerId = Guid.NewGuid().ToString();
            username = "NewPlayer";
            level = 1;
            experience = 0;
            trophies = 0;
            highestTrophies = 0;
            leagueId = 0;
            gold = 500;
            elixir = 500;
            darkElixir = 0;
            gems = 50;
            lastLoginTime = DateTime.UtcNow;
            shieldEndTime = DateTime.MinValue;
            guardEndTime = DateTime.MinValue;
            villageLayoutJson = "";
            clanId = "";
            clanName = "";
            clanRole = 0;
            battlesWon = 0;
            battlesLost = 0;
            attacksAvailable = 3;
            attackRefreshTime = DateTime.UtcNow;
        }
    }
}
