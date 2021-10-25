using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cards
{
    public class CardDeck
    {
        public readonly List<CardData> cardDataList = new List<CardData>(); // information about cards inside deck
        public readonly Dictionary<CardData, int> cardAmountInDeck = new Dictionary<CardData, int>();   // dictionary to hold information about amount in deck
        public readonly List<CardDeck> decks = new List<CardDeck>();    // This field is only for client's global deck information. Because inside stored playable decks
        public string name; // Name of deck
        private Queue<CardData> cardQueue;

        public void PrepareCardQueue()
        {
            var listOfThings = cardDataList.OrderBy(i => Guid.NewGuid()).ToList();
            cardQueue = new Queue<CardData>(listOfThings);
        }

        public CardData TakeCardFromUp()
        {
            return cardQueue.Dequeue();
        }

        public int AmountCardsInDeckQueue()
        {
            return cardQueue.Count;
        }

        public int AmountCardsInDeck()
        {
            return cardDataList.Count;
        }
    }
}