using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Communication
{
    public class ServerInMessage
    {
        // Message came to client. Let's parse it
        public MessageType MsgType;
        private JSONNode jo, body;
        
        // Data fields of incoming messages
        public string uuid;
        public bool result;

        #region ClientData
        public string login;
        public Dictionary<int, int> cardDictionary;
        public List<Dictionary<int, int>> decksCardDictionary;
        public List<string> decksNames;
        #endregion

        public ServerInMessage(string message)
        {
            jo = JSON.Parse(message);
            MsgType = (MessageType)jo["type"].AsInt;
            body = jo["body"];
            switch (MsgType)
            {
                case MessageType.Login:
                    ParseLogin();
                    break;
                case MessageType.ClientData:
                    ParseClientData();
                    break;
                default:
                    break;
            }
        }
        
        private void ParseLogin()
        {
            result = body["result"].AsBool;
            if (body["result"].AsBool)
            {
                // Got success on registration
                uuid = body["uuid"];
            }
        }

        private void ParseClientData()
        {
            login = body["login"].Value;
            var cards = body["cards"].AsObject;
            // Convert json object to dictionary of int int, which can be easily operable inside c#
            var cardDict = new Dictionary<int, int>();
            foreach (var key in cards.Keys)
            {
                cardDict.Add(int.Parse(key), cards[key].AsInt);
            }
            cardDictionary = cardDict;
            // Obtain player's decks
            var decks = body["decks"].AsArray;
            // deck_values and deck names are list, so means id in both list are match
            List<Dictionary<int, int>> decks_values = new List<Dictionary<int, int>>();
            List<string> decks_names = new List<string>();
            foreach (var deck in decks)
            {
                var name = deck.Value["name"].Value;
                var deck_cards = body["cards"].AsObject;
                var cardDeckDict = new Dictionary<int, int>();
                foreach (var key in cards.Keys)
                {
                    cardDeckDict.Add(int.Parse(key), cards[key].AsInt);
                }
                decks_values.Add(cardDeckDict);
                decks_names.Add(name);
            }
            
            decksCardDictionary = decks_values;
            decksNames = decks_names;
        }
    }
}