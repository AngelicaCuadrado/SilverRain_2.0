using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWindow : UIWindow
{
    [Header("Buttons")]
    [SerializeField, Tooltip("")]
    private Button backButton;
    [SerializeField, Tooltip("")]
    private Button nextButton;
    [SerializeField, Tooltip("")]
    private Button previousButton;

    [Header("Panels")]
    [SerializeField, Tooltip("")]
    private List<GameObject> panels;
    [SerializeField,Tooltip("")]
    private int panelIndex = 0;

    private void Start()
    {
        // Initialize the first panel and button states
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == panelIndex);
        }
        previousButton.gameObject.SetActive(false); // Disable previous button on the first panel
        if (panels.Count <= 1)
        {
            nextButton.gameObject.SetActive(false); // Disable next button if there's only one panel
        }
    }

    public override void OnPushed()
    {
        BindUIEvents();
    }

    public override void OnPopped()
    {
        UnbindUIEvents();
    }

    private void BindUIEvents()
    {
        backButton.onClick.AddListener(Back);
        nextButton.onClick.AddListener(Next);
        previousButton.onClick.AddListener(Previous);
    }

    private void UnbindUIEvents()
    {
        backButton.onClick.RemoveListener(Back);
        nextButton.onClick.RemoveListener(Next);
        previousButton.onClick.RemoveListener(Previous);
    }

    private void Back()
    {
        UIManager.Instance.Pop();
    }

    public void Next()
    {
        panels[panelIndex].SetActive(false);
        panelIndex++;
        panels[panelIndex].SetActive(true);
        if (panelIndex == panels.Count - 1)
        {
            // Deactivate the button if we're on the last panel
            nextButton.gameObject.SetActive(false);
        }
        // Activate the previous button if we're past the first panel
        previousButton.gameObject.SetActive(true);
    }
    public void Previous()
    {
        panels[panelIndex].SetActive(false);
        panelIndex--;
        panels[panelIndex].SetActive(true);
        // Deactivate the button if we're on the first panel
        if (panelIndex == 0)
        {
            previousButton.gameObject.SetActive(false);
        }
        // Activate the next button if we're before the last panel
        nextButton.gameObject.SetActive(true);
    }
}