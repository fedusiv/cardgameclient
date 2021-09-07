using System.Collections;
using System.Collections.Generic;
using Communication;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    private WebSocketQueue webSocketQueue;

    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private InputField loginField, passwordField;
    
    private void Awake() {
        webSocketQueue = WebSocketQueue.Instance;
    }
    
    void Start()
    {
        //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
        loginButton.onClick.AddListener(OnLoginClick);
    }

    void OnLoginClick()
    {
        if (!string.IsNullOrEmpty(loginField.text) && !string.IsNullOrEmpty(passwordField.text))
        {
            // Build and Send message to server, that want to login
            var msg = new ServerOutMessage(loginField.text, passwordField.text);
            webSocketQueue.AddToSendQueue(msg);
        }
    }

}
