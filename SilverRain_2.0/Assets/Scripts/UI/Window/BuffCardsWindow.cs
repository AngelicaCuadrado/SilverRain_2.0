using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffCardsWindow : UIWindow
{
    [Header("Buff Card Settings")]
    [SerializeField, Tooltip("A list of all buff cards")]
    private List<BuffCard> buffCards = new();

    
    [Header("Common")]
    [SerializeField] private Button rerollButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMPro.TextMeshProUGUI rerollsAvailableText;
    [SerializeField] private TMPro.TextMeshProUGUI bansAvailableText;

    private object _pauseToken;
    private object _inputToken;

    public override void OnPushed()
    {
        _pauseToken = PauseManager.Instance.Acquire("BuffCard");
        _inputToken = InputManager.Instance.Acquire(InputMode.UI, "BuffCard");
        
        UIManager.Instance.Hide("HUD");

        BindUIEvents();
        DisplayCards();
    }
    
    public override void OnPopped()
    {
        HideCards();
        
        if (_pauseToken != null)
        {
            PauseManager.Instance.Release(_pauseToken);
            _pauseToken = null;
        }
        if (_inputToken != null)
        {
            InputManager.Instance.Release(_inputToken);
            _inputToken = null;
        }
        
        UIManager.Instance.UnHide("HUD");
        
        UnbindUIEvents();
    }
    
    private void DisplayCards()
    {
        List<TemporaryBuff> choices = BuffCardManager.Instance.CurrentChoices;
        int visibleCardCount = 0;

        for (int i = 0; i < buffCards.Count; i++)
        {
            if (i < choices.Count && choices[i] != null)
            {
                buffCards[i].SetupCard(choices[i]);
                buffCards[i].gameObject.SetActive(true);
                visibleCardCount++;
            }
            else
            {
                buffCards[i].gameObject.SetActive(false);
            }
        }

        Debug.Log($"BuffCardsWindow - Displaying {visibleCardCount} cards from {choices.Count} queued choices.");
    }

    private void HideCards()
    {
        foreach (var card in buffCards)
        {
            card.gameObject.SetActive(false);
        }
    }
    
    private void BindUIEvents()
    {
        if (rerollButton != null) rerollButton.onClick.AddListener(RerollCards);
        if (closeButton != null) closeButton.onClick.AddListener(CloseCards);
    }

    private void UnbindUIEvents()
    {
        if (rerollButton != null) rerollButton.onClick.RemoveListener(RerollCards);
        if (closeButton != null) closeButton.onClick.RemoveListener(CloseCards);
    }

    public void RerollCards()
    {
        BuffCardManager.Instance.RerollChoices();
    }

    public void CloseCards()
    {
        UIManager.Instance.Pop();
    }

    public void UpdateRerollsAvailable(int rerolls)
    {
        if (rerollsAvailableText != null)
        {
            rerollsAvailableText.text = $"Rerolls Available: {rerolls}";
        }
    }

    public void UpdateBansAvailable(int bans)
    {
        if (bansAvailableText != null)
        {
            bansAvailableText.text = $"Bans Available: {bans}";
        }
    }
}
