using UnityEngine;
using System.Collections;

namespace CoCGame.Gameplay
{
    public class ResourceProducer : MonoBehaviour
    {
        [Header("Settings")]
        public BuildingType producerType;
        public float productionInterval = 60f; // Seconds per tick
        public int baseProductionAmount = 100;
        
        private Building attachedBuilding;
        private float timeSinceLastProduction;
        private int maxCapacity;
        private int currentStored;

        public int CurrentStored => currentStored;
        public int MaxCapacity => maxCapacity;
        public bool IsFull => currentStored >= maxCapacity;

        private void Awake()
        {
            attachedBuilding = GetComponent<Building>();
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            GameConfig config = GameManager.Instance.Config;
            ProducerConfig prodConf = config.GetProducerConfig(producerType);
            
            productionInterval = prodConf.ProductionTime;
            baseProductionAmount = prodConf.BaseProduction;
            maxCapacity = prodConf.Capacity;
            currentStored = 0;
            timeSinceLastProduction = 0;
        }

        private void Update()
        {
            if (attachedBuilding == null || attachedBuilding.IsDestroyed || attachedBuilding.IsUpgrading)
                return;

            timeSinceLastProduction += Time.deltaTime;

            // Calculate production rate based on building level
            int actualProduction = Mathf.FloorToInt(baseProductionAmount * 
                GameManager.Instance.Config.GetProductionMultiplier(attachedBuilding.Level));

            // Check if enough time has passed for one production tick
            if (timeSinceLastProduction >= productionInterval)
            {
                Produce(actualProduction);
                timeSinceLastProduction = 0;
            }
        }

        private void Produce(int amount)
        {
            if (currentStored >= maxCapacity)
                return;

            int spaceAvailable = maxCapacity - currentStored;
            int toAdd = Mathf.Min(amount, spaceAvailable);

            currentStored += toAdd;

            // Visual feedback could go here (particles, floating text)
            Debug.Log($"Produced {toAdd} resources. Total stored: {currentStored}/{maxCapacity}");
        }

        public int Collect()
        {
            int collected = currentStored;
            currentStored = 0;

            if (collected > 0)
            {
                AddToPlayerResources(collected);
                Debug.Log($"Collected {collected} resources!");
            }

            return collected;
        }

        private void AddToPlayerResources(int amount)
        {
            GameConfig config = GameManager.Instance.Config;
            ProducerConfig prodConf = config.GetProducerConfig(producerType);

            switch (prodConf.ResourceType)
            {
                case ResourceType.Gold:
                    GameManager.Instance.PlayerData.Gold += amount;
                    break;
                case ResourceType.Elixir:
                    GameManager.Instance.PlayerData.Elixir += amount;
                    break;
                case ResourceType.DarkElixir:
                    GameManager.Instance.PlayerData.DarkElixir += amount;
                    break;
            }

            UIManager.Instance.UpdateResourceDisplay();
        }

        public float GetProgress()
        {
            return timeSinceLastProduction / productionInterval;
        }

        public float GetTimeUntilNextProduction()
        {
            return productionInterval - timeSinceLastProduction;
        }
    }

    // Configuration for resource producers
    [System.Serializable]
    public class ProducerConfig
    {
        public BuildingType Type;
        public ResourceType ResourceType;
        public int BaseProduction;      // Amount per tick
        public float ProductionTime;    // Seconds per tick
        public int Capacity;            // Max storage before collection needed
        public int UpgradeFactor;       // Multiplier per level
    }
}
