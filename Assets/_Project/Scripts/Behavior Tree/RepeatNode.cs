public class RepeatNode : DecoratorNode
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override Status OnUpdate()
    {
        child.Tick();
        return Status.Running;
    }
}