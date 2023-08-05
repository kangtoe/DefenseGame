using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Init();
    }

    public void Init()
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

    // 리스트 정보대로 버튼 생성
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
