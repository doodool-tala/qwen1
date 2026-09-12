using UnityEngine;
using System.Collections.Generic;

namespace ClashGame.Combat
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }
        
        [Header("Battle State")]
        public bool isBattleActive = false;
        public float battleTimer = 0f;
        public int battleDurationSeconds = 180;
        
        [Header("Battle Results")]
        public int destructionPercentage = 0;
        public int starsEarned = 0;
        public bool isVictory = false;
        
        [Header("References")]
        public List<Building> enemyBuildings = new List<Building>();
        public List<Troop> deployedTroops = new List<Troop>();
        
        public enum BattleState
        {
            Deployment,
            Active,
            Ended
        }
        
        public BattleState currentState = BattleState.Deployment;
        
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
        }
        
        public void StartBattle(List<Building> enemyBase)
        {
            enemyBuildings = new List<Building>(enemyBase);
            deployedTroops = new List<Troop>();
            isBattleActive = true;
            battleTimer = battleDurationSeconds;
            destructionPercentage = 0;
            starsEarned = 0;
            currentState = BattleState.Deployment;
            
            Debug.Log("Battle Started!");
        }
        
        public void DeployTroop(Troop troop, Vector3 position)
        {
            if (!isBattleActive || currentState != BattleState.Deployment)
                return;
            
            Troop newTroop = Instantiate(troop, position, Quaternion.identity);
            deployedTroops.Add(newTroop);
            
            // Transition to active state after first deployment
            if (currentState == BattleState.Deployment)
            {
                currentState = BattleState.Active;
            }
        }
        
        private void Update()
        {
            if (!isBattleActive) return;
            
            if (currentState == BattleState.Active)
            {
                battleTimer -= Time.deltaTime;
                UpdateDestructionPercentage();
                CheckWinCondition();
                
                if (battleTimer <= 0)
                {
                    EndBattle();
                }
            }
        }
        
        private void UpdateDestructionPercentage()
        {
            if (enemyBuildings.Count == 0)
            {
                destructionPercentage = 100;
                return;
            }
            
            int destroyedCount = 0;
            foreach (var building in enemyBuildings)
            {
                if (building == null || building.IsDestroyed)
                {
                    destroyedCount++;
                }
            }
            
            destructionPercentage = Mathf.RoundToInt((destroyedCount / (float)enemyBuildings.Count) * 100f);
        }
        
        private void CheckWinCondition()
        {
            // Calculate stars
            starsEarned = 0;
            
            if (destructionPercentage >= 50) starsEarned = 1;
            if (destructionPercentage >= 75) starsEarned = 2;
            if (destructionPercentage >= 100) starsEarned = 3;
            
            // Check Town Hall destruction (instant 1 star)
            foreach (var building in enemyBuildings)
            {
                if (building != null && building.buildingType == BuildingType.TownHall && building.IsDestroyed)
                {
                    starsEarned = Mathf.Max(starsEarned, 1);
                }
            }
            
            isVictory = starsEarned >= 1;
        }
        
        public void EndBattle()
        {
            isBattleActive = false;
            currentState = BattleState.Ended;
            
            Debug.Log($"Battle Ended - Stars: {starsEarned}, Destruction: {destructionPercentage}%, Victory: {isVictory}");
            
            // Trigger battle results UI
            if (UI.BattleResultScreen.Instance != null)
            {
                UI.BattleResultScreen.Instance.ShowResults(starsEarned, destructionPercentage, isVictory);
            }
        }
        
        public void Cleanup()
        {
            foreach (var troop in deployedTroops)
            {
                if (troop != null)
                {
                    Destroy(troop.gameObject);
                }
            }
            deployedTroops.Clear();
        }
    }
}
