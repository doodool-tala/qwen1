using UnityEngine;
using System.Collections;

namespace CoCGame.Gameplay
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        [Header("Prefabs")]
        public GameObject buildingPrefab;
        public GameObject previewPrefab;
        public GameObject upgradeProgressPrefab;

        [Header("Settings")]
        public LayerMask buildingLayer;
        public LayerMask groundLayer;

        private Camera mainCamera;
        private GameObject placementPreview;
        private bool isPlacing = false;
        private BuildingType pendingBuildingType;

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

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (isPlacing && placementPreview != null)
            {
                Vector3 worldPos = GetMouseWorldPosition();
                placementPreview.transform.position = new Vector3(
                    Mathf.Round(worldPos.x), 
                    Mathf.Round(worldPos.y), 
                    0
                );

                // Check validity
                bool isValid = VillageManager.Instance.IsValidPlacement(
                    (int)placementPreview.transform.position.x,
                    (int)placementPreview.transform.position.y,
                    pendingBuildingType
                );

                Color c = isValid ? Color.green : Color.red;
                c.a = 0.5f;
                placementPreview.GetComponent<SpriteRenderer>().color = c;

                if (Input.GetMouseButtonUp(0))
                {
                    if (isValid)
                    {
                        PlaceBuilding(
                            (int)placementPreview.transform.position.x,
                            (int)placementPreview.transform.position.y,
                            pendingBuildingType
                        );
                    }
                    CancelPlacement();
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CancelPlacement();
                }
            }
        }

        public void StartPlacement(BuildingType type)
        {
            pendingBuildingType = type;
            isPlacing = true;

            GameConfig config = GameManager.Instance.Config;
            BuildingConfig conf = config.GetBuildingConfig(type);

            placementPreview = Instantiate(previewPrefab);
            SpriteRenderer sr = placementPreview.GetComponent<SpriteRenderer>();
            sr.sprite = conf.Sprite;
            
            // Scale to match building size
            float scale = conf.Size * 0.5f; // Adjust based on your sprite sizing
            placementPreview.transform.localScale = new Vector3(scale, scale, 1);
            
            UIManager.Instance.ShowMainHud();
        }

        private void PlaceBuilding(int x, int y, BuildingType type)
        {
            GameConfig config = GameManager.Instance.Config;
            BuildingConfig conf = config.GetBuildingConfig(type);

            // Deduct cost
            bool success = GameManager.Instance.DeductResource(conf.BaseCost, conf.CostType);
            if (!success)
            {
                Debug.LogWarning("Not enough resources!");
                return;
            }

            // Create actual building
            Vector3 pos = new Vector3(x, y, 0);
            GameObject go = Instantiate(buildingPrefab, pos, Quaternion.identity);
            
            Building building = go.GetComponent<Building>();
            building.Initialize(type, 1, pos);

            // Register with village manager
            VillageManager.Instance.AddBuilding(building, x, y);

            Debug.Log($"Placed {conf.Name} at ({x}, {y})");
        }

        private void CancelPlacement()
        {
            isPlacing = false;
            pendingBuildingType = BuildingType.None;
            
            if (placementPreview != null)
            {
                Destroy(placementPreview);
                placementPreview = null;
            }

            UIManager.Instance.ShowBuildMenu();
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -mainCamera.transform.position.z; // Orthographic camera distance
            return mainCamera.ScreenToWorldPoint(mousePos);
        }

        public void ShowUpgradeProgress(Building building, float duration)
        {
            StartCoroutine(UpgradeProgressCoroutine(building, duration));
        }

        private IEnumerator UpgradeProgressCoroutine(Building building, float duration)
        {
            // Instantiate progress UI (could be a slider or spinner over the building)
            GameObject progressObj = Instantiate(upgradeProgressPrefab, building.transform);
            Slider slider = progressObj.GetComponentInChildren<Slider>();
            
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                slider.value = elapsed / duration;
                yield return null;
            }

            Destroy(progressObj);
            
            // Notify completion
            building.OnUpgradeComplete();
            VillageManager.Instance.OnBuildingUpgradeComplete(building);
        }
    }
}
