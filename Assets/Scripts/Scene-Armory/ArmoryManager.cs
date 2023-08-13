using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 현재 선택된 유닛 정보 표기
// 선택된 유닛에 대한 버튼 클릭시 동작을 정의 (업그레이드, 유닛 사용 등)
public class ArmoryManager : MonoBehaviour
{
    public static ArmoryManager Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                instance = FindObjectOfType<ArmoryManager>();
            }

            // 싱글톤 오브젝트를 반환
            return instance;
        }
    }
    private static ArmoryManager instance; // 싱글톤이 할당될 static 변수    

    public Text unitName;
    public Text unitDesc;
    public Text unitUpgradeGold;

    [Header("디버그용: 현재 선택된 버튼")]
    [SerializeField]    
    ButtonContoller selectedButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelectedUnit(ButtonContoller button)
    {
        if (selectedButton) selectedButton.OnDeselected();        
        selectedButton = button;
        selectedButton.OnSelected();

        // 버튼 오브젝트에 해당하는 유닛 정보 UI에 표시        
        Unit unit = selectedButton.UnitPrefab.GetComponent<Unit>();
        unitName.text = unit.name;
        unitDesc.text = unit.desc;
        Upgradable upgradable = selectedButton.UnitPrefab.GetComponent<Upgradable>();
        unitUpgradeGold.text = upgradable.costToNextLevelStr.ToString();
    }

    public void OnEquiptButtonClick()
    { 
    
    }

    public void OnUpgradeButtonClick()
    {

    }
}
