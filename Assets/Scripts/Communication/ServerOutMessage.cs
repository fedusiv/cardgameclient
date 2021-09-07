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
        public ServerOutMessage(string login, string password)
        {
            var body = new JSONObject();
            body.Add("login",login);
            body.Add("password",password);
            var jo = new JSONObject();
            jo.Add("type",(int)MessageType.Login);
            jo.Add("body", body);
            MessageString = jo.ToString();
            Debug.Log(MessageString);
        }
    }
}