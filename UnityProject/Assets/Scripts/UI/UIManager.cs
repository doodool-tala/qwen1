using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CoCGame.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Screens")]
        public GameObject mainHud;
        public GameObject buildMenu;
        public GameObject upgradePanel;
        public GameObject collectPopup;

        [Header("HUD Elements")]
        public Text goldText;
        public Text elixirText;
        public Text darkElixirText;
        public Text gemsText;
        public Text trophyText;
        public Text levelText;

        [Header("Upgrade Panel Elements")]
        public Text buildingNameText;
        public Text buildingLevelText;
        public Text upgradeCostText;
        public Text upgradeTimeText;
        public Button upgradeButton;
        public Button cancelButton;

        [Header("Build Menu")]
        public Transform buildButtonContainer;
        public GameObject buildButtonPrefab;

        private Building selectedBuilding;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            UpdateResourceDisplay();
            ShowMainHud();
        }

        public void UpdateResourceDisplay()
        {
            if (GameManager.Instance == null) return;

            PlayerData data = GameManager.Instance.PlayerData;
            GameConfig config = GameManager.Instance.Config;

            goldText.text = Mathf.FloorToInt(data.Gold).ToString("N0");
            elixirText.text = Mathf.FloorToInt(data.Elixir).ToString("N0");
            darkElixirText.text = Mathf.FloorToInt(data.DarkElixir).ToString("N0");
            gemsText.text = data.Gems.ToString("N0");
            trophyText.text = data.Trophies.ToString("N0");
            levelText.text = "Lvl " + data.Level;
        }

        public void ShowMainHud()
        {
            HideAllScreens();
            mainHud.SetActive(true);
        }

        public void ShowBuildMenu()
        {
            HideAllScreens();
            buildMenu.SetActive(true);
            PopulateBuildMenu();
        }

        public void ShowUpgradePanel(Building building)
        {
            selectedBuilding = building;
            HideAllScreens();
            upgradePanel.SetActive(true);

            GameConfig config = GameManager.Instance.Config;
            BuildingConfig conf = config.GetBuildingConfig(building.BuildingType);

            buildingNameText.text = conf.Name;
            buildingLevelText.text = "Level " + building.Level;

            int cost = config.GetUpgradeCost(conf.BaseCost, building.Level);
            float time = config.GetUpgradeTime(conf.BaseTime, building.Level);

            upgradeCostText.text = FormatCost(cost, conf.CostType);
            upgradeTimeText.text = FormatTime(time);

            // Check affordability
            bool canAfford = CanAfford(cost, conf.CostType);
            upgradeButton.interactable = canAfford && !building.IsUpgrading;
            
            if (!canAfford)
                upgradeCostText.color = Color.red;
            else
                upgradeCostText.color = Color.white;
        }

        public void OnUpgradeClicked()
        {
            if (selectedBuilding != null)
            {
                GameManager.Instance.TryUpgradeBuilding(selectedBuilding);
                ShowMainHud();
            }
        }

        public void OnCancelUpgradeClicked()
        {
            ShowMainHud();
        }

        public void OnCollectClicked(Building building)
        {
            float collected = building.CollectResources();
            // Could show a popup here
            UpdateResourceDisplay();
        }

        private void PopulateBuildMenu()
        {
            // Clear existing
            foreach (Transform child in buildButtonContainer)
                Destroy(child.gameObject);

            GameConfig config = GameManager.Instance.Config;

            foreach (var kvp in config.Buildings)
            {
                BuildingConfig conf = kvp.Value;
                if (conf.IsDecorative) continue; // Skip decorations for now

                GameObject btnObj = Instantiate(buildButtonPrefab, buildButtonContainer);
                Button btn = btnObj.GetComponent<Button>();
                Text lbl = btnObj.GetComponentInChildren<Text>();
                lbl.text = conf.Name;

                // Add click listener to start placement mode
                // (Implementation would go in VillageManager)
            }
        }

        private bool CanAfford(int cost, ResourceType type)
        {
            PlayerData data = GameManager.Instance.PlayerData;
            switch (type)
            {
                case ResourceType.Gold: return data.Gold >= cost;
                case ResourceType.Elixir: return data.Elixir >= cost;
                case ResourceType.DarkElixir: return data.DarkElixir >= cost;
                default: return data.Gems >= cost;
            }
        }

        private string FormatCost(int cost, ResourceType type)
        {
            string resourceName = type.ToString();
            return $"{cost:N0} {resourceName}";
        }

        private string FormatTime(float seconds)
        {
            System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);
            if (t.Days > 0) return $"{t.Days}d {t.Hours}h";
            if (t.Hours > 0) return $"{t.Hours}h {t.Minutes}m";
            return $"{t.Minutes}m {t.Seconds}s";
        }

        private void HideAllScreens()
        {
            mainHud.SetActive(false);
            buildMenu.SetActive(false);
            upgradePanel.SetActive(false);
            collectPopup.SetActive(false);
        }
    }
}
