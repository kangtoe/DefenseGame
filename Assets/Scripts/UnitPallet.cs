using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ButtonInfo
{   
    //public ButtonContoller button;
    public GameObject spwanObject;
}

public class UnitPallet : MonoBehaviour
{
    [SerializeField]
    Transform buttonParent;

    [SerializeField]
    List<GameObject> spwanObjects;    
    
    // Start is called before the first frame update
    void Start()
    {
        ClearButtons();
        CreateButtons();
    }

    void ClearButtons()
    {        
        foreach (Transform tf in buttonParent)
        {
            Debug.Log("ClearButton");
            Destroy(tf.gameObject);
        }
        
    }

    void CreateButtons()
    {
        foreach (GameObject go in spwanObjects)
        {
            Debug.Log("add btn");
            GameObject btnPrefab = go.GetComponent<Spwanable>().buttonPrefab;
            GameObject btnClone = Instantiate(btnPrefab, buttonParent);

            btnClone.GetComponent<ButtonContoller>().InitButton(go);
        }
    }
}
