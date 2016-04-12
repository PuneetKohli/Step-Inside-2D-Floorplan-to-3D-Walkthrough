using Parse;

[ParseClassName("Node")]

public class NodeParseObject : ParseObject
{
    [ParseFieldName("xpos")]
    public float Xpos
    {
        get { return GetProperty<float>("Xpos"); }
        set { SetProperty<float>(value, "Xpos"); }
    }

    [ParseFieldName("ypos")]
    public float Ypos
    {
        get { return GetProperty<float>("Ypos"); }
        set { SetProperty<float>(value, "Ypos"); }
    }

    [ParseFieldName("plan_id")]
    public Plan PlanId
    {
        get { return GetProperty<Plan>("PlanId"); }
        set { SetProperty<Plan>(value, "PlanId"); }
    }
}