using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgradable : MonoBehaviour
{
    const int MAX_LEVEL = 10;

    public int defaultCost = 100; // 유닛 레벨 1 업그레이드에 요구할 자원량    
    float costMultPerLevel = 1.1f; // 레벨별 증가되는 필요 자원량 (곱연산)

    // 현재 레벨. 레벨0은 사용에 구매 필요
    public string CurrentLevelStr => "lv." + currentLevel;    
    [SerializeField]
    int currentLevel = 0;

    // 다음 레벨업에 필요한 자원량
    public string costToNextLevelStr
    {
        get
        {            
            if (currentLevel < MAX_LEVEL)
            {
                int cost = (int)(defaultCost * currentLevel * costMultPerLevel);
                return cost + " gold";
            }
            else
            {
                return "max";
            }            
        }
    }

    public bool LevelUp()
    {        
        // currentLevel 유효하지 않은 값
        if (currentLevel < 0 || currentLevel > MAX_LEVEL)
        {
            string str = null;
            str += "레벨 값 이상 : " + currentLevel;
            str += "0 - " + MAX_LEVEL + " 사이 값으로 수정";
            Debug.Log(str);

            currentLevel = Mathf.Clamp(0, MAX_LEVEL, currentLevel);

            return false;
        }

        currentLevel++;
        return true;
    }
}
