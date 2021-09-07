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
                default:
                    break;
            }
        }
        
        private void ParseLogin()
        {
            if (body["result"].AsBool)
            {
                // Got success on registration
                uuid = body["uuid"];
            }
        }
    }
}