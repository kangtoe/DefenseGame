using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Vector2 vec = transform.position;
            vec.x = target.position.x;
            transform.position = vec;
        }
        else Debug.Log("on target!");
    }
}
