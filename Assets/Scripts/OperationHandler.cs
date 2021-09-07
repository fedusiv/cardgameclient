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
        if (msg.uuid != null)
        {
            // Client logged in
            uuid = msg.uuid;    // save uuid
            SceneManager.LoadScene("MenuScene");    // load main menu scene
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
            var msg = socketQueue.GetReceiveMessage();
            // got message. Now parse it
            ParseMessages(msg);
        }
    }
}
