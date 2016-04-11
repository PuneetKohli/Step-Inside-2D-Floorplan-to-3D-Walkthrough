using Parse;

[ParseClassName("Node")]

public class NodeParseObject : ParseObject
{
    [ParseFieldName("xpos")]
    public string Xpos
    {
        get { return GetProperty<double>("Xpos"); }
        set { SetProperty<double>(value, "Xpos"); }
    }

    [ParseFieldName("ypos")]
    public string Ypos
    {
        get { return GetProperty<double>("Ypos"); }
        set { SetProperty<double>(value, "Ypos"); }
    }

    [ParseFieldName("plan_id")]
    public string PlanId
    {
        get { return GetProperty<ParseObject>("PlanId"); }
        set { SetProperty<double>(value, "PlanId"); }
    }
}