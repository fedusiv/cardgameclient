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

        public int AmountCardsInDeck()
        {
            return cardQueue.Count;
        }
    }
}