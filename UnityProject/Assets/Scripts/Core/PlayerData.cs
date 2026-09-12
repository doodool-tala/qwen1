using System;

namespace ClashGame.Core
{
    /// <summary>
    /// Resource types available in the game
    /// </summary>
    public enum ResourceType
    {
        Gold,
        Elixir,
        DarkElixir,
        Gems
    }

    /// <summary>
    /// Player data model - stores all player-related information
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        // Basic Info
        public string playerId;
        public string username;
        public int level;
        public int trophies;
        
        // Resources
        public long gold;
        public long elixir;
        public long darkElixir;
        public int gems;
        
        // Progression
        public int townHallLevel;
        public int experiencePoints;
        public DateTime lastLoginTime;
        public DateTime shieldEndTime;
        
        // Army
        public int clanCastleLevel;
        public int barracksLevel;
        
        // Statistics
        public int battlesWon;
        public int battlesLost;
        public int totalResourcesCollected;
        public int totalResourcesStolen;
        
        public PlayerData()
        {
            playerId = Guid.NewGuid().ToString();
            username = "Player";
            level = 1;
            trophies = 0;
            gold = 500;
            elixir = 500;
            darkElixir = 0;
            gems = 50;
            townHallLevel = 1;
            experiencePoints = 0;
            lastLoginTime = DateTime.UtcNow;
            shieldEndTime = DateTime.MinValue;
            clanCastleLevel = 1;
            barracksLevel = 1;
            battlesWon = 0;
            battlesLost = 0;
            totalResourcesCollected = 0;
            totalResourcesStolen = 0;
        }
        
        /// <summary>
        /// Calculate player's league based on trophy count
        /// </summary>
        public string GetLeague()
        {
            if (trophies < 400) return "Bronze III";
            if (trophies < 800) return "Bronze II";
            if (trophies < 1200) return "Bronze I";
            if (trophies < 1600) return "Silver III";
            if (trophies < 2000) return "Silver II";
            if (trophies < 2400) return "Silver I";
            if (trophies < 2800) return "Gold III";
            if (trophies < 3200) return "Gold II";
            if (trophies < 3600) return "Gold I";
            if (trophies < 4000) return "Crystal III";
            if (trophies < 4400) return "Crystal II";
            if (trophies < 4800) return "Crystal I";
            if (trophies < 5200) return "Master III";
            if (trophies < 5600) return "Master II";
            if (trophies < 6000) return "Master I";
            if (trophies < 6400) return "Champion";
            return "Legend";
        }
        
        /// <summary>
        /// Check if player has an active shield
        /// </summary>
        public bool HasActiveShield()
        {
            return DateTime.UtcNow < shieldEndTime;
        }
        
        /// <summary>
        /// Add experience points and handle level up
        /// </summary>
        public void AddExperience(int amount)
        {
            experiencePoints += amount;
            
            // Simple leveling formula: level up every 1000 XP * current level
            int xpNeededForNextLevel = 1000 * level;
            
            while (experiencePoints >= xpNeededForNextLevel)
            {
                experiencePoints -= xpNeededForNextLevel;
                level++;
                UnityEngine.Debug.Log($"Level up! New level: {level}");
                xpNeededForNextLevel = 1000 * level;
            }
        }
    }
}
