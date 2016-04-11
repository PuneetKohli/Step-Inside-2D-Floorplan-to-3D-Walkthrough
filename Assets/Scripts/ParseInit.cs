using UnityEngine;
using Parse;

public class ExtraParseInitialization : MonoBehaviour
{
    void Awake()
    {
        ParseObject.RegisterSubclass<Node>();
        ParseObject.RegisterSubclass<HouseObject>();
        ParseObject.RegisterSubclass<NodeConnection>();
        ParseObject.RegisterSubclass<Plan>();
    }
}
