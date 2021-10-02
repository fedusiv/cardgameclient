using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cards
{
    public class CardDeck
    {
        public readonly List<CardDataActive> cardActiveList = new List<CardDataActive>();
        public readonly List<CardDataCreature> cardCreatureList = new List<CardDataCreature>();
        public readonly List<CardDeck> decks = new List<CardDeck>();    // This field is only for client's global deck information. Because inside stored playable decks
        public string name; // Name of deck
        private Queue<CardDataActive> cardQueue;

        public void PrepareCardQueue()
        {
            var listOfThings = cardActiveList.OrderBy(i => Guid.NewGuid()).ToList();
            cardQueue = new Queue<CardDataActive>(listOfThings);
        }

        public CardDataActive TakeCardFromUp()
        {
            return cardQueue.Dequeue();
        }

        public int AmountCardsInDeckQueue()
        {
            return cardQueue.Count;
        }

        public int AmountCardsInDeck()
        {
            return cardActiveList.Count + cardCreatureList.Count;
        }
    }
}