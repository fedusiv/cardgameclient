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
        public int displayedIndex;  // which index card has in displayed menu
        public void SetDeckName(string name)
        {
            deckName.text = name;
        }

        public void MarkAsCreateNew()
        {
            deckName.text = "";
            buttonName.text = "create";
        }

        public void MarkAsInEdit()
        {
            buttonName.text = "Save";
        }

        public void MarkAsSaved()
        {
            buttonName.text = "Edit";
        }
        
    }
}