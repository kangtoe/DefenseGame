using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1f;

    Animator anim;

    //[SerializeField]
    //int dir = 1; // 1 : 오른쪽, -1: 왼쪽

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int xInput = (int)Input.GetAxisRaw("Horizontal");

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            // FlipCheck
            int _dir = (transform.right.x > 0 ? 1 : -1);
            if (_dir != xInput)
            {
                Flip();
            }                                  
        }

        // 입력에 따른 이동
        float xMove = moveSpeed * Time.deltaTime * Mathf.Abs(xInput);
        anim.SetFloat("move", Mathf.Abs(xMove));
        transform.Translate(xMove, 0, 0);

        // 이동 제한
        LimitMove();
    }

    void Flip()
    {
        Debug.Log("flip");
        transform.Rotate(0, 180, 0);
    }

    void LimitMove()
    {
        float margin = 0.05f;

        // 카메라를 벗어나지 않도록 범위 제한
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0 + margin, 1 - margin);        
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
