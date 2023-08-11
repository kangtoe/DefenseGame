using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpwaner : MonoBehaviour
{
    public Transform unitParent;

    public Transform spwanPointStart;
    public Transform spwanPointEnd;
    //public bool isEnemySpwaner = false;

    virtual public Unit SpwanUnit(GameObject unit)
    {
        //Debug.Log("spwan unit:" + unit.name);

        // 생성
        Vector3 spwanPos = GetRamdomSpwanPos();
        Transform tf = Instantiate(unit, spwanPos, Quaternion.identity).transform;
        // 부모 설정
        tf.SetParent(unitParent);

        Unit _unit = tf.GetComponentInChildren<Unit>();
        return _unit;
        // 적일 경우 추가적 처리
        //if (isEnemySpwaner) _unit.SetEnemy();
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
