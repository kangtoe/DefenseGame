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
    hero = 97,
    teamBase = 99 // 팀 베이스. 파괴시 패배
}

public class Unit : MonoBehaviour
{    
    // 컴포넌트
    Animator anim;
    //Transform UI_object; // 적 캐릭터 반전 시 ui는 반전 대상에서 제외    
    Slider hpBar;
    SpriteRenderer sprite;
    Collider2D coll;
    AnimationEventParser parser;

    [Header("발사체")]
    public Transform firePoint; // 원거리 발사체 생성 위치
    public GameObject bulletPrefab;

    [Header("유닛 수치 정보")]
    public UnitType unitType;
    public AttackType attackType;
    public float maxHp;
    float currentHp;
    public float moveSpeed;
    public float attackRange; // 공격거리 == 적 탐색거리
    public float damage;
    public int impact; // 공격의 충격량    

    [Header("타격 & 식별 정보")]
    public bool isEnemy = false; // 적(오른쪽에서 등장, 왼쪽으로 진행)인가?
    LayerMask targetLayer; // 공격 대상 레이어
    bool isDying = false; // 사망 애니메이션 재생 중
    bool isAttacking = false; // 공격 애니메이션 재생 중
    float hurtEndTime = 0f; // 피격 효과 적용이 끝나는 시간 (현재시간+지속기간)
    float hitEffectDuration = 0.1f;
    int dir; // 바라보는 방향(오른쪽 => 1, 왼쪽 => -1)

    [Header("피격 & 사망")]
    public Transform damageTextPonit; // 피해량 표기 위치(null인 경우 기본 위치는 hp bar 위쪽)
    public GameObject deathEffectPrefab;

    #region 유니티 라이프 사이클

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Unit start : " + transform.name);

        coll = GetComponentInChildren<Collider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();        
        hpBar = GetComponentInChildren<Slider>();
        anim = GetComponentInChildren<Animator>();
        if (unitType != UnitType.teamBase)
        {
            parser = GetComponentInChildren<AnimationEventParser>();
            parser.OnAttack.AddListener(Attack);
            parser.OnEndAttack.AddListener(EndAttackAnimation);
        }        

        // 바라보는 방향(오른쪽 => 1, 왼쪽 => -1)
        if (!isEnemy) dir = 1; else dir = -1;

        currentHp = maxHp;
        SyncSlider();
        hpBar.gameObject.SetActive(false); // 피격 전까지는 hp바 숨기기

        // 적일 경우 뒤집기
        if (isEnemy) Flip();

        // 자신 레이어 설정
        if (isEnemy) coll.gameObject.layer = LayerMask.NameToLayer("Enemy");
        else coll.gameObject.layer = LayerMask.NameToLayer("Player");

        // 타겟 레이어 설정
        if (isEnemy) targetLayer = 1 << LayerMask.NameToLayer("Player");
        else targetLayer = 1 << LayerMask.NameToLayer("Enemy");

        // 피격 효과를 검사하는 코루틴
        //StartCoroutine(ShakeCheck(0.2f, 0.05f));
        StartCoroutine(BlinkCheck());        
    }

    // Update is called once per frame
    void Update()
    {        
        // 활동 영역을 벗어나게 되면 유닛 삭제
        if (!GameManager.instance.CheckInUnitArea(transform.position))
        {
            Destroy(transform.gameObject);
        }         

        DyingAnimationCheck();

        //if(attackType != AttackType.none) Debug.Log("isAttacking: " + isAttacking);

        if (isDying) return;        

        bool hasTarget = HasTarget();
        if (hasTarget) PlayAttackAnimation();

        Move();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // 공격 거리 표기
        if (attackType != AttackType.none)
        {
            // 근접공격의 경우, 타겟 탐지거리는 공격거리보다 약간 더 짧음
            float tragetSearchRange = attackRange;
            if (attackType == AttackType.melee) tragetSearchRange -= 0.5f;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + new Vector3(dir * tragetSearchRange, 0, 0));
        }        
    }

    #endregion

    // 적 유닛일 경우, SpwanManager에서 호출
    public void SetEnemy()
    {
        isEnemy = true;
        //Debug.Log("SetEnemy");
    }

    #region 이동 관련

    void Move()
    {
        if (unitType == UnitType.hero)
        {
            // 입력 방향
            int xInput = (int)Input.GetAxisRaw("Horizontal");
            // 현재 바라보는 방향
            int _dir = (transform.right.x > 0 ? 1 : -1);
            
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                //Debug.Log("Input false");
                // 이동 입력 없는 경우, 오른쪽을 바라보도록                      
                if (_dir != 1)
                {
                    Flip();
                }
            }
            else
            {
                //Debug.Log("Input true");
                // 이동 입력 있는 경우, 입력 방향을 바라보도록
                if (_dir != xInput)
                {
                    Flip();
                }

                if (isAttacking)
                {
                    Debug.Log("stop attack");
                    EndAttackAnimation();
                } 
            }

            // 입력에 따른 이동
            float xMove = moveSpeed * Time.deltaTime * Mathf.Abs(xInput);
            anim.SetFloat("move", Mathf.Abs(xMove));
            transform.Translate(xMove, 0, 0);

            // 이동 제한
            LimitMove();
        }
        else
        {
            if (isAttacking) return;

            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }        
    }
    
    void LimitMove()
    {
        float margin = 0.05f;

        // 카메라를 벗어나지 않도록 범위 제한
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0 + margin, 1 - margin);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    // 적 캐릭터일 경우 반전
    void Flip()
    {
        transform.Rotate(0, 180, 0);

        // ui는 한번 더 뒤집어 원래대로 복원        
        hpBar.transform.Rotate(0, 180, 0);
    }

    #endregion

    #region 공격 관련 메소드

    // 공격 대상 검색
    bool HasTarget()
    {
        if (attackType == AttackType.none) return false;

        int dir; // 바라보는 방향(오른쪽 => 1, 왼쪽 => -1)
        if (!isEnemy) dir = 1; else dir = -1;

        // 근접공격의 경우, 타겟 탐지거리는 공격거리보다 약간 더 짧음
        float tragetSearchRange = attackRange;
        if (attackType == AttackType.melee) tragetSearchRange -= 1;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up, transform.right, tragetSearchRange, targetLayer);

        if (hit) return true;
        else return false;
    }

    // 공격 애니메이션에서 호출
    // 근접 => 범위 모든 대상 타격, 원거리 => 발사체 생성
    public void Attack()
    {
        //Debug.Log(name + " : attack");

        if (attackType == AttackType.none)
        {
            return;
        }
        if (attackType == AttackType.melee)
        {
            // OverlapBox2d로 범위 내 모든 적 알아오기
            Vector2 overlapBoxCenter = transform.position + Vector3.up + new Vector3(attackRange/2, 0, 0) * dir;
            Vector2 boxSize = new Vector2(attackRange, 1);
            Collider2D[] hits = Physics2D.OverlapBoxAll(overlapBoxCenter, boxSize, 0, targetLayer);

            // 알아온 적 각각에 피해 주기
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].attachedRigidbody.GetComponent<Unit>().Hit(damage);
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
            Vector2 overlapBoxCenter = transform.position + Vector3.up + new Vector3(attackRange / 2, 0, 0) * dir;
            Vector2 boxSize = new Vector2(attackRange, 1);
            Collider2D hit = Physics2D.OverlapBox(overlapBoxCenter, boxSize, 0, targetLayer);

            // 알아온 적에게 피해 주기
            if (!hit) return;
            hit.attachedRigidbody.GetComponent<Unit>().Hit(damage);
        }
    }

    // 공격 애니메이션 시작
    void PlayAttackAnimation()
    {
        if (isAttacking) return;

        if (attackType == AttackType.none) return;

        //Debug.Log("PlayAttackAnimation");

        isAttacking = true;
        anim.SetBool("attack", true);
    }

    // 공격 애니메이션 종료시, 애니메이터에서 호출    
    public void EndAttackAnimation()
    {
        if (!isAttacking) return;

        //Debug.Log("EndAttackAnimation");

        isAttacking = false;
        anim.SetBool("attack", false);
    }

    #endregion

    #region 피격 관련 메소드

    // 피해를 받음
    public void Hit(float amount)
    {
        //Debug.Log(name + " : Hit");

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
            Destroy(transform.gameObject);
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

        if (anim)
        {
            anim.Rebind(); // 애니메이션 초기화 (normalized time 초기화 목적)
            anim.SetTrigger("die");            
        }
    }


    // 사망 애니메이션 종료 확인 -> 오브젝트 삭제
    void DyingAnimationCheck()
    {
        if (!isDying) return;
        
        // 사망 애니메이션 종료 시
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            //Debug.Log("Destroy:" + transform.parent.name);
            //Debug.Log("anim time:" + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

            // 폭발 프리팹이 존재하면 생성
            //if (deathEffectPrefab) Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            Destroy(transform.gameObject);
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
                sprite.color = Color.red;             
            else
                sprite.color = Color.white;

            yield return null;
        }
    }

    #endregion
}
