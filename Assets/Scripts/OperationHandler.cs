using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Cards;
using Communication;
using MainMenuScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OperationHandler : MonoBehaviour
{
    [SerializeField] private CardPool cardPoolPref; // prefab for cardPool
    private CardPool cardPool;  // cardPool object, that will be spawned
    private WebSocketQueue socketQueue;
    private OperationsQueue operationsQueue;    // queue with internal operations
    private ServerInMessage clientDataMsg = null;
    
    private ClientData clientData;
    private bool isMainMenuLoaded = false;

    private MainMenu mainMenu;
    
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        socketQueue = WebSocketQueue.Instance;
        operationsQueue = OperationsQueue.Instance;
    }


    private void LoginParse(ServerInMessage msg)
    {
        if (msg.result)
        {
            // Client logged in
            clientData = new ClientData(msg.uuid); // create client's data information
            // Send client data request
            var dataRequest = new ServerOutMessage(MessageType.ClientData);
            socketQueue.AddToSendQueue(dataRequest);
            // Move to loading Main Menu
            SceneManager.LoadScene("MenuScene");
        }
    }

    // Provide required operation when client's data came
    private void ClientDataParse(ServerInMessage msg)
    {
        if (!isMainMenuLoaded)
        {
            clientDataMsg = msg;
            return;
        }
        // if data already received, just update already existing information
        clientData.UpdateClientData(msg.login, msg.cardDictionary);
        var deck = cardPool.CreatePlayerCardDeck(clientData.cardInfoDictionary, msg.decksCardDictionary, msg.decksNames);
        clientData.UpdateClientFullCardDeck(deck);  // this method set clientData.clientFullDeck
        mainMenu.UpdateClientFullDeck(clientData.clientFullDeck);
    }

    private void MainMenuLoaded()
    {   
        // better to operate with everything once we received client data. Means we sent second message
        isMainMenuLoaded = true;
        // First operate with cards and card pool.
        // Instantiate card pool
        cardPool = Instantiate(cardPoolPref, transform.position, Quaternion.identity);
        mainMenu = GameObject.Find("MainMenuManager").GetComponent<MainMenu>(); // get required object
        if (clientDataMsg != null)
        {
            ClientDataParse(clientDataMsg);
        }
        
    }


    private void ParseMessages(ServerInMessage msg)
    {
        switch (msg.MsgType)
        {
            case MessageType.Login:
                LoginParse(msg);
                break;
            case MessageType.ClientData:
                ClientDataParse(msg);
                break;
            default:
                break;
        }
    }

    private void ParseInternalMessages(OperationMessage msg)
    {
        switch (msg.code)
        {
            case OpCodes.MainMenuLoaded:
                MainMenuLoaded();
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

        while (operationsQueue.IfQueueNonEmpty())
        {
            var msg = operationsQueue.ReadFromQueue();
            ParseInternalMessages(msg);
        }
    }
}
