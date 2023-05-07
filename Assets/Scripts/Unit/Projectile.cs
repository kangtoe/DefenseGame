using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject effect; // 피격 시 효과 프리팹

    float damage;
    float impact; // 피격된 대상을 밀어내는 힘
    LayerMask targetLayer;

    public float moveSpeed;    
    public float liveTime = 2f; // 자동 소멸 시간

    // 처음 타격 인가? 
    // 적과 충돌하면 Destroy()를 호출하지만, 이는 다음 프레임에 발생함
    // 때문에 하나의 총알이 사라지긴 전 둘 이상의 콜라이더와 충돌했는 지 검사하기 위함
    // 광역 공격을 구현하려면 구조 개선 필요
    bool firstStrike = true;    

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    // Unit에서 총알을 생성할때 호출
    public void Init(LayerMask _targetLayer, float _damage, int _impact)
    {
        targetLayer = _targetLayer;
        damage = _damage;
        impact = _impact;
    }

    public void SetTargetLayer(LayerMask layerMask)
    {
        targetLayer = layerMask;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (!firstStrike) return;

        int layerMask = 1 << coll.gameObject.layer;

        // 레이어 마스크에 포함된 마스크인지 검사
        if ((targetLayer & layerMask) == targetLayer)
        {
            //Debug.Log("hit");

            Unit obj = coll.attachedRigidbody.GetComponent<Unit>();

            obj.OnHit(damage);

            if(effect) Instantiate(effect, transform.position, Quaternion.identity);

            firstStrike = false;

            Destroy(gameObject);
        }
    }
}
