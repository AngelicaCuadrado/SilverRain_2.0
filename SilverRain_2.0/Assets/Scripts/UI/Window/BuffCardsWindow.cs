using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffCardsWindow : UIWindow
{
    [Header("Buff Card Settings")]
    [SerializeField, Tooltip("A list of all buff cards")]
    private List<BuffCard> buffCards = new();

    
    [Header("Common")]
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button closeButton;
    
    private object _pauseToken;
    private object _inputToken;

    public override void OnPushed()
    {
        _pauseToken = PauseManager.Instance.Acquire("BuffCard");
        _inputToken = InputManager.Instance.Acquire(InputMode.UI, "BuffCard");

        DisplayCards();
        BindUIEvents();
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
        
        UnbindUIEvents();
    }
    
    private void DisplayCards()
    {
        List<TemporaryBuff> choices = BuffCardManager.Instance.CurrentChoices;

        for (int i = 0; i < buffCards.Count; i++)
        {
            if (i < choices.Count)
            {
                buffCards[i].SetupCard(choices[i]);
                buffCards[i].gameObject.SetActive(true);
            }
            else
            {
                buffCards[i].gameObject.SetActive(false);
            }
        }
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
        if (refreshButton != null) refreshButton.onClick.AddListener(RefreshCards);
        if (closeButton != null) closeButton.onClick.AddListener(CloseCards);
    }

    private void UnbindUIEvents()
    {
        if (refreshButton != null) refreshButton.onClick.RemoveListener(RefreshCards);
        if (closeButton != null) closeButton.onClick.RemoveListener(CloseCards);
    }

    private void RefreshCards()
    {
        // TODO: refresh cards logic if needed
    }

    private void CloseCards()
    {
        UIManager.Instance.Pop();
    }
}
