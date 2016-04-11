using Parse;

[ParseClassName("NodeConnection")]

public class NodeConnection : ParseObject
{
    [ParseFieldName("end_node")]
    public string EndNode
    {
        get { return GetProperty<ParseObject>("EndNode"); }
        set { SetProperty<ParseObject>(value, "EndNode"); }
    }
    
    [ParseFieldName("start_node")]
    public string StartNode
    {
        get { return GetProperty<ParseObject>("StartNode"); }
        set { SetProperty<ParseObject>(value, "StartNode"); }
    }
    
    [ParseFieldName("plan_id")]
    public string PlanId
    {
        get { return GetProperty<ParseObject>("PlanId"); }
        set { SetProperty<double>(value, "PlanId"); }
    }
}
