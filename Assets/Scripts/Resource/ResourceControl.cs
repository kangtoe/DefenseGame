using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// 마나, 소울 등의 자원 소모 & 획득 제어
public class ResourceControl : MonoBehaviour
{
    //public bool UiSync = true; // ui 연동 여부. 플레이어의 골드만 ui에 표기 할것
    //public Text text; // 보유한 골드를 표기할 텍스트

    [HideInInspector]
    public UnityEvent onSetResource;

    [Space]
    [SerializeField] // for debug
    protected float currentResource = 0;
    public float CurrentResource => currentResource;

    [SerializeField]
    protected float maxAmount = 99999f;

    [SerializeField]
    float autoEarnPerSec = 10f; // 초당 자동으로 얻게될 골드량

    #region 유니티 메소드

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance && !GameManager.instance.isPlaying) return;

        // 초당 자동으로 얻게될 골드량의 60분의 1만큼 획득
        EarnResource(autoEarnPerSec * Time.deltaTime);
    }

    #endregion

    // amount만큼 gold 획득
    public void EarnResource(float amount)
    {
        if (currentResource == maxAmount) return;

        // 현재 보유량은 최대 보유량을 넘을 수 없음
        if (currentResource > maxAmount) currentResource = maxAmount;
        else SetResource(currentResource + amount);        
    }

    // amount만큼 mana 소비 (불가능한 경우 return false)
    public bool TrySpendResource(float amount)
    {
        //Debug.Log("try spend gold :" + amount);

        // 소비량 > 보유량 : 불가능
        if (amount > currentResource) return false;

        // 실제 자원 소모 처리
        SetResource(currentResource - amount);        

        return true;
    }

    public void SetResource(float amount)
    {
        currentResource = amount;
        onSetResource.Invoke();
    } 
}
