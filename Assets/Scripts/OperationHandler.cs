using System.Collections;
using System.Collections.Generic;
using Communication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OperationHandler : MonoBehaviour
{
    private WebSocketQueue socketQueue;
    private string uuid = "";
    
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        socketQueue = WebSocketQueue.Instance;
    }


    private void LoginParse(ServerInMessage msg)
    {
        if (msg.result)
        {
            // Client logged in
            uuid = msg.uuid;    // save uuid
            // Send client data request
            var dataRequest = new ServerOutMessage(MessageType.ClientData);
            socketQueue.AddToSendQueue(dataRequest);
            // load main menu scene
            SceneManager.LoadScene("MenuScene");
        }
    }
    

    private void ParseMessages(ServerInMessage msg)
    {
        switch (msg.MsgType)
        {
            case MessageType.Login:
                LoginParse(msg);
                break;
            default:
                break;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        while (socketQueue.IsReceiveQueueNonEmpty())
        {
            // Parsing all income operation
            var msg = socketQueue.GetReceiveMessage();
            // got message. Now parse it
            ParseMessages(msg);
        }
    }
}
