using System;
using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button deckManagerButton;
    [SerializeField] private GameObject deckManagerMenu;
    [SerializeField] private GridLayoutGroup deckLibraryGridUI;
    [SerializeField] private Transform cardsSpawnPoint; // point where cards firstly spawned, and where it's located if is not represented
    [SerializeField] private CardObjActive cardObjActivePref;
    [SerializeField] private CardObjCreature cardObjCreaturePref;
    private bool deckManagerEnabled = false;
    private CardDeck clientFullDeck;
    private readonly  List<CardObj> spawnedCardObjects = new List<CardObj>();

    #region Pointer handler field
    private readonly CardManipulationEvent cardOnPointerEnter = new CardManipulationEvent();
    private readonly CardManipulationEvent cardOnPointerExit = new CardManipulationEvent();
    private readonly CardManipulationEvent cardOnPointerDown = new CardManipulationEvent();
    private readonly CardManipulationEvent cardOnPointerUp = new CardManipulationEvent();
    #endregion
    
    private void Start()
    {
        deckManagerButton.onClick.AddListener(OnDeckManagerButtonClick);
    }

    private void OnDeckManagerButtonClick()
    {
        deckManagerEnabled = !deckManagerEnabled;
        deckManagerMenu.SetActive(deckManagerEnabled);
        if (deckManagerEnabled)
        {
            FillDeckLibrary();
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
        foreach (var card in spawnedCardObjects)
        {
            card.transform.SetParent(deckLibraryGridUI.transform);
            card.SetAmountInDeck(card.cardData.amountInDeck);
        }
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
