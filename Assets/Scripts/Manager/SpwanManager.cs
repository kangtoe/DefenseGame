using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanManager : MonoBehaviour
{
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static SpwanManager Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                instance = FindObjectOfType<SpwanManager>();
            }

            // 싱글톤 오브젝트를 반환
            return instance;
        }
    }
    private static SpwanManager instance; // 싱글톤이 할당될 static 변수    

    public Transform unitParent;

    public Transform spwanPointStart;
    public Transform spwanPointEnd;
    public bool isEnemySpwaner = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpwanUnit(GameObject unit)
    {
        //Debug.Log("spwan unit:" + unit.name);

        // 생성
        Vector3 spwanPos = GetRamdomSpwanPos();
        Transform tf = Instantiate(unit, spwanPos, Quaternion.identity).transform;
        // 부모 설정
        tf.SetParent(unitParent);

        Unit _unit = tf.GetComponentInChildren<Unit>();
        // 적일 경우 추가적 처리
        if (isEnemySpwaner) _unit.SetEnemy();
    }

    // spwanPointStart와 spwanPointEnd 사이의 무작위 좌표를 구한다.
    Vector3 GetRamdomSpwanPos()
    {
        Vector3 vec;
       
        float x = Random.Range(spwanPointStart.position.x, spwanPointEnd.position.x);
        float y = Random.Range(spwanPointStart.position.y, spwanPointEnd.position.y);

        // z축에 y값을 넣는 이유: y좌표 기준 스프라이트 정렬 
        // => 같은 유닛끼리 그려지는 순서가 엎치락 뒤치락 하는 것 방지
        vec = new Vector3(x, y, y);
        return vec;
    }
}
