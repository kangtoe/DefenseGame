using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttackType
{
    melee = 0,
    range,
    targeting, // 히트스캔 방식. 하나의 대상만 타격
    none = 99// 공격 안함
}

public enum UnitType
{
    undefined = 0,
    skillUnit = 98, // 다른 유닛과 직접적인 상호작용 없음. 스킬을 위해 존재
    teamBase = 99 // 팀 베이스. 파괴시 패배
}

public class Unit : MonoBehaviour
{   
    // 컴포넌트    
    //public Transform UI_object; // 적 캐릭터 반전 시 ui는 반전 대상에서 제외
    public Transform firePoint; // 원거리 발사체 생성 위치
    public Transform damageTextPonit; // 피해량 표기 위치(null인 경우 기본 위치는 hp bar 위쪽)
    public Animator animator;
    public GameObject bulletPrefab;
    public GameObject deathEffectPrefab;
    public Slider hpBar;
    public SpriteRenderer spriterRenderer;
    Collider2D coll;

    // 수치
    public UnitType unitType;
    public int price; // 생산에 필요한 가격
    public float spwanCooltime; // 최소 생산 간격(초)
    public float maxHp;
    float currentHp;
    public float moveSpeed;
    public float attackRange; // 공격거리 == 적 탐색거리
    public float damage;
    public int impact; // 공격의 충격량
    public AttackType attackType;

    // 타격&식별    
    public bool isEnemy = false; // 적(오른쪽에서 등장, 왼쪽으로 진행)인가?
    LayerMask targetLayer; // 공격 대상 레이어
    bool hasTarget = false; // 공격 대상이 존재하는가
    bool isDying = false; // 사망 애니메이션 재생 중
    bool isAttacking = false; // 공격 애니메이션 재생 중
    float hurtEndTime = 0f; // 피격 효과 적용이 끝나는 시간 (현재시간+지속기간)
    float hitEffectDuration = 0.1f;
    int dir; // 바라보는 방향(오른쪽 => 1, 왼쪽 => -1)

    #region 유니티 라이프 사이클

    // Start is called before the first frame update
    void Start()
    {
        if (unitType == UnitType.skillUnit) return;

        // 바라보는 방향(오른쪽 => 1, 왼쪽 => -1)
        if (!isEnemy) dir = 1; else dir = -1;

        currentHp = maxHp;
        SyncSlider();
        hpBar.gameObject.SetActive(false); // 피격 전까지는 hp바 숨기기

        // 적일 경우 뒤집기
        if (isEnemy) Flip();

        // 레이어 관련 설정
        SetLayer();
        SetTargetLayer();

        // 피격 효과를 검사하는 코루틴
        StartCoroutine(ShakeCheck(0.2f, 0.05f));
        StartCoroutine(BlinkCheck());

        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (unitType == UnitType.skillUnit) return;
        // 활동 영역을 벗어나게 되면 유닛 삭제
        if (!GameManager.instance.CheckInUnitArea(transform.parent.position))
            Destroy(transform.parent.gameObject);

        DyingAnimationCheck();

        #region 탐지&공격 거리 디버깅
        if (attackType != AttackType.none)
        {
            // 근접공격의 경우, 타겟 탐지거리는 공격거리보다 약간 더 짧음
            float tragetSearchRange = attackRange;
            if (attackType == AttackType.melee) tragetSearchRange -= 1;
            Debug.DrawLine(transform.parent.position + Vector3.up, transform.parent.position + Vector3.up + new Vector3(dir * tragetSearchRange, 0, 0));
        }            
        #endregion

        //if(attackType != AttackType.none) Debug.Log("isAttacking: " + isAttacking);

        if (isDying) return;
        if (isAttacking) return;

        //if (!hasTarget) TargetCheck();
        TargetCheck();

        if (hasTarget) PlayAttackAnimation();
        else Move();
    }

    #endregion

    #region 생성 시 설정
    // 적 유닛일 경우, SpwanManager에서 호출
    public void SetEnemy()
    {
        isEnemy = true;
        //Debug.Log("SetEnemy");
    }

    // 적 캐릭터일 경우 반전
    void Flip()
    {
        transform.parent.Rotate(0, 180, 0);
        // ui 는 한번 더 뒤집어 원래대로 복원
        //UI_object.Rotate(0, -180, 0);
        hpBar.transform.Rotate(0, -180, 0);
    }

    // 자기 레이어 설정
    void SetLayer()
    {
        if (isEnemy) gameObject.layer = LayerMask.NameToLayer("Enemy");
        else targetLayer = LayerMask.NameToLayer("Player");
    }

    // 타겟 레이어 설정
    void SetTargetLayer()
    {
        if (isEnemy) targetLayer = 1 << LayerMask.NameToLayer("Player");
        else targetLayer = 1 << LayerMask.NameToLayer("Enemy");
    }
    #endregion

    void Move()
    {
        if (isAttacking) return;

        transform.parent.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    // 공격 대상 검색
    void TargetCheck()
    {       
        if (attackType == AttackType.none) return;

        int dir; // 바라보는 방향(오른쪽 => 1, 왼쪽 => -1)
        if (!isEnemy) dir = 1; else dir = -1;

        // 근접공격의 경우, 타겟 탐지거리는 공격거리보다 약간 더 짧음
        float tragetSearchRange = attackRange;
        if (attackType == AttackType.melee) tragetSearchRange -= 1;

        RaycastHit2D hit = Physics2D.Raycast(transform.parent.position + Vector3.up, Vector2.right * dir, tragetSearchRange, targetLayer);

        if (hit) hasTarget = true;
        else hasTarget = false;

    }

    #region 공격 관련 메소드

    // 공격 애니메이션에서 호출
    // 근접 => 범위 모든 대상 타격, 원거리 => 발사체 생성
    public void Attack()
    {
        //Debug.Log("attack");

        if (attackType == AttackType.none)
        {
            return;
        }
        if (attackType == AttackType.melee)
        {
            // OverlapBox2d로 범위 내 모든 적 알아오기
            Vector2 overlapBoxCenter = transform.parent.position + Vector3.up + new Vector3(attackRange/2, 0, 0) * dir;
            Vector2 boxSize = new Vector2(attackRange, 1);
            Collider2D[] hits = Physics2D.OverlapBoxAll(overlapBoxCenter, boxSize, 0);

            // 알아온 적 각각에 피해 주기
            for (int i = 0; i < hits.Length; i++)
            {
                int layerMask = 1 << hits[i].gameObject.layer;

                // targetLayer의 대상인지 검사
                if (layerMask == targetLayer)
                {
                    hits[i].GetComponentInChildren<Unit>().Hit(damage);
                }
            }
        }
        if (attackType == AttackType.range)
        {
            // 발사체 생성
            GameObject go = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            go.GetComponent<Projectile>().Init(targetLayer, damage, impact);
        }
        if (attackType == AttackType.targeting)
        {
            // OverlapBox2d로 범위 내, targetLayer에서 하나의 적 알아오기
            Vector2 overlapBoxCenter = transform.parent.position + Vector3.up + new Vector3(attackRange / 2, 0, 0) * dir;
            Vector2 boxSize = new Vector2(attackRange, 1);
            Collider2D hit = Physics2D.OverlapBox(overlapBoxCenter, boxSize, 0, targetLayer);

            // 알아온 적에게 피해 주기
            if (!hit) return;
            hit.GetComponentInChildren<Unit>().Hit(damage);
        }
    }

    // 공격 애니메이션 시작
    void PlayAttackAnimation()
    {
        if (isAttacking) return;

        if (attackType == AttackType.none) return;

        //Debug.Log("PlayAttackAnimation");

        isAttacking = true;
        animator.SetBool("attack", true);
    }

    // 공격 애니메이션 종료시, 애니메이터에서 호출    
    public void EndAttackAnimation()
    {
        if (!isAttacking) return;

        //Debug.Log("EndAttackAnimation");

        isAttacking = false;
        animator.SetBool("attack", false);
    }

    #endregion

    #region 피격 관련 메소드

    // 피해를 받음
    public void Hit(float amount)
    {
        if (isDying) return;

        // 피격 후 hp바 활성화
        if (!hpBar.gameObject.activeSelf)
            hpBar.gameObject.SetActive(true); 

        currentHp -= amount;

        // 피격 데미지 표기
        Vector3 textPos;
        if (damageTextPonit) textPos = damageTextPonit.position;
        else textPos = hpBar.gameObject.transform.position + Vector3.up * 0.5f;
        TextMaker.instance.CreateWolrdText(textPos, amount.ToString());
        // n초동안 시각적 피격 효과 적용
        SetHitEffect(hitEffectDuration);

        if (currentHp < 0) currentHp = 0;

        SyncSlider();

        if (currentHp == 0) Die();
    }

    void Die()
    {
        // 베이스 유닛 파괴 시
        if (unitType == UnitType.teamBase)
        {
            if (isEnemy) GameManager.instance.GameClear();
            else GameManager.instance.GameOver();

            // 폭발 프리팹이 존재하면 생성
            if (deathEffectPrefab) Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(transform.parent.gameObject);
        }
        // 일반 유닛 파괴 시
        else
        {
            coll.enabled = false; // 콜라이더 끄기
            hpBar.gameObject.SetActive(false); // hp 표기 중단
            PlayDeathAnimation(); // 사망 애니메이션 재생
        }
    }

    void PlayDeathAnimation()
    {
        isDying = true;

        if (animator)
        {
            animator.Rebind(); // 애니메이션 초기화 (normalized time 초기화 목적)
            animator.SetTrigger("die");            
        }
    }


    // 사망 애니메이션 종료 확인 -> 오브젝트 삭제
    void DyingAnimationCheck()
    {
        if (!isDying) return;
        
        // 사망 애니메이션 종료 시
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            //Debug.Log("Destroy:" + transform.parent.name);
            //Debug.Log("anim time:" + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

            // 폭발 프리팹이 존재하면 생성
            //if (deathEffectPrefab) Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            Destroy(transform.parent.gameObject);
        }        
    }

    void SyncSlider() // 슬라이더ui-현제 체력 동기화
    {
        hpBar.value = currentHp / maxHp;
    }
    
    // duartion 동안 피격효과 적용
    void SetHitEffect(float duartion)
    {
        hurtEndTime = Time.time + duartion;
    }

    // 항시실행. 피격효과 시간이면 transform에 흔들림 적용
    IEnumerator ShakeCheck(float amount, float interval)
    {
        Vector3 originPos = transform.localPosition; //스프라이트 본래 위치
        
        while (true)
        {
            // shake time이면 흔들림 적용, 아니면 본래 위치로.
            if (Time.time < hurtEndTime)
                transform.localPosition = (Vector3)Random.insideUnitCircle * amount + originPos;           
            else
                transform.localPosition = originPos;

            yield return new WaitForSeconds(interval);
        }        
    }

    // 항시 시행. 피격효과 시간이면 스프라이트에 붉은 색 표기
    IEnumerator BlinkCheck()
    {        
        while (true)
        {
            // shake time이면 흔들림 적용, 아니면 본래 위치로.
            if (Time.time < hurtEndTime)
                spriterRenderer.color = Color.red;             
            else
                spriterRenderer.color = Color.white;

            yield return null;
        }
    }

    #endregion
}
