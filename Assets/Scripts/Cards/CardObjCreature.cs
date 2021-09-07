using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class CardObjCreature : CardObj
    {
        [SerializeField] private Text damageTextUI;
        [SerializeField] private Text healthTextUI;
        public override void SetCardData(CardDataCreature data)
        {
            cardDataCreature = data;
            cardData = data;
            cardNameUI.text = cardData.name;
            cardPriceUI.text = cardData.price.ToString();
            damageTextUI.text = data.damage.ToString();
            healthTextUI.text = data.health.ToString();
        }
    }
}