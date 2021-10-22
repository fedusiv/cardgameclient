using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Cards")]
    public class CardScrObj : ScriptableObject
    {
        public CardData data;
    }
}
