using Parse;

[ParseClassName("Node")]

public class NodeParseObject : ParseObject
{
    [ParseFieldName("xpos")]
    public double Xpos
    {
        get { return GetProperty<double>("Xpos"); }
        set { SetProperty<double>(value, "Xpos"); }
    }

    [ParseFieldName("ypos")]
    public double Ypos
    {
        get { return GetProperty<double>("Ypos"); }
        set { SetProperty<double>(value, "Ypos"); }
    }

    [ParseFieldName("plan_id")]
    public Plan PlanId
    {
        get { return GetProperty<Plan>("PlanId"); }
        set { SetProperty<Plan>(value, "PlanId"); }
    }
}