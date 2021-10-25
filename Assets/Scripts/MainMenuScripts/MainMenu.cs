using System;
using System.Collections.Generic;
using Cards;
using UIScripts;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenuScripts
{
    public class MainMenu : MonoBehaviour
    {
        private OperationsQueue internalQueue;
        private MainMenuStartGame mainMenuStartGame;    // reference to start game menu object

        #region Pointer handler field

        private readonly CardManipulationEvent cardOnPointerEnter = new CardManipulationEvent();
        private readonly CardManipulationEvent cardOnPointerExit = new CardManipulationEvent();
        private readonly CardManipulationEvent cardOnPointerDown = new CardManipulationEvent();
        private readonly CardManipulationEvent cardOnPointerUp = new CardManipulationEvent();

        #endregion

        #region CardLibrary

        [SerializeField] private Button deckManagerButton;
        [SerializeField] private GameObject deckManagerMenu;
        [SerializeField] private GridLayoutGroup deckLibraryGridUI;

        [SerializeField]
        private Transform
            cardsSpawnPoint; // point where cards firstly spawned, and where it's located if is not represented

        [SerializeField] private CardObj cardObjPref;
        [SerializeField] private Text libraryCurrentPageText;
        [SerializeField] private Button libraryPrevPageButton;
        [SerializeField] private Button libraryNextPageButton;
        private bool deckManagerEnabled = false;
        private CardDeck clientFullDeck;
        private CardDeck currentCardDeckToDisplay; // store reference to deck, which should be shown now
        private readonly List<CardObj> spawnedCardObjects = new List<CardObj>();
        private const int amountOfCardOnOnePage = 12;

        private int
            currentPageId, amountOfPages; // page id and amount of all pages for current tab or any other filter stuff

        private List<CardObj> displayedCards = new List<CardObj>(); // list of cards, which are displayed

        #endregion

        #region DecksLibrary

        // Decks list
        [SerializeField] private Transform decksNamesListTransformPoint; // point where to attach list elements
        [SerializeField] private GameObject deckElementPref; // prefab of deck element

        private List<DeckElement>
            displayedDeckElements =
                new List<DeckElement>(); // stores game objects, that was created to represent player's decks

        // Cards inside deck
        private int currentDeckOnEditIndex = -1;

        [SerializeField]
        private Transform cardsNamesListInDeckTransfromPoint; // point where represented cards in decks will be attache

        [SerializeField] private GameObject cardInDeckElementPref; // prefab of element which represents card in deck

        private List<DeckCardElement>
            displayedCardDeckElements = new List<DeckCardElement>(); // list of represented cards

        #endregion

        private void Awake()
        {
            internalQueue = OperationsQueue.Instance;
            var msg = new OperationMessage(OpCodes.MainMenuLoaded);
            internalQueue.PutIntoQueue(msg);
            deckManagerButton.onClick.AddListener(OnDeckManagerButtonClick);
            libraryPrevPageButton.onClick.AddListener(OnPrevPageButtonClick);
            libraryNextPageButton.onClick.AddListener(OnNextPageButtonClick);
        }

        private void Start()
        {
            mainMenuStartGame = GameObject.Find("MainMenuStartGame").GetComponent<MainMenuStartGame>();
        }

        #region DeckManagement

        private void OnDeckManagerButtonClick()
        {
            deckManagerEnabled = !deckManagerEnabled;
            deckManagerMenu.SetActive(deckManagerEnabled);
            if (deckManagerEnabled)
            {
                currentPageId = 0; // Start with zero page Id
                // operation on enabling deck manager
                FillDeckLibrary();
                FillDeckNames();
            }
        }

        // Set client's full deck. To know what to represent and what to operate with 
        public void UpdateClientFullDeck(CardDeck deck)
        {
            clientFullDeck = deck;
            currentCardDeckToDisplay = clientFullDeck;
            SpawnAllCardObjects();
        }

        private void FillDeckLibrary()
        {
            // Get list to proceed
            var listToOperate = spawnedCardObjects;

            // Calculate pages amount
            int modulo = listToOperate.Count % amountOfCardOnOnePage;
            amountOfPages = listToOperate.Count / amountOfCardOnOnePage;
            if (modulo > 0)
            {
                amountOfPages += 1;
            }

            // Remove previous spawned cards
            foreach (var card in displayedCards)
            {
                card.transform.SetParent(cardsSpawnPoint, false);
            }

            displayedCards.Clear();

            // Spawn cards
            var lastIdToSpawn = (amountOfCardOnOnePage * (currentPageId + 1));
            var possibleAmountToSpawn = listToOperate.Count - lastIdToSpawn;
            if (possibleAmountToSpawn < 0)
            {
                lastIdToSpawn += possibleAmountToSpawn;
            }

            for (int i = (amountOfCardOnOnePage * currentPageId);
                i < lastIdToSpawn;
                i++)
            {
                var card = listToOperate[i];
                card.transform.SetParent(deckLibraryGridUI.transform);
                if (currentDeckOnEditIndex == -1)
                {
                    // no deck chosen, just display full amount of cards
                    card.SetAmountInDeck(clientFullDeck.cardAmountInDeck[card.cardData]);
                }
                else
                {
                    card.SetAmountInDeck(clientFullDeck.cardAmountInDeck[card.cardData] -
                                         clientFullDeck.decks[currentDeckOnEditIndex]
                                             .cardAmountInDeck[card.cardData]); // display how many card available
                }

                displayedCards.Add(card);
            }

            // Change library navigation information
            FillLibraryNavigation();
        }

        // Print button and page number in library field
        private void FillLibraryNavigation()
        {
            var humanPageId = currentPageId + 1;
            libraryCurrentPageText.text = humanPageId.ToString() + "/" + amountOfPages.ToString();
            if (humanPageId == 1)
            {
                libraryPrevPageButton.gameObject.SetActive(false);
            }
            else
            {
                libraryPrevPageButton.gameObject.SetActive(true);
            }

            if (humanPageId == amountOfPages)
            {
                libraryNextPageButton.gameObject.SetActive(false);
            }
            else
            {
                libraryNextPageButton.gameObject.SetActive(true);
            }

        }

        // Print deck Names of client
        private void FillDeckNames()
        {
            // Remove previous instantiation
            foreach (var deck in displayedDeckElements)
            {
                Destroy(deck.gameObject);
            }

            displayedDeckElements.Clear();
            // Instantiate new decks list
            foreach (var deck in clientFullDeck.decks)
            {
                var deckElement = Instantiate(deckElementPref, decksNamesListTransformPoint, false)
                    .GetComponent<DeckElement>();
                deckElement.SetDeckName(deck.name);
                var currentId = displayedDeckElements.Count;
                deckElement.button.onClick.AddListener(delegate
                {
                    OnDeckNameButtonPressed(currentId);
                }); // please be very careful with id, because for callback it's hardcoded here
                displayedDeckElements.Add(deckElement);
            }

            // Create additional element to create new deck
            var newDeckElement = Instantiate(deckElementPref, decksNamesListTransformPoint, false)
                .GetComponent<DeckElement>();
            newDeckElement.MarkAsCreateNew();
            var newElementId = displayedDeckElements.Count;
            newDeckElement.button.onClick.AddListener(delegate
            {
                OnDeckNameButtonPressed(newElementId);
            }); // please be very careful with id, because for callback it's hardcoded here
            displayedDeckElements.Add(newDeckElement);
        }

        // Display, cards in the chosen deck, and allow to make changes
        private void FillDeckWithCards()
        {
            // Remove previous representation of cards
            foreach (var cardDeck in displayedCardDeckElements)
            {
                Destroy(cardDeck.gameObject);
            }

            displayedCardDeckElements.Clear();

            if (clientFullDeck.decks.Count <= currentDeckOnEditIndex)
            {
                // This is request to create new deck
            }
            else
            {
                // Edit already existing deck
                var deck = clientFullDeck.decks[currentDeckOnEditIndex]; // current deck
                // Display active card first
                foreach (var card in deck.cardDataList)
                {
                    var cardDeck = Instantiate(cardInDeckElementPref, cardsNamesListInDeckTransfromPoint, false)
                        .GetComponent<DeckCardElement>();
                    cardDeck.SetCardData(card.name, card.cost_mana, deck.cardAmountInDeck[card]);
                    var index = displayedCardDeckElements.Count;
                    cardDeck.removeButton.onClick.AddListener(delegate { OnCardInDeckRemoveButtonPressed(index); });
                    displayedCardDeckElements.Add(cardDeck);
                }
            }
        }

        private void SpawnAllCardObjects()
        {
            foreach (var card in clientFullDeck.cardDataList)
            {
                var obj = Instantiate(cardObjPref, cardsSpawnPoint, true);
                obj.SetCardData(card);
                obj.InitStage(CardLocationType.Library);
                spawnedCardObjects.Add(obj);
                obj.cardId = spawnedCardObjects.Count;
                obj.SetPointerEvents(cardOnPointerEnter, cardOnPointerExit, cardOnPointerDown, cardOnPointerUp);
            }
        }

        private void OnNextPageButtonClick()
        {
            currentPageId += 1;
            FillDeckLibrary();
        }

        private void OnPrevPageButtonClick()
        {
            currentPageId -= 1;
            FillDeckLibrary();
        }

        private void OnDeckNameButtonPressed(int id)
        {
            if (currentDeckOnEditIndex != id)
            {
                // Open new edit deck
                currentDeckOnEditIndex = id;
                FillDeckWithCards();
                displayedDeckElements[currentDeckOnEditIndex].MarkAsInEdit();
            }
            else
            {
                // Deck is already opened to edit. This button pressed to save
                displayedDeckElements[currentDeckOnEditIndex].MarkAsSaved();
                currentDeckOnEditIndex = -1;
            }

            FillDeckLibrary();
        }

        private void OnCardInDeckRemoveButtonPressed(int id)
        {
            Debug.Log("I want to edit card id: " + id);
        }

        #endregion // DeckManagement

        #region PointerHandlers of Cards in deck library

        private void InitEvents()
        {
            cardOnPointerEnter.AddListener(CardOnPointerEnterHandler);
            cardOnPointerExit.AddListener(CardOnPointerExitHandler);
            cardOnPointerDown.AddListener(CardOnPointerDownHandler);
            cardOnPointerUp.AddListener(CardOnPointerUpHandler);
        }

        private void CardOnPointerEnterHandler(int cardId)
        {

        }

        private void CardOnPointerExitHandler(int cardId)
        {

        }

        private void CardOnPointerDownHandler(int cardId)
        {

        }

        private void CardOnPointerUpHandler(int cardId)
        {

        }

        #endregion
    }
}