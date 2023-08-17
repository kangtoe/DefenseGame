using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMaker : MonoBehaviour
{    
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static TextMaker instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<TextMaker>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private static TextMaker m_instance; // 싱글톤이 할당될 static 변수

    public GameObject textUI;   
    public Transform worldCanvas;
    public Transform camearaCanvas;    

    // 월드좌표 기준 텍스트 생성 (카메라 위치와 독립적)
    public void CreateWolrdText(Vector3 pos, string _text, Color color)
    {
        //Debug.Log("CreateText");
        Text txt = Instantiate(textUI, pos, Quaternion.identity, worldCanvas).GetComponent<Text>();
        txt.rectTransform.SetParent(worldCanvas);
        txt.text = _text;
        txt.color = color;
    }

    // 카메라 좌표 기준 텍스트 생성 (카메라 위치에 종속적)
    public void CreateCameraText(string _text, int fontSize = 60, float fadeSpeed =1, float moveSpeed = 0.5f, Vector3? pos = null)
    {
        if (pos == null) pos = Vector3.zero;
        Vector3 vec = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f)) + pos.Value;
        Text txt = Instantiate(textUI, vec, Quaternion.identity, camearaCanvas).GetComponent<Text>();
        txt.fontSize = fontSize;
        txt.rectTransform.SetParent(camearaCanvas);
        txt.text = _text;

        FloatingText floating = txt.GetComponent<FloatingText>(); ;
        floating.fadeSpeed = fadeSpeed;
        floating.moveSpeed = moveSpeed;
    }

    public void DebugFunc()
    {
        TextMaker.instance.CreateCameraText("Not Enough Gold!");
    }
}
