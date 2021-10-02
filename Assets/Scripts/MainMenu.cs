using System;
using System.Collections.Generic;
using Cards;
using UIScripts;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private OperationsQueue internalQueue;
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
    [SerializeField] private Transform cardsSpawnPoint; // point where cards firstly spawned, and where it's located if is not represented
    [SerializeField] private CardObjActive cardObjActivePref;
    [SerializeField] private CardObjCreature cardObjCreaturePref;
    [SerializeField] private Text libraryCurrentPageText;
    [SerializeField] private Button libraryPrevPageButton;
    [SerializeField] private Button libraryNextPageButton;
    private bool deckManagerEnabled = false;
    private CardDeck clientFullDeck;
    private readonly  List<CardObj> spawnedCardObjects = new List<CardObj>();
    private const int amountOfCardOnOnePage = 12;
    private int currentPageId, amountOfPages;   // page id and amount of all pages for current tab or any other filter stuff
    private List<CardObj> displayedCards = new List<CardObj>(); // list of cards, which are displayed
    #endregion

    #region DecksLibrary
    [SerializeField] private Transform decksNamesListTransformPoint; // point where to attach list elements
    [SerializeField] private GameObject deckElementPref;    // prefab of deck element
    private List<DeckElement> displayedDeckElements = new List<DeckElement>();    // stores game objects, that was created to represent player's decks
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
    

    private void OnDeckManagerButtonClick()
    {
        deckManagerEnabled = !deckManagerEnabled;
        deckManagerMenu.SetActive(deckManagerEnabled);
        if (deckManagerEnabled)
        {
            currentPageId = 0;  // Start with zero page Id
            // operation on enabling deck manager
            FillDeckLibrary();
            FillDeckNames();
        }
    }

    // Set client's full deck. To know what to represent and what to operate with 
    public void UpdateClientFullDeck(CardDeck deck)
    {
        clientFullDeck = deck;
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
            card.transform.SetParent(cardsSpawnPoint,false);
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
            card.SetAmountInDeck(card.cardData.amountInDeck);
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
        var currentId = 0;
        foreach (var deck in clientFullDeck.decks)
        {
            var deckElement = Instantiate(deckElementPref, decksNamesListTransformPoint, false).GetComponent<DeckElement>();
            deckElement.SetDeckName(deck.name);
            currentId = displayedDeckElements.Count;
            deckElement.button.onClick.AddListener(delegate { OnDeckNameButtonPressed(currentId); });
            displayedDeckElements.Add(deckElement);
        }
        // Create additional element to create new deck
        var newDeckElement = Instantiate(deckElementPref, decksNamesListTransformPoint, false).GetComponent<DeckElement>();
        newDeckElement.MarkAsCreateNew();
        currentId = displayedDeckElements.Count;
        newDeckElement.button.onClick.AddListener(delegate { OnDeckNameButtonPressed(currentId); });
        displayedDeckElements.Add(newDeckElement);
    }
    
    private void SpawnAllCardObjects()
    {
        foreach (var active in clientFullDeck.cardActiveList)
        {
            var obj = Instantiate(cardObjActivePref, cardsSpawnPoint, true);
            obj.SetCardData(active);
            obj.InitStage(CardLocationType.Library);
            spawnedCardObjects.Add(obj);
            obj.cardId = spawnedCardObjects.Count;
            obj.SetPointerEvents(cardOnPointerEnter,cardOnPointerExit, cardOnPointerDown, cardOnPointerUp);
        }
        foreach (var creature in clientFullDeck.cardCreatureList)
        {
            var obj = Instantiate(cardObjCreaturePref, cardsSpawnPoint, true);
            obj.SetCardData(creature);
            obj.InitStage(CardLocationType.Library);
            spawnedCardObjects.Add(obj);
            obj.cardId = spawnedCardObjects.Count;
            obj.SetPointerEvents(cardOnPointerEnter,cardOnPointerExit, cardOnPointerDown, cardOnPointerUp);
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
        Debug.Log(id);
    }
    #region PointerHandlers
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
