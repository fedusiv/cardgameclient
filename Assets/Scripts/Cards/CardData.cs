using System;

namespace Cards
{
    [Serializable]
    public class CardData
    {
        public int id;  // unique card id in the whole game. Please be very careful with it
        public string name;
        public int cost_action;
        public int cost_mana;
        public CardType cardType;
    }
}