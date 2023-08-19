using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        LoadPallet();
        Init();
    }

    public void Init()
    {
        CheckListCount();
        ClearButtons();
        CreateButtons();

        if (palletButtonType == ButtonType.Upgrade)
        {
            Debug.Log("unitButtonParent :" + unitButtonParent);
            Debug.Log("unitButtonParent.GetChild(0) :" + unitButtonParent.GetChild(0).gameObject.name);

            Button button = unitButtonParent.GetChild(0).GetComponentInChildren<Button>();
            Debug.Log("button : " + button.name);
            button.onClick.Invoke();
        }
    }

    // 디버그용
    void CheckListCount()
    {
        if (unitPrefabs.Count != unitCount)
        {
            Debug.Log("유닛 리스트 개수 : " + unitPrefabs.Count);
        }

        if (skillPrefabs.Count != skillCount)
        {
            Debug.Log("스킬 리스트 개수 : " + skillPrefabs.Count);
        }
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
        // prefab 리스트 개수 = 사전 정의한 카운트 개수
        int addUnit = unitCount - unitPrefabs.Count;
        for (int i = 0; i < addUnit; i++)
        {
            unitPrefabs.Add(null);
        }
        int addSkill = skillCount - skillPrefabs.Count;
        for (int i = 0; i < addSkill; i++)
        {
            skillPrefabs.Add(null);
        }

        // 버튼 생성
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
                //Debug.Log("skill is null");
                Instantiate(emptySkillButton, skillButtonParent);
                continue;
            }
            CreateButton(skill);
        }
    }

    // 스폰 프리팹 -> 버튼 만들기
    ButtonContoller CreateButton(GameObject SpwanPrefab)
    {
        //Debug.Log("add btn : object = " + SpwanPrefab.name);

        Spwanable spwanable = SpwanPrefab.GetComponent<Spwanable>();        
        GameObject btnPrefab = spwanable.buttonPrefab;

        GameObject btnClone;

        switch (spwanable.type)
        {
            case spwanType.unit:
                btnClone = Instantiate(btnPrefab, unitButtonParent);
                break;
            case spwanType.skill:
                btnClone = Instantiate(btnPrefab, skillButtonParent);
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
        Debug.Log("AddToList : " + spwanPrefab);

        int count;
        List<GameObject> list;
        // 스폰 프리팹의 타입 (유닛/스킬) 알아내기
        spwanType type = spwanPrefab.GetComponent<Spwanable>().type;
        // 타입에 따라 추가할 리스트와 max 카운트 정하기
        switch (type)
        {
            case spwanType.unit:
                count = unitCount;
                list = unitPrefabs;
                break;
            case spwanType.skill:
                count = skillCount;
                list = skillPrefabs;
                break;
            default:
                Debug.Log("spwanType error : " + type);
                return false;
        }

        for (int i = 0; i < unitPrefabs.Count; i++)
        {
            // 리스트 요소 중 비어있는 것이 있는 경우, 채워넣기
            if (list[i] == null)
            {
                list[i] = spwanPrefab;
                SavePallet(); // 세이브데이터 갱신
                Init(); // UI 갱신
                return true;
            }
        }

        // 유닛 최대 등록 수 검사
        if (list.Count >= count)
        {
            // TODO : 게임 내 text 띄우기
            TextMaker.instance.CreateCameraText("list full!");
            return false;
        }

        // 유닛 리스트에 추가
        list.Add(spwanPrefab);

        SavePallet(); // 세이브데이터 갱신
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
        
        SavePallet(); // 세이브데이터 갱신
        Init(); // UI 갱신

        return;
    }

    // unitPrefabs -> 세이브 파일 저장
    void SavePallet()
    {
        if (palletButtonType != ButtonType.Spwan)
        {
            //Debug.Log("스폰 팔레트의 유닛만 저장/불러오기 가능해야함!");
            return;
        }

        SaveManager.SaveUsingUnits(unitPrefabs);
        SaveManager.SaveUsingSkills(skillPrefabs);
    }

    // 세이브 파일 -> unitPrefabs 불러오기
    void LoadPallet()
    {
        if (palletButtonType != ButtonType.Spwan)
        {
            //Debug.Log("스폰 팔레트의 유닛만 저장/불러오기 가능해야함!");
            return;
        }

        unitPrefabs = SaveManager.GetUsingUnits();
        skillPrefabs = SaveManager.GetUsingSkills();
    }
}
