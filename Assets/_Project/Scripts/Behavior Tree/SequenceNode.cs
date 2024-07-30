public class SequenceNode : CompositeNode
{
    private int _current;
    protected override void OnStart()
    {
        _current = 0;
    }

    protected override void OnStop()
    {
        
    }

    protected override Status OnUpdate()
    {
        var child = children[_current];

        switch (child.Tick())
        {
            case Status.Running:
                return Status.Running;
            case Status.Failure:
                return Status.Failure;
            case Status.Success:
                _current++;
                break;
        }

        return _current == children.Count ? Status.Success : Status.Running;
    }
}