using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button deckManagerButton;
    [SerializeField] private GameObject deckManagerMenu;
    private bool deckManagerEnabled = false;
    
    private void Start()
    {
        deckManagerButton.onClick.AddListener(OnDeckManagerButtonClick);
    }

    private void OnDeckManagerButtonClick()
    {
        deckManagerEnabled = !deckManagerEnabled;
        deckManagerMenu.SetActive(deckManagerEnabled);
    }
}
