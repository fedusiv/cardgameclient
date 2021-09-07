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
        [SerializeField] protected Text cardPriceUI;
        [HideInInspector] public CardData cardData;
        [HideInInspector] public CardDataActive cardDataActive;
        [HideInInspector] public CardDataCreature cardDataCreature;
        [HideInInspector] public CardLocationType locationType;
        [HideInInspector] public int cardId;
        [HideInInspector] public int cardIdInLocation;
        #endregion
        #region EnlargeFields
        [SerializeField] protected float enlargeScaleFactor;
        [SerializeField] protected float enlargeYUpPoint;
        private int enlargePrevForegroundSiblingId;
        protected CardChangeTransformEnum enlargeStatus = CardChangeTransformEnum.Nothing;
        protected Vector3 enlargePosTarget; // To what need to change
        protected Vector3 enlargePosPrev;   // what was before the change
        protected Quaternion enlargeAngleTarget; // target angle to what need to rotate
        protected Quaternion enlargePrevAngle; // target angle to what need to rotate
        protected Vector3 enlargeScaleTarget; // To what need to change
        protected Vector3 enlargeScalePrev;   // what was before the change
        #endregion
        #region CardEffects
        [SerializeField] private Image selectionImage;
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
            // Get graveyard location
            graveyardObj = GameObject.Find("GraveyardPlace").GetComponent<Transform>();
        }

        private void Update()
        {
            if (enlargeStatus != CardChangeTransformEnum.Nothing)
            {
                ChangeCardTransform();  // enlarging and delarging
            }
        }

        #region CardDataOperations

        public virtual void SetCardData(CardDataActive data)
        {

        }
        public virtual void SetCardData(CardDataCreature data)
        {
            cardData = data;
            cardNameUI.text = cardData.name;
            cardPriceUI.text = cardData.price.ToString();
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
        protected virtual void ChangeCardTransform()
        { }
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
        EnemyZone
    }
}
