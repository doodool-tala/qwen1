using UnityEngine;

namespace ClashGame.Core
{
    /// <summary>
    /// Game configuration - can be loaded from ScriptableObject or JSON
    /// Contains all balance values and game settings
    /// </summary>
    [System.Serializable]
    public class GameConfig
    {
        [Header("Resource Settings")]
        public float goldMineProductionRate = 1.0f; // Gold per second at level 1
        public float elixirCollectorProductionRate = 1.0f; // Elixir per second at level 1
        public int maxGoldStorage = 5000;
        public int maxElixirStorage = 5000;
        public int maxDarkElixirStorage = 500;
        
        [Header("Upgrade Time Multipliers")]
        public float upgradeTimeMultiplier = 1.0f;
        public float trainingTimeMultiplier = 1.0f;
        
        [Header("Gem Conversion Rates")]
        public int gemsPerMinuteSkip = 1;
        public int goldPerGem = 100;
        public int elixirPerGem = 100;
        
        [Header("Battle Settings")]
        public int battleDurationSeconds = 180; // 3 minutes
        public float troopDeploymentWidth = 30f;
        public int maxTroopDeploymentCount = 200;
        
        [Header("Matchmaking")]
        public int trophySearchRangeMin = 200;
        public int trophySearchRangeMax = 600;
        public float matchmakingTimeoutSeconds = 30f;
        
        [Header("Shield System")]
        public int shieldDurationMinutes = 30;
        public int guardDurationMinutes = 15;
        
        [Header("League Thresholds")]
        public int[] leagueTrophyThresholds = new int[] { 400, 800, 1200, 1600, 2000, 2400, 2800, 3200, 3600, 4000, 4400, 4800, 5200, 5600, 6000, 6400 };
        
        [Header("Experience")]
        public int xpPerBuildingUpgrade = 5;
        public int xpPerTroopTraining = 1;
        public int xpPerBattleWin = 10;
        public int xpPerBattleLoss = 3;
        
        /// <summary>
        /// Calculate production rate based on building level
        /// Formula: baseRate * (1 + (level - 1) * 0.2)
        /// </summary>
        public float CalculateProductionRate(float baseRate, int level)
        {
            return baseRate * (1 + (level - 1) * 0.2f);
        }
        
        /// <summary>
        /// Calculate upgrade cost based on building type and level
        /// </summary>
        public long CalculateUpgradeCost(ResourceType resource, int baseCost, int level)
        {
            // Cost increases by 50% each level
            float multiplier = Mathf.Pow(1.5f, level - 1);
            return (long)(baseCost * multiplier);
        }
        
        /// <summary>
        /// Calculate upgrade time based on level
        /// </summary>
        public float CalculateUpgradeTime(float baseTime, int level)
        {
            // Time increases by 30% each level
            return baseTime * Mathf.Pow(1.3f, level - 1);
        }
        
        /// <summary>
        /// Calculate gems needed to skip a timer
        /// </summary>
        public int CalculateGemsForTimeSkip(float remainingMinutes)
        {
            return Mathf.Max(1, Mathf.FloorToInt(remainingMinutes / 60f * gemsPerMinuteSkip));
        }
        
        /// <summary>
        /// Get storage capacity based on building level
        /// </summary>
        public int GetStorageCapacity(int buildingLevel, ResourceType resource)
        {
            int baseCapacity = resource switch
            {
                ResourceType.Gold => 5000,
                ResourceType.Elixir => 5000,
                ResourceType.DarkElixir => 500,
                _ => 5000
            };
            
            // Capacity doubles every 2 levels
            return baseCapacity * Mathf.FloorToInt(Mathf.Pow(2, buildingLevel / 2f));
        }
    }
}
