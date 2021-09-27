// Class to store data about player

using System.Collections.Generic;
using Cards;

public class ClientData
{
    public string loginName { get; private set; }
    public string uuidString { get; private set; }
    public Dictionary<int, int> cardInfoDictionary { get; private set; } // variable to store text data about player's cards.
    public CardDeck clientFullDeck { get; private set; }
    public ClientData(string uuid)
    {
        uuidString = uuid;
    }

    public void UpdateClientData(string login, Dictionary<int, int> cardDict)
    {
        loginName = login;
        cardInfoDictionary = cardDict;
    }

    public void UpdateClientFullCardDeck(CardDeck deck)
    {
        clientFullDeck = deck;
    }
}