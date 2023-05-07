using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// animator에 unit 스크팁트의 event를 전달하는 역할
public class AnimationEventParser : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnAttack;
    [HideInInspector]
    public UnityEvent OnEndAttack;


    public void Atttack()
    {
        OnAttack.Invoke();
    }

    public void EndAttack()
    {
        OnEndAttack.Invoke();
    }
}
