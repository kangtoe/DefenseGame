using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpwanPallet : MonoBehaviour
{
    [SerializeField]
    Transform buttonParent;

    [SerializeField]
    GameObject emptyUnitButton;

    [SerializeField]
    List<GameObject> unitPrefabs;

    [SerializeField]
    List<GameObject> skillPrefabs;

    [SerializeField]
    int unitCount = 6; // 유닛 등록 제한
    [SerializeField]
    int skillCount = 2; // 유닛 등록 제한

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

    public void AddUnit(GameObject new_unitPrefab)
    {
        for (int i = 0; i < unitPrefabs.Count; i++)
        {
            // 리스트 요소 중 비어있는 것이 있는 경우, 채워넣기
            if (unitPrefabs[i] == null)
            {                
                unitPrefabs[i] = new_unitPrefab;                
                Init(); // UI 갱신
                return;
            }
        }

        // 유닛 최대 등록 수 초과
        if (unitPrefabs.Count > unitCount)
        {
            // TODO : 게임 내 text 띄우기
            Debug.Log("유닛 최대 등록 수 초과");
            return;
        }

        // 유닛 리스트에 추가
        unitPrefabs.Add(new_unitPrefab);
        Init(); // UI 갱신        
    }
}
