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
        public string login;
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
            Debug.Log(cards);
            Debug.Log(cards["1"].AsInt);
        }
    }
}