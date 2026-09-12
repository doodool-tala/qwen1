using UnityEngine;

namespace ClashGame.Core
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
            gameConfig = LoadGameConfig();
            
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
        }

        private void OnEnterBattle()
        {
            // Load battle scene and start battle
        }

        private void OnEnterShop()
        {
            // Show shop UI
        }

        private void OnEnterSettings()
        {
            // Show settings UI
        }

        #endregion

        #region Data Management

        /// <summary>
        /// Load player data from storage or create new
        /// </summary>
        private PlayerData LoadPlayerData()
        {
            // TODO: Implement actual save/load system
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
            // TODO: Implement save system
            Debug.Log("Saving player data...");
        }

        /// <summary>
        /// Load game configuration
        /// </summary>
        private GameConfig LoadGameConfig()
        {
            // TODO: Load from ScriptableObject or JSON
            return new GameConfig();
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
                    playerData.gems += amount;
                    break;
            }
            
            Debug.Log($"Added {amount} {type}. New balance: {GetResourceAmount(type)}");
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
                        playerData.gems -= amount;
                        break;
                }
                
                Debug.Log($"Removed {amount} {type}. New balance: {GetResourceAmount(type)}");
                return true;
            }
            
            Debug.LogWarning($"Insufficient {type}! Required: {amount}, Available: {currentAmount}");
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

        #endregion
    }
}
