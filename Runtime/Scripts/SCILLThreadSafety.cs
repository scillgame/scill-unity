using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class SCILLThreadSafety : MonoBehaviour
{
    // Create a queue that we run through each update cycle. In this Queue, we add stuff to update that changed
    // in other worker threads
    protected static readonly ConcurrentQueue<Action> RunOnMainThread = new ConcurrentQueue<Action>();
    
    // Update is called once per frame
    void Update()
    {
        // Run all code pieces that we dispatched in the queue
        if(!RunOnMainThread.IsEmpty)
        {
            while(RunOnMainThread.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }
    }
}
