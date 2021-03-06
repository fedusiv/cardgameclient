using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using GameLogic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SessionManager : MonoBehaviour
{
    [SerializeField] private CardsManipulationHandler cardsManipulationHandler;
    private CardPool cardPool;
    private void Start()
    {
        cardPool = GameObject.Find("CardPool").GetComponent<CardPool>();
        InstantiatePlayerDeck();    // Prepare information for card deck
        cardsManipulationHandler.PrepareDeskToStart();  // start
    }

    private void Update()
    {

    }

    private void InstantiatePlayerDeck()
    {
        // Session will receive two list, shuffled.
        // list 1 - creatures.
        // list 2 - active cards.
        // Here is kind of fake of it.
        var playerDeck = new CardDeck();
        // for (var i = 0; i < 20; i++)
        // {
        //     var uid = Random.Range(1, 6);
        //     var cardData = cardPool.GetCardData(uid);
        //     playerDeck.cardActiveList.Add(cardData);
        // }
        // for (var i = 0; i < 6; i++)
        // {
        //     var uid = Random.Range(7, 12);
        //     var cardData = cardPool.GetCardCreatureData(uid);
        //     playerDeck.cardCreatureList.Add(cardData);
        // }
        // // Now let's fake opponent's cards
        // var enemyDeck = new CardDeck();
        // for (var i = 0; i < 20; i++)
        // {
        //     var uid = Random.Range(1, 6);
        //     var cardData = cardPool.GetCardData(uid);
        //     enemyDeck.cardActiveList.Add(cardData);
        // }
        // for (var i = 0; i < 6; i++)
        // {
        //     var uid = Random.Range(7, 12);
        //     var cardData = cardPool.GetCardCreatureData(uid);
        //     enemyDeck.cardCreatureList.Add(cardData);
        // }
        //
        // enemyDeck.PrepareCardQueue();
        // playerDeck.PrepareCardQueue();
        // cardsManipulationHandler.SetCardDeck(playerDeck,enemyDeck);
    }

   
}
