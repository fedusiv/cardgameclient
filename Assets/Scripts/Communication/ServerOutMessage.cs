using System.Collections;
using System.Collections.Generic;
using MessageOut;
using UnityEngine;
using SimpleJSON;

namespace Communication
{
    public class ServerOutMessage
    {
        public string MessageString = null;
        // Login request message builder
        public ServerOutMessage(string login, string password)
        {
            var body = new JSONObject();
            body.Add("login",login);
            body.Add("password",password);
            var jo = new JSONObject();
            jo.Add("type",(int)MessageType.Login);
            jo.Add("body", body);
            MessageString = jo.ToString();
        }

        // Any request message
        public ServerOutMessage(MessageType type)
        {
            var body = new JSONObject();
            var jo = new JSONObject();
            jo.Add("type",(int)type);
            jo.Add("body", body);
            MessageString = jo.ToString();
        }
    }
}