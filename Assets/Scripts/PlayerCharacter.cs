using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1f;

    Animator anim;

    [SerializeField]
    int dir = 1; // 1 : 오른쪽, -1: 왼쪽

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {        
        float vAxis = Input.GetAxisRaw("Vertical");
        
        transform.Translate(vAxis * moveSpeed * Time.deltaTime ,0 ,0);

        FlipCheck();
    }

    void FlipCheck()
    {
        int _dir = (transform.localScale.x > 0 ? 1 : -1);        
    }

    void Flip()
    { 
    
    }
}
