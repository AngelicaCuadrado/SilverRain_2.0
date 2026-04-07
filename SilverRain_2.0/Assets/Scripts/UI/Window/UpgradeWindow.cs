using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeWindow : UIWindow
{
    [Header("Left Panel UI")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button buyButton;

    [Header("Bottom UI")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetButton;

    [Header("Right Panel Slots")]
    [SerializeField] private List<UpgradeSlot> upgradeSlots;

    private PermanentUpgrade _selectedUpgrade;

    public void Start()
    {
        foreach (var slot in upgradeSlots)
        {
            slot.button.image.sprite = StatManager.Instance.GetPermanentUpgrade(slot.statType).UIData.UpgradeIcon;
        }
    }
    public override void OnPushed()
    {
        BindUIEvents();
        UpdateGoldUI();

        if (upgradeSlots.Count > 0)
        {
            SelectUpgradeByType(upgradeSlots[0].statType);
        }
        UpdateAllSlotsUI();
    }

    public override void OnPopped()
    {
        UnbindUIEvents();
    }

    private void BindUIEvents()
    {
        backButton.onClick.AddListener(Back);
        buyButton.onClick.AddListener(OnBuyClicked);
        resetButton.onClick.AddListener(OnResetClicked);

        GoldManager.OnGoldChanged.AddListener(UpdateGoldUI);

        foreach (var slot in upgradeSlots)
        {
            if (slot.button != null)
            {
                StatType typeToSelect = slot.statType;
                slot.button.onClick.AddListener(() => SelectUpgradeByType(typeToSelect));
            }
        }
    }

    private void UnbindUIEvents()
    {
        backButton.onClick.RemoveListener(Back);
        buyButton.onClick.RemoveListener(OnBuyClicked);
        resetButton.onClick.RemoveListener(OnResetClicked);

        GoldManager.OnGoldChanged.RemoveListener(UpdateGoldUI);

        foreach (var slot in upgradeSlots)
        {
            if (slot.button != null)
            {
                slot.button.onClick.RemoveAllListeners();
            }
        }
    }

    private void SelectUpgradeByType(StatType type)
    {
        PermanentUpgrade upgrade = StatManager.Instance.GetPermanentUpgrade(type);
        SelectUpgrade(upgrade);
    }

    private void SelectUpgrade(PermanentUpgrade upgrade)
    {
        if (upgrade == null) return;

        _selectedUpgrade = upgrade;
        _selectedUpgrade.UpdateDescription();

        nameText.text = _selectedUpgrade.UIData.UpgradeName;
        levelText.text = $"Lv:{_selectedUpgrade.Level} / {_selectedUpgrade.MaxLevel}";
        descriptionText.text = _selectedUpgrade.UIData.FinalUpgradeDescription;

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (_selectedUpgrade == null) return;

        if (_selectedUpgrade.Level >= _selectedUpgrade.MaxLevel)
        {
            costText.text = "MAX";
            buyButton.interactable = false;
        }
        else
        {
            float cost = _selectedUpgrade.GetNextLevelCost();
            costText.text = $"{cost}G";
            buyButton.interactable = GoldManager.Instance.CurrentGold >= cost;
        }
    }

    private void OnBuyClicked()
    {
        if (_selectedUpgrade == null || _selectedUpgrade.Level >= _selectedUpgrade.MaxLevel) return;

        float cost = _selectedUpgrade.GetNextLevelCost();

        if (GoldManager.Instance.SpendGold(cost))
        {
            _selectedUpgrade.LevelUp();
            StatManager.Instance.UpdatePermStats(_selectedUpgrade.StatType);
            SelectUpgrade(_selectedUpgrade);
        }
        UpdateAllSlotsUI();
    }

    private void OnResetClicked()
    {
        GoldManager.Instance.RefundAllSpentGold();
        StatManager.Instance.ResetPermStats();

        foreach (var slot in upgradeSlots)
        {
            StatManager.Instance.UpdatePermStats(slot.statType);
        }

        if (_selectedUpgrade != null)
        {
            SelectUpgrade(_selectedUpgrade);
        }
        UpdateAllSlotsUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"{GoldManager.Instance.CurrentGold}G";
        }
        UpdateButtonState();
    }

    private void UpdateAllSlotsUI()
    {
        foreach (var slot in upgradeSlots)
        {
            if (slot.slotLevelText != null)
            {
                var upgrade = StatManager.Instance.GetPermanentUpgrade(slot.statType);
                if (upgrade != null)
                {
                    slot.slotLevelText.text = $"{upgrade.Level}";
                }
            }
        }
    }

    private void Back()
    {
        UIManager.Instance.Pop();
    }

    [System.Serializable]
    public struct UpgradeSlot
    {
        public Button button;
        public StatType statType;
        public TextMeshProUGUI slotLevelText;
    }
}