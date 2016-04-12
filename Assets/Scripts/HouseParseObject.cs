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
    public float Rotation
    {
        get { return GetProperty<float>("Rotation"); }
        set { SetProperty<float>(value, "Rotation"); }
    }

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

    [ParseFieldName("elevation")]
    public float Elevation
    {
        get { return GetProperty<float>("Elevation"); }
        set { SetProperty<float>(value, "Elevation"); }
    }

    [ParseFieldName("length")]
    public float Length
    {
        get { return GetProperty<float>("Length"); }
        set { SetProperty<float>(value, "Length"); }
    }

    [ParseFieldName("height")]
    public float Height
    {
        get { return GetProperty<float>("Height"); }
        set { SetProperty<float>(value, "Height"); }
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