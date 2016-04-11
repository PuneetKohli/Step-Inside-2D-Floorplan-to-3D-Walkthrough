using Parse;

[ParseClassName("HouseObject")]

public class HouseParseObject : ParseObject
{
    [ParseFieldName("name")]
    public string Name
    {
        get { return GetProperty<string>("Name"); }
        set { SetProperty<string>(value, "Name"); }
    }
    
    [ParseFieldName("rotation")]
    public double Rotation
    {
        get { return GetProperty<double>("Rotation"); }
        set { SetProperty<double>(value, "Rotation"); }
    }

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
    
    [ParseFieldName("category")]
    public string Category
    {
        get { return GetProperty<string>("Category"); }
        set { SetProperty<string>(value, "Category"); }
    }

    [ParseFieldName("is_attached")]
    public bool Isattached
    {
        get { return GetProperty<bool>("Isattached"); }
        set { SetProperty<bool>(value, "Isattached"); }
    }

    [ParseFieldName("plan_id")]
    public Plan PlanId
    {
        get { return GetProperty<Plan>("PlanId"); }
        set { SetProperty<Plan>(value, "PlanId"); }
    }
}
