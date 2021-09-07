using System.Collections;
using System.Collections.Generic;

using Communication;

public class WebSocketQueue
{
    // Singleton Part. 
    private static WebSocketQueue webSocketQueue;
    public static WebSocketQueue Instance
    {
        get
        {
            if(webSocketQueue == null)
            {
                webSocketQueue = new WebSocketQueue();
            }
            return webSocketQueue;
        }
    }

    private WebSocketQueue()
    {
        sendQueue = new Queue<ServerOutMessage>();
        receiveQueue = new Queue<ServerInMessage>();
    }

    // Functionality
    private Queue<ServerOutMessage> sendQueue;
    private Queue<ServerInMessage> receiveQueue;
    
    public void AddToSendQueue(ServerOutMessage msg)
    {
        sendQueue.Enqueue(msg);
    }

    public ServerOutMessage GetSendMessage()
    {
        return sendQueue.Dequeue();
    }
    public bool IsSendQueueNonEmpty()
    {
        if (sendQueue.Count > 0)
        {
            return true;
        }
        return false;
    }

    public void AddToReceiveQueue(ServerInMessage msg)
    {
        receiveQueue.Enqueue(msg);
    }
    public ServerInMessage GetReceiveMessage()
    {
        return receiveQueue.Dequeue();
    }
    
    public bool IsReceiveQueueNonEmpty()
    {
        if (receiveQueue.Count > 0)
        {
            return true;
        }
        return false;
    }
}
