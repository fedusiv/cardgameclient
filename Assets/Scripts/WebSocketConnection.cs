using System.Collections;
using System.Collections.Generic;
using Communication;
using UnityEngine;

using NativeWebSocket;

public class WebSocketConnection : MonoBehaviour
{
    private WebSocket webSocket;
    private WebSocketQueue socketQueue;
    // Start is called before the first frame update
    async void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        socketQueue = WebSocketQueue.Instance;
        webSocket = new WebSocket("ws://localhost:3002");
        webSocket.OnOpen += () =>
        {
            OnOpen();
        };
        webSocket.OnClose += (e) =>
        {
            OnClose();
        };
        webSocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            OnMessage(message);
        };
        await webSocket.Connect();
    }


    private void OnOpen()
    {
        Debug.Log("Opened");
    }

    private void OnClose()
    {
        Debug.Log("Closed");
    }

    private void OnMessage(string message)
    {
        var msg = new ServerInMessage(message);
        socketQueue.AddToReceiveQueue(msg);
    }
    async void SendWebSocketMessage(string message)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.SendText(message);
        }
    }

    private async void OnApplicationQuit()
    {
        await webSocket.Close();
    }

    // Update is called once per frame
    void Update()
    {    
        // Updating to receive websocket message
        #if !UNITY_WEBGL || UNITY_EDITOR
            webSocket.DispatchMessageQueue();
        #endif
        // Sending data
        while (socketQueue.IsSendQueueNonEmpty())
        {
            var msg = socketQueue.GetSendMessage();
            SendWebSocketMessage(msg.MessageString);
        }
    }
}
