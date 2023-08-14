using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPallet : MonoBehaviour
{
    [Space]
    [SerializeField]
    ButtonType palletButtonType;

    [Header("오브젝트 등록 제한 카운트")]
    [SerializeField]
    int unitCount = 6; // 유닛 등록 제한
    [SerializeField]
    int skillCount = 2; // 유닛 등록 제한

    [Header("버튼 부모 오브젝트 : 자식 UI 정렬 기능")]
    [SerializeField]
    Transform unitButtonParent;
    [SerializeField]
    Transform skillButtonParent;

    [Header("빈 버튼 프리팹")]
    [SerializeField]
    GameObject emptyUnitButton;
    [SerializeField]
    GameObject emptySkillButton;

    [Header("버튼을 생성할 오브젝트 리스트")]
    [SerializeField]
    List<GameObject> unitPrefabs;
    [SerializeField]
    List<GameObject> skillPrefabs;


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

        foreach (Transform tf in unitButtonParent)
        {
            Destroy(tf.gameObject);
        }
        foreach (Transform tf in skillButtonParent)
        {
            Destroy(tf.gameObject);
        }
    }

    // 리스트 정보대로 버튼 생성
    void CreateButtons()
    {
        foreach (GameObject unit in unitPrefabs)
        {
            if (unit == null)
            {
                Instantiate(emptyUnitButton, unitButtonParent);
                continue;
            }

            CreateButton(unit);
        }

        foreach (GameObject skill in skillPrefabs)
        {
            if (skill == null)
            {
                Instantiate(emptySkillButton, skillButtonParent);
                continue;
            }

            CreateButton(skill);
        }
    }

    // 스폰 프리팹 -> 버튼 만들기
    ButtonContoller CreateButton(GameObject SpwanPrefab)
    {
        Debug.Log("add btn : object = " + SpwanPrefab.name);

        Spwanable spwanable = SpwanPrefab.GetComponent<Spwanable>();        
        GameObject btnPrefab = spwanable.buttonPrefab;

        GameObject btnClone;

        switch (spwanable.type)
        {
            case spwanType.unit:
                btnClone = Instantiate(btnPrefab, unitButtonParent);
                break;
            case spwanType.skill:
                btnClone = Instantiate(btnPrefab, unitButtonParent);
                break;
            default:
                Debug.Log("유효하지 않은 spwanType : " + spwanable.type);
                return null;
        }        

        ButtonContoller button = btnClone.GetComponent<ButtonContoller>();
        button.buttonType = palletButtonType;
        button.InitButton(SpwanPrefab);
        return button;
    }

    // 스폰 프리팹을 리스트에 추가
    public bool AddToList(GameObject spwanPrefab)
    {
        for (int i = 0; i < unitPrefabs.Count; i++)
        {
            // 리스트 요소 중 비어있는 것이 있는 경우, 채워넣기
            if (unitPrefabs[i] == null)
            {
                unitPrefabs[i] = spwanPrefab;
                Init(); // UI 갱신
                return true;
            }
        }

        // 유닛 최대 등록 수 검사
        if (unitPrefabs.Count >= unitCount)
        {
            // TODO : 게임 내 text 띄우기
            TextMaker.instance.CreateCameraText("list full!");
            return false;
        }

        // 유닛 리스트에 추가
        unitPrefabs.Add(spwanPrefab);
        Init(); // UI 갱신
        return true;
    }

    // 기존 등록된 스폰 프리팹 등록 해제
    public void DeleteOnList(GameObject spwanPrefab)
    {
        // 인덱스 찾기
        int idx = unitPrefabs.FindIndex(x => x == spwanPrefab);
        if (idx == -1)
        {
            Debug.Log("리스트에 없는 spwanPrefab : " + spwanPrefab);
            return;
        }

        unitPrefabs[idx] = null;
        Init(); // UI 갱신
        return;
    }
}
