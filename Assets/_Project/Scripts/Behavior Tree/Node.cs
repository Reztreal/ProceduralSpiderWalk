using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status { Success, Running, Failure }

public abstract class Node : ScriptableObject
{
    public Status status = Status.Running;
    public bool started = false;

    public string guid;

    public Vector2 position;

    public Status Tick()
    {
        if (!started)
        {
            started = true;
            OnStart();
        }

        status = OnUpdate();

        if (status == Status.Failure || status == Status.Success)
        {
            OnStop();
            started = false;
        }

        return status;
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }
    
    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract Status OnUpdate();
}
