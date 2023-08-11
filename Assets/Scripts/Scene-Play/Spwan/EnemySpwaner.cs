using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpwaner : UnitSpwaner
{
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static EnemySpwaner Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                instance = FindObjectOfType<EnemySpwaner>();
            }

            // 싱글톤 오브젝트를 반환
            return instance;
        }
    }
    private static EnemySpwaner instance; // 싱글톤이 할당될 static 변수    

    public override Unit SpwanUnit(GameObject unit)
    {
        Unit _unit = base.SpwanUnit(unit);
        _unit.SetEnemy();
        return _unit;
    }
}
