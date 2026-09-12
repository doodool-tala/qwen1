using UnityEngine;

namespace ClashGame.Gameplay
{
    /// <summary>
    /// Manages the village grid system and building placement
    /// </summary>
    public class VillageManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        public int gridSize = 32; // 32x32 grid
        public float cellSize = 2.0f; // Each cell is 2x2 units
        public Vector2Int villageCenter = new Vector2Int(16, 16);
        
        [Header("References")]
        public GameObject buildingPrefab;
        public GameObject wallPrefab;
        
        [Header("Village Data")]
        public Building[,] grid;
        public System.Collections.Generic.List<Building> placedBuildings;
        
        [Header("Placement Mode")]
        public bool isPlacementMode = false;
        public BuildingType buildingToPlace;
        public GameObject placementPreview;
        public Vector2Int currentGridPosition;
        
        private Camera mainCamera;
        
        private void Start()
        {
            InitializeVillage();
            mainCamera = Camera.main;
        }
        
        /// <summary>
        /// Initialize the village grid
        /// </summary>
        public void InitializeVillage()
        {
            grid = new Building[gridSize, gridSize];
            placedBuildings = new System.Collections.Generic.List<Building>();
            
            // Place initial buildings for new player
            PlaceInitialBuildings();
            
            Debug.Log($"Village initialized with {gridSize}x{gridSize} grid");
        }
        
        /// <summary>
        /// Place initial buildings for a new player
        /// </summary>
        private void PlaceInitialBuildings()
        {
            // Place Town Hall in center
            PlaceBuilding(BuildingType.TownHall, villageCenter);
            
            // Place some initial resource collectors
            PlaceBuilding(BuildingType.GoldMine, villageCenter + new Vector2Int(-3, -2));
            PlaceBuilding(BuildingType.GoldMine, villageCenter + new Vector2Int(3, -2));
            PlaceBuilding(BuildingType.ElixirCollector, villageCenter + new Vector2Int(-3, 2));
            PlaceBuilding(BuildingType.ElixirCollector, villageCenter + new Vector2Int(3, 2));
            
            // Place initial storages
            PlaceBuilding(BuildingType.GoldStorage, villageCenter + new Vector2Int(-5, 0));
            PlaceBuilding(BuildingType.ElixirStorage, villageCenter + new Vector2Int(5, 0));
            
            // Place some walls around town hall
            PlaceWall(villageCenter + new Vector2Int(-2, -2));
            PlaceWall(villageCenter + new Vector2Int(2, -2));
            PlaceWall(villageCenter + new Vector2Int(-2, 2));
            PlaceWall(villageCenter + new Vector2Int(2, 2));
        }
        
        /// <summary>
        /// Enable placement mode for a specific building type
        /// </summary>
        public void EnablePlacementMode(BuildingType type)
        {
            isPlacementMode = true;
            buildingToPlace = type;
            
            // Create preview object
            if (placementPreview == null && buildingPrefab != null)
            {
                placementPreview = Instantiate(buildingPrefab);
                Renderer renderer = placementPreview.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = new Material(renderer.material);
                    mat.color = new Color(0, 1, 0, 0.5f); // Green transparent
                    renderer.material = mat;
                }
            }
            
            Debug.Log($"Placement mode enabled for {type}");
        }
        
        /// <summary>
        /// Disable placement mode
        /// </summary>
        public void DisablePlacementMode()
        {
            isPlacementMode = false;
            buildingToPlace = BuildingType.TownHall;
            
            if (placementPreview != null)
            {
                Destroy(placementPreview);
                placementPreview = null;
            }
        }
        
        private void Update()
        {
            if (isPlacementMode)
            {
                UpdatePlacementPreview();
                
                if (Input.GetMouseButtonDown(0))
                {
                    TryPlaceBuilding();
                }
                
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    DisablePlacementMode();
                }
            }
        }
        
        /// <summary>
        /// Update the placement preview position
        /// </summary>
        private void UpdatePlacementPreview()
        {
            if (placementPreview == null) return;
            
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -mainCamera.transform.position.y;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            
            // Convert to grid position
            currentGridPosition = WorldToGrid(worldPos);
            
            // Clamp to grid bounds
            currentGridPosition.x = Mathf.Clamp(currentGridPosition.x, 0, gridSize - 1);
            currentGridPosition.y = Mathf.Clamp(currentGridPosition.y, 0, gridSize - 1);
            
            // Update preview position
            Vector3 gridWorldPos = GridToWorld(currentGridPosition);
            placementPreview.transform.position = new Vector3(gridWorldPos.x, gridWorldPos.y, 0);
            
            // Check if placement is valid
            bool isValid = IsValidPlacement(currentGridPosition, buildingToPlace);
            placementPreview.GetComponent<SpriteRenderer>().color = 
                isValid ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        }
        
        /// <summary>
        /// Try to place the building at current position
        /// </summary>
        private void TryPlaceBuilding()
        {
            if (!IsValidPlacement(currentGridPosition, buildingToPlace))
            {
                Debug.LogWarning("Invalid placement position!");
                return;
            }
            
            // Check if player can afford
            long cost = GetBuildingCost(buildingToPlace);
            if (GameManager.Instance != null && GameManager.Instance.playerData.gold < cost)
            {
                Debug.LogWarning("Not enough gold!");
                return;
            }
            
            // Place the building
            Building placed = PlaceBuilding(buildingToPlace, currentGridPosition);
            
            if (placed != null)
            {
                // Deduct cost
                GameManager.Instance.RemoveResource(ResourceType.Gold, cost);
                
                Debug.Log($"Placed {buildingToPlace} at {currentGridPosition}");
            }
        }
        
        /// <summary>
        /// Place a building at grid position
        /// </summary>
        public Building PlaceBuilding(BuildingType type, Vector2Int gridPos)
        {
            if (grid[gridPos.x, gridPos.y] != null)
            {
                Debug.LogWarning("Cell already occupied!");
                return null;
            }
            
            // Create building instance
            GameObject go = Instantiate(buildingPrefab, GridToWorld(gridPos), Quaternion.identity);
            Building building = go.GetComponent<Building>();
            
            if (building == null)
            {
                building = go.AddComponent<Building>();
            }
            
            // Initialize building data
            building.buildingId = System.Guid.NewGuid().ToString();
            building.type = type;
            building.gridPosition = gridPos;
            building.level = 1;
            
            // Set up building stats based on type
            SetupBuildingStats(building, type);
            
            // Add to grid and list
            grid[gridPos.x, gridPos.y] = building;
            placedBuildings.Add(building);
            
            return building;
        }
        
        /// <summary>
        /// Place a wall segment
        /// </summary>
        public Building PlaceWall(Vector2Int gridPos)
        {
            return PlaceBuilding(BuildingType.Wall, gridPos);
        }
        
        /// <summary>
        /// Remove a building from the village
        /// </summary>
        public bool RemoveBuilding(Vector2Int gridPos)
        {
            if (gridPos.x < 0 || gridPos.x >= gridSize || 
                gridPos.y < 0 || gridPos.y >= gridSize)
            {
                return false;
            }
            
            Building building = grid[gridPos.x, gridPos.y];
            
            if (building == null)
            {
                return false;
            }
            
            // Can't remove Town Hall
            if (building.type == BuildingType.TownHall)
            {
                Debug.LogWarning("Cannot remove Town Hall!");
                return false;
            }
            
            // Refund some resources
            long refund = GetBuildingCost(building.type) / 2;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddResource(ResourceType.Gold, refund);
            }
            
            // Remove from grid and list
            grid[gridPos.x, gridPos.y] = null;
            placedBuildings.Remove(building);
            
            // Destroy GameObject
            Destroy(building.gameObject);
            
            Debug.Log($"Removed {building.type} from {gridPos}");
            return true;
        }
        
        /// <summary>
        /// Check if placement is valid
        /// </summary>
        public bool IsValidPlacement(Vector2Int gridPos, BuildingType type)
        {
            // Check bounds
            if (gridPos.x < 0 || gridPos.x >= gridSize || 
                gridPos.y < 0 || gridPos.y >= gridSize)
            {
                return false;
            }
            
            // Check if cell is empty
            if (grid[gridPos.x, gridPos.y] != null)
            {
                return false;
            }
            
            // Check collision with nearby buildings (simple radius check)
            int radius = GetBuildingRadius(type);
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector2Int checkPos = gridPos + new Vector2Int(x, y);
                    
                    if (checkPos.x >= 0 && checkPos.x < gridSize &&
                        checkPos.y >= 0 && checkPos.y < gridSize)
                    {
                        if (grid[checkPos.x, checkPos.y] != null)
                        {
                            return false;
                        }
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Get building cost based on type
        /// </summary>
        private long GetBuildingCost(BuildingType type)
        {
            return type switch
            {
                BuildingType.GoldMine => 250,
                BuildingType.ElixirCollector => 250,
                BuildingType.GoldStorage => 500,
                BuildingType.ElixirStorage => 500,
                BuildingType.Cannon => 500,
                BuildingType.ArcherTower => 750,
                BuildingType.Wall => 100,
                BuildingType.Barracks => 1000,
                BuildingType.TownHall => 0, // Free upgrade only
                _ => 500
            };
        }
        
        /// <summary>
        /// Get building radius for collision detection
        /// </summary>
        private int GetBuildingRadius(BuildingType type)
        {
            return type switch
            {
                BuildingType.Wall => 1,
                BuildingType.TownHall => 3,
                BuildingType.GoldStorage => 2,
                BuildingType.ElixirStorage => 2,
                _ => 2
            };
        }
        
        /// <summary>
        /// Setup building stats based on type
        /// </summary>
        private void SetupBuildingStats(Building building, BuildingType type)
        {
            switch (type)
            {
                case BuildingType.GoldMine:
                    building.producedResource = ResourceType.Gold;
                    building.productionRate = 1.0f;
                    building.maxStorage = 500;
                    break;
                    
                case BuildingType.ElixirCollector:
                    building.producedResource = ResourceType.Elixir;
                    building.productionRate = 1.0f;
                    building.maxStorage = 500;
                    break;
                    
                case BuildingType.Cannon:
                    building.maxHitPoints = 500;
                    building.damagePerSecond = 20;
                    building.attackRange = 7f;
                    building.targetLayer = "Ground";
                    break;
                    
                case BuildingType.ArcherTower:
                    building.maxHitPoints = 400;
                    building.damagePerSecond = 15;
                    building.attackRange = 9f;
                    building.targetLayer = "Both";
                    break;
                    
                case BuildingType.TownHall:
                    building.maxHitPoints = 1000;
                    break;
                    
                case BuildingType.Wall:
                    building.maxHitPoints = 200;
                    break;
            }
            
            building.InitializeBuilding();
        }
        
        /// <summary>
        /// Convert grid position to world position
        /// </summary>
        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(
                (gridPos.x - gridSize / 2f) * cellSize,
                (gridPos.y - gridSize / 2f) * cellSize,
                0
            );
        }
        
        /// <summary>
        /// Convert world position to grid position
        /// </summary>
        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt(worldPos.x / cellSize + gridSize / 2f);
            int y = Mathf.RoundToInt(worldPos.y / cellSize + gridSize / 2f);
            return new Vector2Int(x, y);
        }
        
        /// <summary>
        /// Get all resource producer buildings
        /// </summary>
        public System.Collections.Generic.List<Building> GetResourceProducers()
        {
            var producers = new System.Collections.Generic.List<Building>();
            
            foreach (var building in placedBuildings)
            {
                if (building.IsProducer())
                {
                    producers.Add(building);
                }
            }
            
            return producers;
        }
        
        /// <summary>
        /// Collect resources from all producers
        /// </summary>
        public void CollectAllResources()
        {
            var producers = GetResourceProducers();
            
            foreach (var producer in producers)
            {
                producer.CollectResources();
            }
        }
    }
}
