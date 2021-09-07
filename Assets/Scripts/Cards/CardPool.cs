using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class CardPool : MonoBehaviour
    {
        [SerializeField] private List<CardScrObjActive> cardActivePool;
        [SerializeField] private List<CardScrObjCreature> cardCreaturePool;

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

    }
}