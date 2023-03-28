using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1f;

    Animator anim;

    [SerializeField]
    int dir = 1; // 1 : ������, -1: ����

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {                
        // �Է¿� ���� �̵�
        float xMove = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
        transform.Translate(xMove, 0 ,0);
        anim.SetFloat("move", Mathf.Abs(xMove));

        // �̵� ����
        LimitMove();

        // �ø�
        if (xMove >= 0) dir = 1;
        else dir = -1;
        FlipCheck();
    }

    void FlipCheck()
    {
        int _dir = (transform.localScale.x > 0 ? 1 : -1);

        if (_dir != dir)
        {
            Flip();
        }
    }

    void Flip()
    {
        transform.localScale *= new Vector2(-1, 1);
    }

    void LimitMove()
    {
        float margin = 0.05f;

        // ī�޶� ����� �ʵ��� ���� ����
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0 + margin, 1 - margin);        
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
