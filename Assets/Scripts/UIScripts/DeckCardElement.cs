using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    // This is ui, that represents card inside chosen deck
    public class DeckCardElement : MonoBehaviour
    {
        [SerializeField] private Text cardName;
        [SerializeField] private Text cardPrice;
        [SerializeField] private Text cardAmount;
        public Button removeButton;

        public void SetCardData(string name, int cost_mana, int amount)
        {
            cardName.text = name;
            cardPrice.text = cost_mana.ToString();
            if (amount > 1)
            {
                cardAmount.text = "x" + amount;
            }
            else
            {
                cardAmount.text = "";
            }
        }

    }
}