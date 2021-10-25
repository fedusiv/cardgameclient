using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cards
{
    public class CardObj : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler, IPointerUpHandler
    {
        protected enum CardChangeTransformEnum
        {
            Up,
            Down,
            SideMove,
            Nothing
        }

        #region CardDataFields
        [SerializeField] protected Text cardNameUI;
        [SerializeField] protected Text cardIdUI;
        [SerializeField] protected Text cardCostActionUI;
        [SerializeField] protected Text cardCostManaUI;
        [HideInInspector] public CardData cardData;
        [HideInInspector] public CardLocationType locationType;
        [HideInInspector] public int cardId;    // id in spawn location
        [HideInInspector] public int cardIdInLocation;
        #endregion
        #region EnlargeFields
        [SerializeField] protected float enlargeScaleFactor;
        [SerializeField] protected float enlargeYUpPoint;
        private int enlargePrevForegroundSiblingId;
        protected CardChangeTransformEnum enlargeStatus = CardChangeTransformEnum.Nothing;
        private Vector3 enlargePosTarget; // To what need to change
        private Vector3 enlargePosPrev;   // what was before the change
        private Quaternion enlargeAngleTarget; // target angle to what need to rotate
        private Quaternion enlargePrevAngle; // target angle to what need to rotate
        private Vector3 enlargeScaleTarget; // To what need to change
        private Vector3 enlargeScalePrev;   // what was before the change
        #endregion
        #region CardEffects
        [SerializeField] private Image selectionImage;
        [SerializeField] private Text amountInDeckText;
        [SerializeField] private GameObject cardTypeManaUIGroup;
        [SerializeField] private GameObject cardTypeSpellUIGroup;
        #endregion

        private Transform graveyardObj;
        
        #region EventsOnPointer
        [HideInInspector] private CardManipulationEvent onPointerEnter;
        [HideInInspector] private CardManipulationEvent onPointerExit;
        [HideInInspector] private CardManipulationEvent onPointerDown;
        [HideInInspector] private CardManipulationEvent onPointerUp;
        #endregion

        public void SetPointerEvents(CardManipulationEvent pointerEnter, CardManipulationEvent pointerExit,
            CardManipulationEvent pointerDown, CardManipulationEvent pointerUp)
        {
            onPointerEnter = pointerEnter;
            onPointerExit = pointerExit;
            onPointerDown = pointerDown;
            onPointerUp = pointerUp;
        }

        private void Start()
        {
            // Enlarge scale is always constant
            enlargeScalePrev = transform.localScale;
            enlargeScaleTarget = new Vector3(enlargeScalePrev.x * enlargeScaleFactor,
                enlargeScalePrev.y * enlargeScaleFactor, enlargeScalePrev.z);
            // Enlarge target angle is also always same
            enlargeAngleTarget = Quaternion.Euler(0, 0, 0);
        }
        
        // Additional method for initialization.
        // Because card obj handles card in different places
        public void InitStage(CardLocationType locType)
        {
            locationType = locType;
            if (locationType != CardLocationType.Library)
            {
                // Get graveyard location
                graveyardObj = GameObject.Find("GraveyardPlace").GetComponent<Transform>();
            }
        }

        private void Update()
        {
            if (enlargeStatus != CardChangeTransformEnum.Nothing)
            {
                ChangeCardTransform();  // enlarging and delarging
            }
        }

        #region CardDataOperations
        public void SetCardData(CardData data)
        {
            cardData = data;
            cardNameUI.text = cardData.name;
            cardIdUI.text = cardData.id.ToString();
            cardCostActionUI.text = cardData.cost_action.ToString();
            cardCostManaUI.text = cardData.cost_mana.ToString();
            switch (cardData.cardType)
            {
                case CardType.Mana:
                    cardTypeManaUIGroup.SetActive(true);
                    break;
                case CardType.Spell:
                    cardTypeSpellUIGroup.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        // Draw and represent all necessary staff for displaying to player amount available in deck
        public void SetAmountInDeck(int amount)
        {
            if (amount > 1)
            {
                amountInDeckText.text = "x" + amount.ToString();
                amountInDeckText.enabled = true;
            }
            else if(amount == 1)
            {
                amountInDeckText.enabled = false;
            }
            else
            {
                // amount less or equl 0
                amountInDeckText.text = "All used";
                amountInDeckText.enabled = true;
            }
        }
        #endregion
        
        #region IPointerHandler implementation
        public void OnPointerDown(PointerEventData data)
        {
            onPointerDown.Invoke(cardId);
        }
        public void OnPointerUp(PointerEventData data)
        {
            onPointerUp.Invoke(cardId);
        }
        public void OnPointerEnter(PointerEventData data)
        {

            onPointerEnter.Invoke(cardId);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit.Invoke(cardId);
        }

        #endregion

        public void SetCardAsInteractable(bool status)
        {
            selectionImage.enabled = status;
        }
        public void MoveToGraveyard()
        {
            var t = transform;
            t.SetParent(graveyardObj);
            t.position = graveyardObj.position;
        }
        public  void EnlargeOnPointerEnter()
        {
            enlargeStatus = CardChangeTransformEnum.Up;
            enlargePrevForegroundSiblingId = transform.GetSiblingIndex();
            gameObject.transform.SetAsLastSibling();
        }
        public void EnlargeOnPointerExit()
        {
            enlargeStatus = CardChangeTransformEnum.Down;
            transform.SetSiblingIndex(enlargePrevForegroundSiblingId);
        }

        private void ChangeCardTransform()
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
        public void SetCardPosition(Vector3 newLocalPos, Quaternion newRotation)
        {
            // Target angle
            enlargePrevAngle = newRotation;
            // Target position
            enlargePosPrev = newLocalPos;
            enlargePosTarget = new Vector3(newLocalPos.x, enlargeYUpPoint, newLocalPos.z);
            // Set status to update
            enlargeStatus = CardChangeTransformEnum.SideMove;
        }
    }

    public enum CardLocationType
    {
        None,
        PlayerHand,
        PlayerZone,
        EnemyHand,
        EnemyZone,
        Library
    }
}
