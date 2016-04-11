using UnityEngine;
using System.Collections;

public class PlacableHouseObject : HouseObject
{
    
    // Use this for initialization
    protected override void Start()
    {
        isWallAttachable = false;
        base.Start();
    }
    
    public override void init(string category, string name, bool isWallAttachable)
    {
        base.init(category, name, isWallAttachable);
    }
    
    // Update is called once per frame
    void Update()
    {
    }
    
    protected override void MakePlacable()
    {
        base.MakePlacable();
    }
    
    protected override void PlaceObject()
    {
        wallManager.houseObjectList.Add(gameObject);
        base.PlaceObject();
    }    
}