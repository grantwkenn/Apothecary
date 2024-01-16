using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropDataManager : ScriptableObject
{
    [SerializeField]
    List<Crop_Data> allCropData;

    [SerializeField]
    List<Item_Data> seedItemData;

    [SerializeField]

    
    Dictionary<string, Crop_Data> cropDataMap;

    private void OnValidate()
    {
        if (cropDataMap == null) cropDataMap = new Dictionary<string, Crop_Data>();
        cropDataMap.Clear();

        foreach(Crop_Data cd in allCropData)
        {
            cropDataMap.Add(cd.name, cd);
        }
    }
}
