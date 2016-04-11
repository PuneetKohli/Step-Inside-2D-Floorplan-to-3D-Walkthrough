using Parse;

[ParseClassName("NodeConnection")]

public class NodeConnection : ParseObject
{
    [ParseFieldName("end_node")]
    public NodeParseObject EndNode
    {
        get { return GetProperty<NodeParseObject>("EndNode"); }
        set { SetProperty<NodeParseObject>(value, "EndNode"); }
    }

    [ParseFieldName("start_node")]
    public NodeParseObject StartNode
    {
        get { return GetProperty<NodeParseObject>("StartNode"); }
        set { SetProperty<NodeParseObject>(value, "StartNode"); }
    }

    [ParseFieldName("plan_id")]
    public Plan PlanId
    {
        get { return GetProperty<Plan>("PlanId"); }
        set { SetProperty<Plan>(value, "PlanId"); }
    }
}