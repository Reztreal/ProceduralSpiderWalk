using UnityEngine;

public class WaitNode : ActionNode
{
    public float duration = 1f;
    private float _startTime;
    
    protected override void OnStart()
    {
        _startTime = Time.time;
    }

    protected override void OnStop()
    {
        
    }

    protected override Status OnUpdate()
    {
        if (Time.time - _startTime > duration)
        {
            return Status.Success;
        }

        return Status.Running;
    }
}