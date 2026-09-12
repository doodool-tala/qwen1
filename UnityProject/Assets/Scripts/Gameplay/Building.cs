using UnityEngine;

namespace ClashGame.Gameplay
{
    /// <summary>
    /// Represents a building in the village
    /// </summary>
    public class Building : MonoBehaviour
    {
        [Header("Building Info")]
        public string buildingId;
        public BuildingType type;
        public int level = 1;
        public int maxLevel = 10;
        
        [Header("Position")]
        public Vector2 gridPosition;
        public float rotationAngle;
        
        [Header("Stats")]
        public int hitPoints;
        public int maxHitPoints;
        public bool isDestroyed = false;
        
        [Header("Upgrade System")]
        public bool isUpgrading = false;
        public float upgradeStartTime;
        public float upgradeDuration;
        public long upgradeCostGold;
        public long upgradeCostElixir;
        public long upgradeCostDarkElixir;
        
        [Header("Production Buildings Only")]
        public ResourceType producedResource;
        public float productionRate; // Resources per second
        public float lastCollectionTime;
        public long storedResources;
        public long maxStorage;
        
        [Header("Defense Buildings Only")]
        public float attackRange;
        public float damagePerSecond;
        public float attackSpeed;
        public string targetLayer; // Which layer to target (Ground, Air, Both)
        
        [Header("Visual References")]
        public SpriteRenderer spriteRenderer;
        public GameObject upgradeEffectPrefab;
        public GameObject destructionEffectPrefab;
        
        private void Start()
        {
            InitializeBuilding();
        }
        
        /// <summary>
        /// Initialize building with default values
        /// </summary>
        public void InitializeBuilding()
        {
            hitPoints = maxHitPoints;
            isDestroyed = false;
            isUpgrading = false;
            lastCollectionTime = Time.time;
            
            if (spriteRenderer != null)
            {
                UpdateVisuals();
            }
        }
        
        /// <summary>
        /// Start upgrading the building
        /// </summary>
        public bool StartUpgrade()
        {
            if (isUpgrading || level >= maxLevel)
            {
                return false;
            }
            
            // Check if player has enough resources
            GameManager gm = GameManager.Instance;
            if (gm == null) return false;
            
            bool canAfford = true;
            
            if (upgradeCostGold > 0 && !gm.RemoveResource(ResourceType.Gold, upgradeCostGold))
                canAfford = false;
                
            if (upgradeCostElixir > 0 && !gm.RemoveResource(ResourceType.Elixir, upgradeCostElixir))
                canAfford = false;
                
            if (upgradeCostDarkElixir > 0 && !gm.RemoveResource(ResourceType.DarkElixir, upgradeCostDarkElixir))
                canAfford = false;
            
            if (!canAfford)
            {
                Debug.LogWarning($"Cannot afford upgrade for {type}");
                return false;
            }
            
            // Start upgrade
            isUpgrading = true;
            upgradeStartTime = Time.time;
            
            Debug.Log($"Started upgrading {type} to level {level + 1}. Duration: {upgradeDuration}s");
            return true;
        }
        
        /// <summary>
        /// Update upgrade progress
        /// </summary>
        private void Update()
        {
            if (isUpgrading)
            {
                float elapsed = Time.time - upgradeStartTime;
                
                if (elapsed >= upgradeDuration)
                {
                    CompleteUpgrade();
                }
            }
            
            // Handle resource production
            if (IsProducer() && !isUpgrading && !isDestroyed)
            {
                ProduceResources();
            }
        }
        
        /// <summary>
        /// Complete the upgrade
        /// </summary>
        public void CompleteUpgrade()
        {
            isUpgrading = false;
            level++;
            
            // Recalculate stats for new level
            RecalculateStats();
            
            // Add experience to player
            if (GameManager.Instance != null && GameManager.Instance.gameConfig != null)
            {
                GameManager.Instance.playerData.AddExperience(
                    GameManager.Instance.gameConfig.xpPerBuildingUpgrade);
            }
            
            // Show upgrade effect
            if (upgradeEffectPrefab != null)
            {
                Instantiate(upgradeEffectPrefab, transform.position, Quaternion.identity);
            }
            
            UpdateVisuals();
            
            Debug.Log($"{type} upgraded to level {level}!");
        }
        
        /// <summary>
        /// Cancel current upgrade (with partial refund)
        /// </summary>
        public void CancelUpgrade()
        {
            if (!isUpgrading) return;
            
            isUpgrading = false;
            
            // Refund 50% of resources
            long refundGold = upgradeCostGold / 2;
            long refundElixir = upgradeCostElixir / 2;
            long refundDarkElixir = upgradeCostDarkElixir / 2;
            
            if (refundGold > 0) GameManager.Instance.AddResource(ResourceType.Gold, refundGold);
            if (refundElixir > 0) GameManager.Instance.AddResource(ResourceType.Elixir, refundElixir);
            if (refundDarkElixir > 0) GameManager.Instance.AddResource(ResourceType.DarkElixir, refundDarkElixir);
            
            Debug.Log($"Upgrade cancelled. Refunded 50% of resources.");
        }
        
        /// <summary>
        /// Collect resources from producer building
        /// </summary>
        public long CollectResources()
        {
            if (!IsProducer() || storedResources <= 0)
            {
                return 0;
            }
            
            long collected = storedResources;
            storedResources = 0;
            lastCollectionTime = Time.time;
            
            // Add to player's resources
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddResource(producedResource, collected);
                GameManager.Instance.playerData.totalResourcesCollected += (int)collected;
            }
            
            Debug.Log($"Collected {collected} {producedResource} from {type}");
            return collected;
        }
        
        /// <summary>
        /// Produce resources over time
        /// </summary>
        private void ProduceResources()
        {
            float deltaTime = Time.time - lastCollectionTime;
            long produced = (long)(productionRate * deltaTime);
            
            if (produced > 0 && storedResources < maxStorage)
            {
                long spaceAvailable = maxStorage - storedResources;
                long actualProduced = Mathf.Min(produced, spaceAvailable);
                
                storedResources += actualProduced;
                
                // Reset timer but keep fractional time
                lastCollectionTime = Time.time;
            }
        }
        
        /// <summary>
        /// Take damage from attack
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (isDestroyed) return;
            
            hitPoints -= (int)damage;
            
            if (hitPoints <= 0)
            {
                DestroyBuilding();
            }
        }
        
        /// <summary>
        /// Destroy the building
        /// </summary>
        public void DestroyBuilding()
        {
            isDestroyed = true;
            hitPoints = 0;
            
            // Show destruction effect
            if (destructionEffectPrefab != null)
            {
                Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
            }
            
            Debug.Log($"{type} has been destroyed!");
        }
        
        /// <summary>
        /// Recalculate stats based on current level
        /// </summary>
        private void RecalculateStats()
        {
            // Base stats increase by 10% per level
            maxHitPoints = (int)(maxHitPoints * 1.1f);
            hitPoints = maxHitPoints;
            
            if (IsProducer())
            {
                productionRate *= 1.2f; // 20% increase per level
                maxStorage = (long)(maxStorage * 1.5f); // 50% storage increase
            }
            
            if (IsDefensive())
            {
                damagePerSecond *= 1.15f; // 15% DPS increase
                attackRange *= 1.05f; // 5% range increase
            }
        }
        
        /// <summary>
        /// Update visual appearance based on level and state
        /// </summary>
        private void UpdateVisuals()
        {
            if (spriteRenderer == null) return;
            
            // Change color when upgrading
            if (isUpgrading)
            {
                spriteRenderer.color = Color.yellow;
            }
            else if (isDestroyed)
            {
                spriteRenderer.color = Color.gray;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
        
        /// <summary>
        /// Check if building produces resources
        /// </summary>
        public bool IsProducer()
        {
            return type == BuildingType.GoldMine || 
                   type == BuildingType.ElixirCollector || 
                   type == BuildingType.DarkElixirDrill;
        }
        
        /// <summary>
        /// Check if building is defensive
        /// </summary>
        public bool IsDefensive()
        {
            return type == BuildingType.Cannon || 
                   type == BuildingType.ArcherTower || 
                   type == BuildingType.Mortar || 
                   type == BuildingType.AirDefense;
        }
        
        /// <summary>
        /// Check if building is a storage
        /// </summary>
        public bool IsStorage()
        {
            return type == BuildingType.GoldStorage || 
                   type == BuildingType.ElixirStorage || 
                   type == BuildingType.DarkElixirStorage;
        }
    }
    
    /// <summary>
    /// Types of buildings in the game
    /// </summary>
    public enum BuildingType
    {
        // Core
        TownHall,
        
        // Resource Production
        GoldMine,
        ElixirCollector,
        DarkElixirDrill,
        
        // Resource Storage
        GoldStorage,
        ElixirStorage,
        DarkElixirStorage,
        
        // Defensive
        Cannon,
        ArcherTower,
        Mortar,
        AirDefense,
        WizardTower,
        HiddenTesla,
        
        // Walls
        Wall,
        
        // Army
        Barracks,
        DarkBarracks,
        ArmyCamp,
        SpellFactory,
        DarkSpellFactory,
        ClanCastle,
        Laboratory,
        
        // Special
        HeroAltar,
        Decoration
    }
}
