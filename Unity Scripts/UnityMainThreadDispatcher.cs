using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    private void Update()
    {
        // Locking the execution queue to ensure thread safety.    
        lock (_executionQueue)
        {
        // While there are actions in the queue, dequeue and invoke each action.    
        while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    // Public method to add an action to the execution queue.
    public void Enqueue(Action action)
    {
        // Locking the execution queue to ensure thread safety.
        lock (_executionQueue)
        {
            // Adding the action to the queue.
            _executionQueue.Enqueue(action);
        }
    }
}