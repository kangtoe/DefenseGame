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

    public Text unitUpgradeButtonText;
    public Text unitEquiptButtonText;

    [Header("디버그용: 현재 선택된 버튼")]
    [SerializeField]    
    ButtonContoller selectedButton;
    // 버튼의 다른 속성들
    Unit unit;
    Upgradable upgradable;

    ResourceControl soulRes => PlayerResourceManager.Instance.SoulResource;

    // selectedButton 변경
    public void SetSelectedUnit(ButtonContoller button)
    {
        if (selectedButton) selectedButton.OnDeselected();        
        selectedButton = button;
        selectedButton.OnSelected();

        unit = selectedButton.UnitPrefab.GetComponent<Unit>();
        upgradable = selectedButton.UnitPrefab.GetComponent<Upgradable>();

        SetInfoUi();
    }

    // 현재 selectedButton -> 정보 UI 반영
    void SetInfoUi()
    {                
        // 버튼 오브젝트에 해당하는 유닛 정보 UI에 표시
        unitName.text = unit.name;
        unitDesc.text = unit.desc;     
        
        string str;
        // 강화에 필요한 자원
        if (upgradable.CurrentLevel >= Upgradable.MAX_LEVEL) str = "max";
        else str = upgradable.CostToNextLevel.ToString() + " soul";
        unitUpgradeGold.text = str;
        // 사용 잠금해제 / 업그레이드 버튼의 텍스트
        if (upgradable.CurrentLevel < 1) str = "unlock";
        else str = "upgrade";
        unitUpgradeButtonText.text = str;
        // 장비 / 장비해제 버튼의 텍스트
        if (selectedButton.IsEquipted) str = "unequipt";
        else str = "equipt";
        unitEquiptButtonText.text = str;
    }

    #region 버튼 이벤트

    public void OnEquiptButtonClick()
    {
        // 유닛 잠금 해제 검사
        if (!selectedButton.IsUnlocked)
        {
            TextMaker.instance.CreateCameraText("need unlock!");
            return;
        }

        selectedButton.ToggleEquipted();

        SetInfoUi();
    }

    public void OnUpgradeButtonClick()
    {
        // 레벨 한계치 검사
        if (upgradable.CurrentLevel >= Upgradable.MAX_LEVEL)
        {
            TextMaker.instance.CreateCameraText("level max!");
            return;
        }

        int cost = upgradable.CostToNextLevel;
        // 자원 검사
        if (cost > soulRes.CurrentResource)
        {
            TextMaker.instance.CreateCameraText("Not Enough Soul!");
            return;
        }

        // 강화 처리
        soulRes.TrySpendResource(cost);
        upgradable.LevelUp();
        selectedButton.InitButton(selectedButton.UnitPrefab);
        SetInfoUi();
    }

    #endregion

}
