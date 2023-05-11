using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ButtonInfo
{   
    public ButtonContoller button;
    public GameObject unitPrefab;
}

public class UnitPallet : MonoBehaviour
{
    [SerializeField]
    ButtonInfo[] buttonInfos;

    // Start is called before the first frame update
    void Start()
    {
        foreach (ButtonInfo buttonInfo in buttonInfos)
        {
            buttonInfo.button.InitButton(buttonInfo.unitPrefab);
        }
    }
}
