using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgradable : MonoBehaviour
{
    public const int MAX_LEVEL = 10;

    public int defaultCost = 100; // 유닛 레벨 1 업그레이드에 요구할 자원량    
    float costMultPerLevel = 1.1f; // 레벨별 증가되는 필요 자원량 (곱연산)

    // 현재 레벨. 레벨0은 사용에 구매 필요
    public int CurrentLevel => currentLevel;    
    [SerializeField]
    int currentLevel = 0;

    [Header("업그레이드 오브젝트 설명")]
    [TextArea(3, 6)]
    public string desc;

    // 다음 레벨업에 필요한 자원량   
    public int CostToNextLevel
    {
        get {
            if (CurrentLevel == 0) return (int)defaultCost;
            else return (int)(defaultCost * (CurrentLevel + 1) * costMultPerLevel); ; 
        }
    }

    // currentLevel 값이 유효한지 검사 및 수정
    void ValueCheck()
    {        
        if (currentLevel < 0 || currentLevel > MAX_LEVEL)
        {
            string str = null;
            str += "레벨 값 이상 : " + currentLevel;
            str += "0 - " + MAX_LEVEL + " 사이 값으로 수정";
            Debug.Log(str);

            currentLevel = Mathf.Clamp(currentLevel, 0, MAX_LEVEL);         
        }        
    }

    public void SetLevelFromSave()
    {
        currentLevel = SaveManager.GetUpgradeLevel(name);
        Debug.Log("SetLevelFromSave => " + gameObject.name + " Level : " + currentLevel);
        ValueCheck();
    }
    
    public bool LevelUp()
    {
        currentLevel++;
        ValueCheck();

        Debug.Log(gameObject + " Level : " + currentLevel);

        // 정보 저장
        SaveManager.UpdateLevelInfo(gameObject.name, currentLevel);        
        return true;
    }
}
