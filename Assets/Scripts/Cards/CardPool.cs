using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class CardPool : MonoBehaviour
    {
        [SerializeField] private List<CardScrObjActive> cardActivePool;
        [SerializeField] private List<CardScrObjCreature> cardCreaturePool;
        
        private void Start()
        {
            DontDestroyOnLoad(this.gameObject); // Once it's instantiated we will keep card pool
        }
        public CardDataActive GetCardActiveData(int id)
        {
            var data=  cardActivePool.Find(x => x.data.id.Equals(id));
            return data.data;
        }
        public CardDataCreature GetCardCreatureData(int id)
        {
            var data=  cardCreaturePool.Find(x => x.data.id.Equals(id));
            return data.data;
        }

        public CardDeck CreatePlayerCardDeck(Dictionary<int, int> cardDict)
        {
            var cardDeck = new CardDeck();
            foreach (var key in cardDict.Keys)
            {
                // First try to find from active pool, because active pool is bigger
                var data_act=  cardActivePool.Find(x => x.data.id.Equals(key));
                if (data_act != null)
                {
                    // Found
                    data_act.data.amountInDeck = cardDict[key];
                    cardDeck.cardActiveList.Add(data_act.data);
                    continue;
                }
                // Okay card is not from active pool
                var data_cre=  cardCreaturePool.Find(x => x.data.id.Equals(key));
                if (data_cre != null)
                {
                    // Found
                    data_cre.data.amountInDeck = cardDict[key];
                    cardDeck.cardCreatureList.Add(data_cre.data);
                }
            }

            return cardDeck;
        }

    }
}