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
    public string Rotation
    {
        get { return GetProperty<double>("Rotation"); }
        set { SetProperty<double>(value, "Rotation"); }
    }

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
    
    [ParseFieldName("category")]
    public string Category
    {
        get { return GetProperty<string>("Category"); }
        set { SetProperty<string>(value, "Category"); }
    }

    [ParseFieldName("is_attached")]
    public string Isattached
    {
        get { return GetProperty<bool>("Isattached"); }
        set { SetProperty<bool>(value, "Isattached"); }
    }

    [ParseFieldName("plan_id")]
    public string PlanId
    {
        get { return GetProperty<ParseObject>("PlanId"); }
        set { SetProperty<double>(value, "PlanId"); }
    }
}
