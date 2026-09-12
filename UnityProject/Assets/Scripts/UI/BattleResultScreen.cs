using UnityEngine;
using UnityEngine.UI;

namespace ClashGame.UI
{
    public class BattleResultScreen : MonoBehaviour
    {
        public static BattleResultScreen Instance { get; private set; }
        
        [Header("UI Elements")]
        public GameObject resultPanel;
        public Text starsText;
        public Text destructionText;
        public Text victoryDefeatText;
        public Text lootGoldText;
        public Text lootElixirText;
        public Text trophiesGainedText;
        
        [Header("Star Display")]
        public Image[] starImages;
        public Sprite starEmpty;
        public Sprite starFull;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
        }
        
        public void ShowResults(int stars, int destruction, bool victory)
        {
            if (resultPanel == null) return;
            
            resultPanel.SetActive(true);
            
            // Update stars display
            if (starsText != null)
            {
                starsText.text = $"Stars: {stars}/3";
            }
            
            // Update star images
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    starImages[i].sprite = i < stars ? starFull : starEmpty;
                }
            }
            
            // Update destruction percentage
            if (destructionText != null)
            {
                destructionText.text = $"Destruction: {destruction}%";
            }
            
            // Update victory/defeat text
            if (victoryDefeatText != null)
            {
                victoryDefeatText.text = victory ? "VICTORY!" : "DEFEAT";
                victoryDefeatText.color = victory ? Color.green : Color.red;
            }
            
            // Calculate loot
            int goldLoot = CalculateLoot(Core.GameConfig.Instance.goldProductionRate * 60, victory);
            int elixirLoot = CalculateLoot(Core.GameConfig.Instance.elixirProductionRate * 60, victory);
            int trophiesGained = victory ? 
                Mathf.RoundToInt(Core.GameConfig.Instance.trophyWinBase * (1 + stars * 0.5f)) : 
                -Mathf.RoundToInt(Core.GameConfig.Instance.trophyLossBase);
            
            // Update loot display
            if (lootGoldText != null)
            {
                lootGoldText.text = $"+{goldLoot} Gold";
            }
            
            if (lootElixirText != null)
            {
                lootElixirText.text = $"+{elixirLoot} Elixir";
            }
            
            if (trophiesGainedText != null)
            {
                string trophySign = trophiesGained >= 0 ? "+" : "";
                trophiesGainedText.text = $"{trophySign}{trophiesGained} Trophies";
            }
            
            // Apply rewards to player
            ApplyRewards(goldLoot, elixirLoot, trophiesGained);
        }
        
        private int CalculateLoot(float baseAmount, bool victory)
        {
            if (!victory) return 0;
            return Mathf.RoundToInt(baseAmount * Core.GameConfig.Instance.lootPercentage);
        }
        
        private void ApplyRewards(int gold, int elixir, int trophies)
        {
            Core.PlayerData playerData = Core.GameManager.Instance.playerData;
            
            if (playerData != null)
            {
                playerData.gold += gold;
                playerData.elixir += elixir;
                playerData.trophies += trophies;
                
                // Ensure trophies don't go below 0
                if (playerData.trophies < 0)
                {
                    playerData.trophies = 0;
                }
                
                Debug.Log($"Rewards Applied: Gold +{gold}, Elixir +{elixir}, Trophies {trophies}");
            }
        }
        
        public void CloseResults()
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
            
            // Cleanup battle
            if (Combat.BattleManager.Instance != null)
            {
                Combat.BattleManager.Instance.Cleanup();
            }
            
            // Return to village
            Core.GameManager.Instance.ChangeGameState(Core.GameManager.GameState.Village);
        }
    }
}
