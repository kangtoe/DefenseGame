using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 마나 소모 & 획득 제어
public class ManaResource : MonoBehaviour
{
    //public bool UiSync = true; // ui 연동 여부. 플레이어의 골드만 ui에 표기 할것
    //public Text text; // 보유한 골드를 표기할 텍스트

    [SerializeField] // for debug
    protected float currentMana = 0;
    public float CurrentMana => currentMana;

    [SerializeField]
    protected float maxMana = 99999f;

    [SerializeField]
    float autoManaPerSec = 10f; // 초당 자동으로 얻게될 골드량

    #region 유니티 메소드

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance && !GameManager.instance.isPlaying) return;

        // 초당 자동으로 얻게될 골드량의 60분의 1만큼 획득
        EarnMana(autoManaPerSec * Time.deltaTime);
    }

    #endregion

    // amount만큼 gold 획득
    public void EarnMana(float amount)
    {
        if (currentMana == maxMana) return;

        // 현재 보유량은 최대 보유량을 넘을 수 없음
        if (currentMana > maxMana) currentMana = maxMana;
        else SetMana(currentMana + amount);        
    }

    // amount만큼 mana 소비 (불가능한 경우 return false)
    public bool TrySpendGold(float amount)
    {
        //Debug.Log("try spend gold :" + amount);

        // 소비량 > 보유량 : 불가능
        if (amount > currentMana) return false;

        // 실제 gold 소모 처리
        SetMana(currentMana - amount);        

        return true;
    }

    virtual public void SetMana(float amount)
    {
        currentMana = amount;
    }

 
}
