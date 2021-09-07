using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class CardObjActive : CardObj
    {
        [SerializeField] private Text damageTextUI;
        [SerializeField] private Image activeTypeImage;
        [SerializeField] private Sprite weaponTypeImage;
        [SerializeField] private Sprite spellTypeImage;
        [SerializeField] private Sprite shieldTypeImage;
        #region CardDataOperations
        public override void SetCardData(CardDataActive data)
        {
            cardDataActive = data;
            cardData = data;
            cardNameUI.text = cardData.name;
            cardPriceUI.text = cardData.price.ToString();
            damageTextUI.text = data.damage.ToString();
            switch (data.cardType)
            {
                case CardType.Spell:
                    activeTypeImage.sprite = spellTypeImage;
                    break;
                case CardType.Weapon:
                    activeTypeImage.sprite = weaponTypeImage;
                    break;
                case CardType.Shield:
                    activeTypeImage.sprite = shieldTypeImage;
                    break;
            }
        }
        #endregion
        
        protected override void ChangeCardTransform()
        {
            // This function is called from Update.
            var curTransform = gameObject.transform;
            var localPos = curTransform.localPosition;
            var localScale = curTransform.localScale;
            var localRotation = curTransform.rotation;
            var deltaTime = Time.deltaTime * 10;
            Vector3 scaleTarget, posTarget;
            Quaternion angleTarget;
            if (enlargeStatus == CardChangeTransformEnum.Up)
            {
                scaleTarget = enlargeScaleTarget;
                angleTarget = enlargeAngleTarget;
                posTarget = enlargePosTarget;
            }
            else// if(enlargeStatus == CardChangeTransformEnum.Down)
            {
                scaleTarget = enlargeScalePrev;
                angleTarget = enlargePrevAngle;
                posTarget = enlargePosPrev;
                // Check if already get to the point need do nothing
                if (localRotation == angleTarget && localScale == scaleTarget && localPos == posTarget)
                {
                    enlargeStatus = CardChangeTransformEnum.Nothing;
                    return; // Exit function
                }
            }

            // Increase card above to display player
            transform.localScale = Vector3.Lerp(localScale, scaleTarget, deltaTime);
            // Update angle of card
            transform.rotation = Quaternion.Lerp(localRotation, angleTarget, deltaTime);
            // Move card above on the screen
            transform.localPosition = Vector3.Lerp(localPos, posTarget, deltaTime);
        }
    }
}