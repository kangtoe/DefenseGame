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
    GameObject emptyUnitButton;

    [SerializeField]
    List<GameObject> unitPrefabs;

    [SerializeField]
    List<GameObject> skillPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        ClearButtons();
        CreateButtons();
    }

    void ClearButtons()
    {
        //Debug.Log("ClearButton");

        foreach (Transform tf in buttonParent)
        {            
            Destroy(tf.gameObject);
        }
        
    }

    void CreateButtons()
    {
        foreach (GameObject go in unitPrefabs)
        {
            if (go == null)
            {
                Instantiate(emptyUnitButton, buttonParent);
                continue;
            }
            else
            {
                Debug.Log("add btn : gbject = " + go.name);
                GameObject btnPrefab = go.GetComponent<Spwanable>().buttonPrefab;
                GameObject btnClone = Instantiate(btnPrefab, buttonParent);

                btnClone.GetComponent<ButtonContoller>().InitButton(go);
            }

            
        }
    }
}
