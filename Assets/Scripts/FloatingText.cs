using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Text selfText;
    Color alpha;

    public float moveSpeed;
    public float fadeSpeed; // 사라지는 속도

    void Start()
    {
        alpha = selfText.color;
    }

    float t;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, moveSpeed * Time.deltaTime, 0);

        // 이미지를 흐릿하게
        Color color = selfText.color;
        color.a = Mathf.Lerp(1, 0, t);
        t += Time.deltaTime * fadeSpeed;
        selfText.color = color;

        if (selfText.color.a < 0.1f) Destroy(gameObject);
    }
}
