using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// spwan Unit 리스트의 유닛 스폰
// goldManager의 골드를 사용하여, SpwanManager를 통해 스폰
public class AutoSpwan : MonoBehaviour
{
    // 매니저
    public UnitSpwaner spwaner;
    public ResourceControl resourceControl;

    // 스폰 관련
    public bool canSpwan = true;
    public float spwanInterval = 2f;
    public int spwanSkipPrecent = 50; // 스폰하지 않고 넘어갈 확률(골드를 절약함)
    public List<GameObject> spwanUnits; // 스폰할 전체 유닛 리스트    
    List<GameObject> spwanableUnits; // 현재 골드로 스폰 가능한 유닛 리스트

    // Start is called before the first frame update
    void Start()
    {
        spwanableUnits = new List<GameObject>();

        // 시작하자마자 하나 스폰
        spwaner.SpwanUnit(spwanUnits[0]);

        StartCoroutine(SpwanTryRepeat());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpwanTryRepeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(spwanInterval);

            // 플레이 중이 아니면 넘어가기
            if (!GameManager.instance.isPlaying) continue;
            // 스폰 불가 상태면 넘어가기
            if (!canSpwan) continue;

            // 일정확률로 유닛을 스폰 시도를 건너뜀(골드절약)
            int r = Random.Range(0, 100);
            if (r < spwanSkipPrecent) continue;

            SetSpwanableUnits(); // 스폰가능 유닛 찾기
            if (spwanableUnits.Count < 1) continue;

            //스폰 가능한 유닛 중 랜덤하게 스폰
            int i = Random.Range(0, spwanableUnits.Count);            
            TrySpwanUnit(spwanableUnits[i]);            
        }        
    }

    // 유닛 스폰 시도
    void TrySpwanUnit(GameObject unitPrefab)
    {
        Spwanable unit = unitPrefab.GetComponentInChildren<Spwanable>();

        // 게임 메니저에서 돈이 부족해서 스폰에 실패한 경우
        if (!resourceControl.TrySpendResource(unit.price))
        {
            //Debug.Log("!!! unit: " + unitPrefab.name + " || gold need: " + unit.price);
            return;
        } 

        // 스폰 성공 처리
        spwaner.SpwanUnit(unitPrefab);
    }

    // 현재 골드로 스폰 가능한 유닛 리스트 작성
    void SetSpwanableUnits()
    {
        spwanableUnits.Clear(); // 리스트 초기화

        for (int i = 0; i < spwanUnits.Count; i++)
        {
            Spwanable unit = spwanUnits[i].GetComponentInChildren<Spwanable>();
            // 현재 골드보다 price가 적은 유닛만 리스트에 등록
            float currentGold = resourceControl.CurrentResource;
            
            if (unit.price < currentGold)
            {
                //Debug.Log("loop: " + i);
                //Debug.Log("currentGold: " + currentGold);
                //Debug.Log("unit.price: " + unit.price);
                spwanableUnits.Add(spwanUnits[i]);
            }
        }
    }
}
