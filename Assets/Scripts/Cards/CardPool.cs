using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class CardPool : MonoBehaviour
    {
        [SerializeField] private List<CardScrObjActive> cardActivePool;
        [SerializeField] private List<CardScrObjCreature> cardCreaturePool;
        private List<CardDataActive> cardActiveData = new List<CardDataActive>();
        private List<CardDataCreature> cardCreatureData = new List<CardDataCreature>();

        private void Awake()
        {
            RefreshDataList();
        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject); // Once it's instantiated we will keep card pool
        }
        public CardDataActive GetCardActiveData(int id)
        {
            var data=  cardActiveData.Find(x => x.id.Equals(id));
            return data;
        }
        public CardDataCreature GetCardCreatureData(int id)
        {
            var data=  cardCreatureData.Find(x => x.id.Equals(id));
            return data;
        }

        public void RefreshDataList()
        {
            foreach (var card in cardActivePool)
            {
                cardActiveData.Add(card.data);
            }
            foreach (var card in cardCreaturePool)
            {
                cardCreatureData.Add(card.data);
            }
        }

        public CardDeck CreatePlayerCardDeck(Dictionary<int, int> cardDict, List<Dictionary<int, int>> decksDict, List<string> decksNames)
        {
            // First create client's main deck aka card library.
            var cardDeck = CreateCardDeck(cardDict, cardActiveData, cardCreatureData);

            // After library created, let's parse decks
            var elementsAmount = decksDict.Count;   // They should be equal
            for (var i = 0; i < elementsAmount; i++)
            {
                var deck = CreateCardDeck(decksDict[i], cardDeck.cardActiveList, cardDeck.cardCreatureList);
                deck.name = decksNames[i];
                cardDeck.decks.Add(deck);
            }

            return cardDeck;
        }

        private CardDeck CreateCardDeck(Dictionary<int, int> cardDict, List<CardDataActive> actPool,
            List<CardDataCreature> creaturePool)
        {
            var cardDeck = new CardDeck();
            foreach (var key in cardDict.Keys)
            {
                // First try to find from active pool, because active pool is bigger
                var dataAct = actPool.Find(x => x.id.Equals(key));
                if (dataAct != null)
                {
                    // Found
                    dataAct.amountInDeck = cardDict[key];
                    cardDeck.cardActiveList.Add(dataAct);
                    continue;
                }
                // Okay card is not from active pool
                var dataCre = creaturePool.Find(x => x.id.Equals(key));
                if (dataCre != null)
                {
                    // Found
                    dataCre.amountInDeck = cardDict[key];
                    cardDeck.cardCreatureList.Add(dataCre);
                }
            }

            return cardDeck;
        }

    }
}