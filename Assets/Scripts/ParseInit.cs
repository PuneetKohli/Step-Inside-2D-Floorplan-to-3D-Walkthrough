using UnityEngine;
using Parse;

public class ParseInit : MonoBehaviour
{
    void Awake()
    {
        ParseObject.RegisterSubclass<NodeParseObject>();
        ParseObject.RegisterSubclass<HouseParseObject>();
        ParseObject.RegisterSubclass<NodeConnection>();
        ParseObject.RegisterSubclass<Plan>();
    }
}
