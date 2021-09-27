using System;

namespace Cards
{
    [Serializable]
    public class CardData
    {
        public int id;  // unique card id in the whole game. Please be very careful with it
        public string name;
        public int price;
        public CardType cardType;
        public int amountInDeck;
    }

    [Serializable]
    public class CardDataActive : CardData
    {
        public int damage;
        public int defence;
        public int poison;
        public int duration;
    }
    [Serializable]
    public class CardDataCreature : CardData
    {
        public int damage;
        public int health;
    }
}