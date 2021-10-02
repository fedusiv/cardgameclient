using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class DeckElement : MonoBehaviour
    {
        [SerializeField] private Text deckName;
        [SerializeField] private Text buttonName;
        public Button button;
        public void SetDeckName(string name)
        {
            deckName.text = name;
        }

        public void MarkAsCreateNew()
        {
            deckName.text = "";
            buttonName.text = "create";
        }
    }
}