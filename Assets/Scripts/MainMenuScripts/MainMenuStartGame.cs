using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenuScripts
{
    // Responsible to for start game management
    public class MainMenuStartGame : MonoBehaviour
    {
        [SerializeField] private Button startGameMenuButton;

        private void Awake()
        {
            startGameMenuButton.onClick.AddListener(OnStartGameMenuButtonPressed);
        }

        private void OnStartGameMenuButtonPressed()
        {
            Debug.Log("Start Game");
        }

        public void DisableStartGameButton()
        {
            
        }
    }
}