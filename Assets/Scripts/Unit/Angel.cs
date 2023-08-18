using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// angel 스킬 사용을 위한 스크립트
public class Angel: MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator anim;

    [SerializeField]
    LayerMask enemyLayer;
    [SerializeField]
    Vector2 AttackArea;

    // Start is called before the first frame update
    void Start()
    {        
        // 위치 조정 (화면 중앙 상단)
        transform.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.75f, 0));

        // 유닛 등장 시 스킬이 사용된것으로 간주
        StartCoroutine(Skill());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,0,0,0.33f);
        Gizmos.DrawCube(transform.position, AttackArea);
    }

    // 1. 투명상태로 등장 
    // 2. 점차 뚜렷해짐
    // 3.완전히 선명해진 시점부터 사라질 때까지 영역 안의 적에게 지속 피해
    IEnumerator Skill()
    {
        float fadeTime = 2f;
        Color fadeColor = new Color(1,1,1,0); // 투명 상태

        // fade in
        while (fadeColor.a < 1f)
        {
            fadeColor.a += Time.deltaTime / fadeTime;
            spriteRenderer.color = fadeColor;

            yield return null;
        }

        // aniation transition
        anim.SetTrigger("attack");

        // 화면 내 유닛에게 지속적인 dot 피해 주기
        StartCoroutine(DealDotDamageToEnemysOnView(0.1f, 10));

        // random explosione effect
        float explosionDuration = 1f; // 폭발 효과 유지 시간
        float time = 0; // 폭발 효과 경과 시간
        while (time < explosionDuration)
        {
            // TODO : 폭발효과 발생 코루틴 구현 및 추가
            time += Time.deltaTime;
            yield return null;
        }

        // fade out
        while (0 < fadeColor.a)
        {
            fadeColor.a -= Time.deltaTime / fadeTime;
            spriteRenderer.color = fadeColor;

            yield return null;
        }

        StopAllCoroutines(); // 모든 코루틴 종료
        Destroy(gameObject);
    }

    // 일정시간마다 화면 내 유닛에게 피해 주기
    IEnumerator DealDotDamageToEnemysOnView(float interval, int damage)
    {        
        // 카메라 영역 내, enemyUnits (모든 적 유닛의 부모) 하위 오브젝트에서 적 유닛 알아오는 방법
        {
            //Transform enemyUnits; // 모든 적 유닛의 부모
            //Camera cam;
            //enemyUnits = GameObject.Find("EnemyUnits").transform;
            //cam = Camera.main;

            //// 화면 중앙 상단에 배치
            //Vector2 cameraCenter = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.7f, 0));
            //transform.position = cameraCenter;

            //// 범위 내 모든 대상 자식 유닛들에게 dot 피해
            //void DamageAllChildUnitInView(Transform parent, int damage, float stratX, float endX)
            //{
            //    int cnt = parent.childCount;

            //    for (int i = 0; i < cnt; i++)
            //    {
            //        float posX = parent.GetChild(i).position.x; // i번째 자식의 x 좌표
            //        if (posX < stratX || endX < posX)
            //        {
            //            //Debug.Log("stratX: " + stratX + " || " + "posX: " + posX + " || " + "endX: " + endX);
            //            continue; // 화면 안에 없는 경우 return
            //        }

            //        // 자식이 unit 스크립트를 가지고 있는 경우, unit에게 피해
            //        Unit unit = parent.GetChild(i).GetComponentInChildren<Unit>();
            //        if (unit) unit.OnHit(damage);
            //    }
            //}

            //// 화면 시작과 끝 지점의 x 좌표(월드 좌표계) 구하기
            //Vector2 cameraCenter = cam.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            //Vector2 cameraRightEnd = cam.ViewportToWorldPoint(new Vector2(1f, 0.5f));
            //float cameraWidthHalf = (cameraRightEnd - cameraCenter).x; // 카메라 폭의 절반
            //float viewStratX = transform.position.x - cameraWidthHalf; // 카메라 화면 시작지점(angle이 생성된 시간 기준)
            //float viewEndX = transform.position.x + cameraWidthHalf; // 카메라 화면 끝지점(angle이 생성된 시간 기준)
            //                                                         //Debug.DrawLine(new Vector3(viewStratX, 0,0), new Vector3(viewEndX, 0, 0), Color.red, 1f);

            //while (true)
            //{
            //    DamageAllChildUnitInView(enemyUnits, damage, viewStratX, viewEndX);
            //    yield return new WaitForSeconds(interval);
            //}
        }

        // 수동으로 지정된 영역 내 적 유닛 알아오기
        List<Unit> units;
        while (true)
        {
            units = GetUnitsInArea(AttackArea, enemyLayer);
            //Debug.Log("units : " + units.Count);
            foreach (Unit _unit in units)
            {
                _unit.OnHit(damage);
            }
            yield return new WaitForSeconds(interval);
        }        
    }

    // 영역 내 대상 Unit들 알아오기
    List<Unit> GetUnitsInArea(Vector2 Area, LayerMask targetLayers)
    {
        // 영역 내 충돌체 구하기
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, Area, 0, targetLayers);
        //Debug.Log("colliders : " + colliders.Length);

        // 충돌체가 Unit인 경우, list에 추가
        List<Unit> targets = new List<Unit>();
        foreach (Collider2D coll in colliders)
        {
            Unit unit = coll.attachedRigidbody.GetComponent<Unit>();
            targets.Add(unit);
        }

        return targets;
    }
    
}
