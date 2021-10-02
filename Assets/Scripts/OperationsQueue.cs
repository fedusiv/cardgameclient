using System.Collections.Generic;

public class OperationsQueue
{
    // Singleton Part. 
    private static OperationsQueue operationsQueue;
    public static OperationsQueue Instance
    {
        get
        {
            if(operationsQueue == null)
            {
                operationsQueue = new OperationsQueue();
            }
            return operationsQueue;
        }
    }

    private Queue<OperationMessage> queue;

    private OperationsQueue()
    {
        queue = new Queue<OperationMessage>();
    }

    public void PutIntoQueue(OperationMessage msg)
    {
        queue.Enqueue(msg);
    }

    public OperationMessage ReadFromQueue()
    {
        return queue.Dequeue();
    }

    public bool IfQueueNonEmpty()
    {
        if (queue.Count > 0)
        {
            return true;
        }

        return false;
    }
}

public class OperationMessage
{
    public OpCodes code;
    public OperationMessage(OpCodes code)
    {
        
    }
}

public enum OpCodes
{
    MainMenuLoaded
}
