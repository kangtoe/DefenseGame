using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 애니메이션 종료 후 오브젝트 삭제
public class RemoveEffect : MonoBehaviour
{
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            Destroy(gameObject);
    }
}
