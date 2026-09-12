using UnityEngine;
using System.Collections.Generic;

namespace CoCGame.Core
{
    /// <summary>
    /// Singleton pattern for GameManager - controls overall game state and flow
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public GameState currentState = GameState.MainMenu;
        
        [Header("Player Data")]
        public PlayerData playerData;
        
        [Header("Configuration")]
        public GameConfig gameConfig;

        public enum GameState
        {
            MainMenu,
            Loading,
            Village,
            Battle,
            Shop,
            Settings
        }

        private void Awake()
        {
            // Ensure singleton instance
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        /// <summary>
        /// Initialize the game - load player data, setup systems
        /// </summary>
        public void InitializeGame()
        {
            Debug.Log("Initializing game...");
            
            // Load or create player data
            playerData = LoadPlayerData();
            
            // Load game configuration
            gameConfig = new GameConfig();
            gameConfig.Initialize();
            
            // Set initial state
            ChangeState(GameState.Village);
            
            Debug.Log("Game initialized successfully!");
        }

        /// <summary>
        /// Change the current game state
        /// </summary>
        public void ChangeState(GameState newState)
        {
            GameState previousState = currentState;
            currentState = newState;
            
            Debug.Log($"Game state changed from {previousState} to {newState}");
            
            // Handle state-specific logic
            switch (newState)
            {
                case GameState.MainMenu:
                    OnEnterMainMenu();
                    break;
                case GameState.Village:
                    OnEnterVillage();
                    break;
                case GameState.Battle:
                    OnEnterBattle();
                    break;
                case GameState.Shop:
                    OnEnterShop();
                    break;
                case GameState.Settings:
                    OnEnterSettings();
                    break;
            }
        }

        #region State Handlers
        
        private void OnEnterMainMenu()
        {
            // Show main menu UI
        }

        private void OnEnterVillage()
        {
            // Load village scene and show village UI
            Debug.Log("Entering Village mode");
        }

        private void OnEnterBattle()
        {
            // Load battle scene and start battle
            Debug.Log("Entering Battle mode");
        }

        private void OnEnterShop()
        {
            // Show shop UI
            Debug.Log("Entering Shop mode");
        }

        private void OnEnterSettings()
        {
            // Show settings UI
            Debug.Log("Entering Settings mode");
        }

        #endregion

        #region Data Management

        /// <summary>
        /// Load player data from storage or create new
        /// </summary>
        private PlayerData LoadPlayerData()
        {
            // TODO: Implement actual save/load system with PlayerPrefs or JSON
            // For now, return default data
            return new PlayerData
            {
                playerId = System.Guid.NewGuid().ToString(),
                username = "NewPlayer",
                level = 1,
                trophies = 0,
                gold = 500,
                elixir = 500,
                darkElixir = 0,
                gems = 50
            };
        }

        /// <summary>
        /// Save player data to storage
        /// </summary>
        public void SavePlayerData()
        {
            // TODO: Implement save system with JSON serialization
            Debug.Log("Saving player data...");
        }

        #endregion

        #region Resource Methods

        /// <summary>
        /// Add resources to player
        /// </summary>
        public void AddResource(ResourceType type, long amount)
        {
            switch (type)
            {
                case ResourceType.Gold:
                    playerData.gold += amount;
                    break;
                case ResourceType.Elixir:
                    playerData.elixir += amount;
                    break;
                case ResourceType.DarkElixir:
                    playerData.darkElixir += amount;
                    break;
                case ResourceType.Gems:
                    playerData.gems += (int)amount;
                    break;
            }
            
            Debug.Log($"Added {amount} {type}. New balance: {GetResourceAmount(type)}");
            UIManager.Instance?.UpdateResourceDisplay();
        }

        /// <summary>
        /// Remove resources from player
        /// </summary>
        public bool RemoveResource(ResourceType type, long amount)
        {
            long currentAmount = GetResourceAmount(type);
            
            if (currentAmount >= amount)
            {
                switch (type)
                {
                    case ResourceType.Gold:
                        playerData.gold -= amount;
                        break;
                    case ResourceType.Elixir:
                        playerData.elixir -= amount;
                        break;
                    case ResourceType.DarkElixir:
                        playerData.darkElixir -= amount;
                        break;
                    case ResourceType.Gems:
                        playerData.gems -= (int)amount;
                        break;
                }
                
                Debug.Log($"Removed {amount} {type}. New balance: {GetResourceAmount(type)}");
                UIManager.Instance?.UpdateResourceDisplay();
                return true;
            }
            
            Debug.LogWarning($"Insufficient {type}! Required: {amount}, Available: {currentAmount}");
            return false;
        }

        /// <summary>
        /// Try to deduct resource for building upgrade
        /// </summary>
        public bool TryUpgradeResource(Building building)
        {
            BuildingConfig config = gameConfig.GetBuildingConfig(building.BuildingType);
            long cost = gameConfig.CalculateUpgradeCost(config.CostType, config.BaseCost, building.Level);
            
            if (RemoveResource(config.CostType, cost))
            {
                building.StartUpgrade();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get current amount of a resource
        /// </summary>
        public long GetResourceAmount(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Gold:
                    return playerData.gold;
                case ResourceType.Elixir:
                    return playerData.elixir;
                case ResourceType.DarkElixir:
                    return playerData.darkElixir;
                case ResourceType.Gems:
                    return playerData.gems;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Check if player can afford a building
        /// </summary>
        public bool CanAffordBuilding(BuildingType type)
        {
            BuildingConfig config = gameConfig.GetBuildingConfig(type);
            return GetResourceAmount(config.CostType) >= config.BaseCost;
        }

        #endregion

        #region Building Management

        public bool TryUpgradeBuilding(Building building)
        {
            return TryUpgradeResource(building);
        }

        public bool DeductResource(long amount, ResourceType type)
        {
            return RemoveResource(type, amount);
        }

        #endregion
    }
}
