using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using GameLogic;

namespace Cards
{
    public class CardsManipulationHandler : MonoBehaviour
    {
        #region Parameters
        [SerializeField] private GameObject cardActiveObjectPref;
        [SerializeField] private GameObject cardCreatureObjectPref;
        [SerializeField] private GameObject cardEnemyHandObjectPref;
        [SerializeField] private Transform playerCardZone;
        [SerializeField] private Transform playerCardZoneSpawnPoint;
        [SerializeField] private RectTransform playerPlayZone;
        [SerializeField] private Transform playerPlayZoneSpawnPoint;
        [SerializeField] private RectTransform enemyPlayZone;
        [SerializeField] private Transform enemyPlayZoneSpawnPoint;
        [SerializeField] private RectTransform enemyHandZone;
        [SerializeField] private Transform enemyHandZoneSpawnPoint;

        public float spacingDistance;
        public float spacingAngle;
        public float rotationAngleDefault;
        [SerializeField] private GameObject cardCursorLinePref;
        #endregion
        [SerializeField] private Camera mainCamera;

        [SerializeField] private Text playerDeckAmount;
        [SerializeField] private Text enemyDeckAmount;
        private readonly List<CardObj> cardList = new List<CardObj>();  // List of card currently on the whole desk
        private readonly List<CardObjActive> cardHandList = new List<CardObjActive>();  // Cards in Player's Hand
        private readonly List<CardObjCreature> cardPlayerList = new List<CardObjCreature>();  // Cards in Player Play zone list
        private readonly List<CardObjActive> cardEnemyHandList = new List<CardObjActive>();  // Cards in Player's Hand
        private readonly List<CardObjCreature> cardEnemyPlayList = new List<CardObjCreature>();  // Cards in Enemy's play zone list
        private CardDeck cardDeck;  // Card deck for current game session
        private CardDeck enemyCardDeck; // Card deck of player's opponent
        
        #region CardPointerAndSelection
        private CardCursor cardCursor;
        private bool isCardSelected = false;
        private readonly CardManipulationEvent cardOnPointerEnter = new CardManipulationEvent();
        private readonly CardManipulationEvent cardOnPointerExit = new CardManipulationEvent();
        private readonly CardManipulationEvent cardOnPointerDown = new CardManipulationEvent();
        private readonly CardManipulationEvent cardOnPointerUp = new CardManipulationEvent();
        private int cardSelectedId;
        private bool cardSelectedPressed = false;   // Mark true, when player pressed on card, means he wants to make some manipulation with it
        private bool cardSelectionCursorNeedInstantiate = false;   // Mark true when card cursor was instantiated
        private int cardInteractableId; // with what card player want to interact
        private bool cardInteractableSelected = false;  // player pointed to any card, and this card can be interactable
        #endregion

        private void Start()
        {
            InitEvents();
        }

        private void Update()
        {
            // Process cursor instantiation
            if (cardSelectedPressed)
            {
                if (cardSelectionCursorNeedInstantiate)
                {
                    CallCardOperationalCursor();
                }
            }
        }

        #region CursorManipulationRegion
        private void CallCardOperationalCursor()
        {
            if (cardCursor != null)
            {
                // Do not need to instantiate a new cursor
                return;
            }
            // Instantiate cursor
            cardSelectionCursorNeedInstantiate = false; // No need to instantiate more cursor
            var spawnPos = cardHandList[cardList[cardSelectedId].cardIdInLocation].transform.position;
            var obj = Instantiate(cardCursorLinePref,spawnPos,Quaternion.identity);
            cardCursor = obj.GetComponent<CardCursor>();
        }
        private void DestroyCardOperationalCursor()
        {
            cardCursor.DisableLine();
            Destroy(cardCursor.gameObject);
        }
        #endregion
        #region PoiterHandlers
        private void InitEvents()
        {
            cardOnPointerEnter.AddListener(CardOnPointerEnterHandler);
            cardOnPointerExit.AddListener(CardOnPointerExitHandler);
            cardOnPointerDown.AddListener(CardOnPointerDownHandler);
            cardOnPointerUp.AddListener(CardOnPointerUpHandler);
        }
        private void CardOnPointerEnterHandler(int cardId)
        {
            var curObj = cardList[cardId];
            if (!isCardSelected)
            {
                isCardSelected = true;
                cardSelectedId = cardId;
                if (curObj.locationType == CardLocationType.PlayerHand)
                {
                    cardHandList[curObj.cardIdInLocation].EnlargeOnPointerEnter();
                }
            }
            else
            {
                // card is selected, means, we want to interact with other card. Call visual to represent to player
                // that he can interact with it
                if (InteractChecker.IsInteractable(cardList[cardSelectedId].cardData.cardType,
                    curObj.cardData.cardType))
                {
                    cardInteractableId = cardId;
                    CardInteractionEffectsManipulation(curObj, true);
                }
            }
        }
        private void CardOnPointerExitHandler(int cardId)
        {
            var cardObj = cardList[cardId];
            if (isCardSelected)
            {
                // if card is selected, need to understand, from what card player is exiting
                if (cardId == cardSelectedId)
                {
                    // players exits from card, what was selected
                    if (cardSelectedPressed)
                    {
                        // check if this is player hand card
                        if (cardObj.locationType != CardLocationType.PlayerHand)
                        {
                            return; // Not player's hand, exit
                        }

                        // If card was pressed, means player want to interact with it, and we catched the movement out of card
                        cardSelectionCursorNeedInstantiate = true;
                    }
                    else
                    {
                        // Card was not pressed, just unselected it
                        isCardSelected = false;
                        switch (cardObj.locationType)
                        {
                            case CardLocationType.PlayerHand:
                                // Card is on player's hand, decrease size
                                cardHandList[cardObj.cardIdInLocation].EnlargeOnPointerExit();
                                break;
                        }

                    }
                }
                else
                {
                    // player is trying to exit from other card
                    if (cardId == cardInteractableId && cardInteractableSelected)
                    {
                        // player exits from card, which can be interactable.
                        // Disable effects
                        CardInteractionEffectsManipulation(cardList[cardId], false);
                    }
                }
            }

        }
        private void CardOnPointerDownHandler(int cardId)
        {
            if (cardId != cardSelectedId)
            {
                return;
            }
            // check if this is player hand card
            if (cardList[cardId].locationType != CardLocationType.PlayerHand)
            {
                return; // Not player's hand, exit
            }
            // Call cursor
            cardSelectedPressed = true;
        }
        private void CardOnPointerUpHandler(int cardId)
        {
            // Pointer up handler depends on pointer down, so cardId there will be always id on pointer down card
            // To be more precisely let's operate with id of selection and etc 
            // Remove all selection and all cursors
            cardSelectedPressed = false;    // Player clicked
            isCardSelected = false; // No selection anymore
            switch (cardList[cardSelectedId].locationType)
            {
                case CardLocationType.PlayerHand:
                    // Player's hand. Call visuals and remove cursor
                    DestroyCardOperationalCursor();
                    cardHandList[cardList[cardId].cardIdInLocation].EnlargeOnPointerExit();    // Call to make some visual process with Card
                    // if we can interact need to run interact process
                    if (cardInteractableSelected)
                    {
                        // Temporary solution
                        RemoveCardFromPlayerHand(cardSelectedId);
                        CardInteractionEffectsManipulation(cardList[cardInteractableId], false);
                    }
                    break;
            }
        }
        #endregion

        private void CardInteractionEffectsManipulation(CardObj curObj,bool status)
        {
            // manipulate with effects for interactable target card
            curObj.SetCardAsInteractable(status); // enable interaction effect
            cardCursor.SetCursorAsInteractable(status);   // enable cursor effect
            cardInteractableSelected = status;  // set the status of interactable
        }
        
        
        public void SetCardDeck(CardDeck playerDeck, CardDeck enemyDeck)
        {
            cardDeck = playerDeck;
            enemyCardDeck = enemyDeck;
            playerDeckAmount.text = cardDeck.AmountCardsInDeck().ToString();
        }

        public void PrepareDeskToStart()
        {
            for (var i = 0; i < 5; i++)
            {
                AddCardToHand();
            }
            foreach (var cData in cardDeck.cardCreatureList)
            {
                AddCardToPlayerPlayZone(cData);
            }

            for (var i = 0; i < 5; i++)
            {
                AddCardToEnemyHand();
            }
            foreach (var cData in enemyCardDeck.cardCreatureList)
            {
                AddCardToEnemyPlayZone(cData);
            }
        }

        #region AddRemoveCard and Redraw Operations
        private void RemoveCardFromPlayerHand(int id)
        {
            var cardObj = cardList[id];
            cardHandList.RemoveAt(cardObj.cardIdInLocation);
            cardObj.MoveToGraveyard();
            // recalculate cards id in hand zone location
            for (var i = 0; i < cardHandList.Count; i++)
            {
                cardHandList[i].cardIdInLocation = i;
            }
            ReDrawCardsInHand(CardLocationType.PlayerHand);
        }
        
        private void AddCardToHand()
        {
            // Create object
            var card = Instantiate(cardActiveObjectPref, playerCardZoneSpawnPoint.position, Quaternion.identity);
            card.transform.SetParent(playerCardZone);
            var cardObj = card.GetComponent<CardObjActive>();
            AttachCardToList(cardObj);
            cardObj.SetPointerEvents(cardOnPointerEnter,cardOnPointerExit, cardOnPointerDown, cardOnPointerUp); // attach pointers to card
            // Take data
            var cardInfo = cardDeck.TakeCardFromUp();
            cardObj.SetCardData(cardInfo);
            playerDeckAmount.text = cardDeck.AmountCardsInDeck().ToString();
            ReDrawCardsInHand(CardLocationType.PlayerHand);
        }
        private void ReDrawCardsInHand(CardLocationType locationType)
        {
            List<CardObjActive> handList;
            float leftAngle, rightAngle, angleDefault;
            int angleKoef;
            Vector3 spawnPointPos;
            if (locationType == CardLocationType.PlayerHand)
            {
                spawnPointPos = playerCardZoneSpawnPoint.localPosition;
                handList = cardHandList;
                leftAngle = (spacingAngle + 180) * Mathf.Deg2Rad;
                rightAngle = (spacingAngle * -1) * Mathf.Deg2Rad;
                angleKoef = 1;
            }
            else
            {
                spawnPointPos = enemyHandZoneSpawnPoint.localPosition;
                handList = cardEnemyHandList;
                leftAngle = (180 - spacingAngle) * Mathf.Deg2Rad;
                rightAngle = (spacingAngle) * Mathf.Deg2Rad;
                angleKoef = -1;
            }
            // Calculate new default position for a card
            var amount = handList.Count;
            int amountHalf = amount / 2;
            var addHalf = 0;
            if (amount % 2 == 1)
            {
                // Amount is odd. Middle card, should be in the center
                handList[amountHalf].SetCardPosition(spawnPointPos,Quaternion.Euler(0,0,0) );
                addHalf = 1;
            }

            float rotationAngle = 0;
            var radiusCommon = spacingDistance * (Mathf.Cos(spacingAngle * Mathf.Deg2Rad));
            // Left side
            for (var i = amountHalf - 1; i >=0; i--)
            {
                var radius = radiusCommon * (amountHalf - i);
                var x = radius * Mathf.Cos(leftAngle) + spawnPointPos.x;
                var y = radius * Mathf.Sin(leftAngle) + spawnPointPos.y;
                rotationAngle = angleKoef * (rotationAngleDefault + amountHalf - i);
                handList[i].SetCardPosition(new Vector3(x, y, 0),Quaternion.Euler(0,0,rotationAngle) );
            }
            // Right side
            for (var i = amountHalf+ addHalf; i < amount; i++)
            {
                var radius = radiusCommon * (i - amountHalf);
                var x = radius * Mathf.Cos(rightAngle) + spawnPointPos.x;
                var y = radius * Mathf.Sin(rightAngle) + spawnPointPos.y;
                rotationAngle = angleKoef * (rotationAngleDefault + i - amountHalf);
                handList[i].SetCardPosition(new Vector3(x, y, 0),Quaternion.Euler(0,0,-rotationAngle) );
            }
        }
        private void AddCardToPlayerPlayZone(CardDataCreature data)
        {
            var card = Instantiate(cardCreatureObjectPref, playerPlayZoneSpawnPoint.position, Quaternion.identity);
            card.transform.SetParent(playerPlayZone);
            var cardObj = card.GetComponent<CardObjCreature>();
            AttachCardToList(cardObj); // give unique id in the session and attach also to required list
            cardObj.SetPointerEvents(cardOnPointerEnter,cardOnPointerExit, cardOnPointerDown, cardOnPointerUp);
            cardObj.SetCardData(data);
            RedrawCardsInPlayerZone();
        }
        private void RedrawCardsInPlayerZone()
        {
            var width = playerPlayZone.rect.width;
            var leftPosX = -(width / 2);
            var spacingSize = width / (cardPlayerList.Count + 1);
            var spacingSizeCounter = spacingSize;
            foreach (var cardObj in cardPlayerList)
            {
                var pos = new Vector3(leftPosX + spacingSizeCounter, playerPlayZoneSpawnPoint.localPosition.y, 0);
                cardObj.transform.localPosition = pos;
                spacingSizeCounter += spacingSize;
            }
        }
        private void AddCardToEnemyHand()
        {
            // Create object
            var card = Instantiate(cardEnemyHandObjectPref, enemyHandZoneSpawnPoint.position, Quaternion.identity);
            card.transform.SetParent(enemyHandZone);
            var cardObj = card.GetComponent<CardObjActive>();
            AttachCardToEnemyList(cardObj);
            cardObj.SetPointerEvents(cardOnPointerEnter,cardOnPointerExit, cardOnPointerDown, cardOnPointerUp); // attach pointers to card
            // Take data
            var cardInfo = enemyCardDeck.TakeCardFromUp();
            cardObj.SetCardData(cardInfo);
            enemyDeckAmount.text = enemyCardDeck.AmountCardsInDeck().ToString();
            ReDrawCardsInHand(CardLocationType.EnemyHand);
        }
        private void AddCardToEnemyPlayZone(CardDataCreature data)
        {
            var card = Instantiate(cardCreatureObjectPref, enemyPlayZoneSpawnPoint.position, Quaternion.identity);
            card.transform.SetParent(enemyPlayZone);
            var cardObj = card.GetComponent<CardObjCreature>();
            AttachCardToEnemyList(cardObj); // give unique id in the session and attach also to required list
            cardObj.SetPointerEvents(cardOnPointerEnter,cardOnPointerExit, cardOnPointerDown, cardOnPointerUp);
            cardObj.SetCardData(data);
            RedrawCardsInEnemyZone();
        }
        private void RedrawCardsInEnemyZone()
        {
            var width = enemyPlayZone.rect.width;
            var leftPosX = -(width / 2);
            var spacingSize = width / (cardPlayerList.Count + 1);
            var spacingSizeCounter = spacingSize;
            foreach (var cardObj in cardEnemyPlayList)
            {
                var pos = new Vector3(leftPosX + spacingSizeCounter, enemyPlayZoneSpawnPoint.localPosition.y, 0);
                cardObj.transform.localPosition = pos;
                spacingSizeCounter += spacingSize;
            }
        }
        
        private void AttachCardToList(CardObjActive cardObj)
        {
            cardObj.locationType = CardLocationType.PlayerHand;
            cardObj.cardId = cardList.Count;
            cardList.Add(cardObj);
            cardObj.cardIdInLocation = cardHandList.Count;
            cardHandList.Add(cardObj);
        }
        private void AttachCardToList(CardObjCreature cardObj)
        {
            cardObj.locationType = CardLocationType.PlayerZone;
            cardObj.cardId = cardList.Count;
            cardList.Add(cardObj);
            cardObj.cardIdInLocation = cardPlayerList.Count;
            cardPlayerList.Add(cardObj);
        }
        private void AttachCardToEnemyList(CardObjActive cardObj)
        {
            cardObj.locationType = CardLocationType.EnemyHand;
            cardObj.cardId = cardList.Count;
            cardList.Add(cardObj);
            cardObj.cardIdInLocation = cardEnemyHandList.Count;
            cardEnemyHandList.Add(cardObj);
        }
        private void AttachCardToEnemyList(CardObjCreature cardObj)
        {
            cardObj.locationType = CardLocationType.EnemyZone;
            cardObj.cardId = cardList.Count;
            cardList.Add(cardObj);
            cardObj.cardIdInLocation = cardEnemyPlayList.Count;
            cardEnemyPlayList.Add(cardObj);
        }
        #endregion
    }
}