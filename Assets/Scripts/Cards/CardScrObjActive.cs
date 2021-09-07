using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "New Active Card", menuName = "Cards/Active")]
    public class CardScrObjActive : ScriptableObject
    {
        public CardDataActive data;
    }
    [CreateAssetMenu(fileName = "New Creature Card", menuName = "Cards/Creature")]
    public class CardScrObjCreature : ScriptableObject
    {
        public CardDataCreature data;
    }
}
