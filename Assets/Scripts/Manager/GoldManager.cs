using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 유닛 생산과 보유 골드 소모&획득 제어&UI관리
public class GoldManager : MonoBehaviour
{
    public bool UiSync = true; // ui 연동 여부. 플레이어의 골드만 ui에 표기 할것

    public Text goldText; // 보유한 골드를 표기할 텍스트

    [SerializeField] // for debug
    float currentGold = 0;
    [SerializeField]
    float maxGold = 99999f;
    [SerializeField]
    float autoGetGold = 10f; // 초당 자동으로 얻게될 골드량

    #region 유니티 메소드
    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance && !GameManager.instance.isPlaying) return;

        // 초당 자동으로 얻게될 골드량의 60분의 1만큼 획득
        EarnGold(autoGetGold * Time.deltaTime);
        
    }

    #endregion

    // amount만큼 gold 획득
    public void EarnGold(float amount)
    {        
        if (currentGold == maxGold) return;

        currentGold += amount;

        // 현재 보유량은 최대 보유량을 넘을 수 없음
        if (currentGold > maxGold) currentGold = maxGold;

        SyncGoldText(); // 골드 ui에 표기
    }

    // 현재 소지한 골드 알아오기
    public float GetCurrentGold()
    {
        return currentGold;
    }

    // amount만큼 gold 소비 (불가능한 경우 return false)
    public bool TrySpendGold(int amount)
    {
        //Debug.Log("try spend gold :" + amount);

        // 소비량 > 보유량 : 불가능
        if (amount > currentGold) return false;

        // 실제 gold 소모 처리
        currentGold -= amount;

        SyncGoldText();

        return true;
    }

    // gold text와 실제 골드 보유량 동기화
    void SyncGoldText()
    {
        if (!UiSync) return;

        // 다섯 자릿수에서 없는 단위는 0으로 채우기
        string str = ((int)currentGold).ToString("00000");
        goldText.text = str;
    }
}
