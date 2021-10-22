using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class CardPool : MonoBehaviour
    {
        [SerializeField] private List<CardScrObj> cardActivePool;
        private List<CardData> cardDataList = new List<CardData>();

        private void Awake()
        {
            RefreshDataList();
        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject); // Once it's instantiated we will keep card pool
        }
        public CardData GetCardData(int id)
        {
            var data=  cardDataList.Find(x => x.id.Equals(id));
            return data;
        }
        public void RefreshDataList()
        {
            foreach (var card in cardActivePool)
            {
                cardDataList.Add(card.data);
            }
        }

        public CardDeck CreatePlayerCardDeck(Dictionary<int, int> cardDict, List<Dictionary<int, int>> decksDict, List<string> decksNames)
        {
            // First create client's main deck aka card library.
            var cardDeck = CreateCardDeck(cardDict);

            // After library created, let's parse decks
            var elementsAmount = decksDict.Count;   // They should be equal
            for (var i = 0; i < elementsAmount; i++)
            {
                var deck = CreateCardDeck(decksDict[i]);
                deck.name = decksNames[i];
                cardDeck.decks.Add(deck);
            }

            return cardDeck;
        }

        private CardDeck CreateCardDeck(Dictionary<int, int> cardDict)
        {
            var cardDeck = new CardDeck();
            foreach (var key in cardDict.Keys)
            {
                // First try to find from active pool, because active pool is bigger
                var cardData = cardDataList.Find(x => x.id.Equals(key));
                if (cardData != null)
                {
                    // Found
                    cardData.amountInDeck = cardDict[key];
                    cardDeck.cardDataList.Add(cardData);
                }
            }

            return cardDeck;
        }

    }
}