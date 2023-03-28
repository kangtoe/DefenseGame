using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 드레그 입력 감지, 그에 따른 카메라 이동 제어 
public class CameraManager_ClickNMove : MonoBehaviour
{
    [Space]
    // 카메라 조작 가능 여부
    public bool camearaControl = true;
    [Space]

    public Camera myCamera;

    // 영역 제한
    public Transform cameraAreaStart;
    public Transform cameraAreaEnd;

    // 마우스
    Vector2 mousebefore; // 마우스 이전 위치 (클릭 후에만 갱신)
    Vector2 mouseNow; // 마우스 현재 위치
    bool onMouse; // 마우스 입력 중인가?

    

    // 카메라 이동 민감도
    public float moveSensitivity = 1f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LimitCameraMove());
    }

    // Update is called once per frame
    void Update()
    {
        if (!camearaControl) return;

        // 마우스 클릭 상태 검사
        if (Input.GetMouseButton(0)) onMouse = true;
        else onMouse = false;

        // 마우스 입력 지점 기억
        if (Input.GetMouseButtonDown(0)) mousebefore = Input.mousePosition;
        mouseNow = Input.mousePosition;                
    }

    // 마우스 x축 드래그 값에 비례하여 카메라 이동
    // 카메라 이동 영역 제한
    IEnumerator LimitCameraMove()
    {
        // 카메라 너비의 절반을 월드좌표로 구하기
        Vector2 cameraCenter = myCamera.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
        Vector2 cameraRightEnd = myCamera.ViewportToWorldPoint(new Vector2(1f, 0.5f));
        float cameraWidthHalf = (cameraRightEnd - cameraCenter).x;

        // 카메라 최소, 최대 위치
        float minX = cameraAreaStart.position.x + cameraWidthHalf;
        Vector3 cameraMin = new Vector3(minX, myCamera.transform.position.y, myCamera.transform.position.z);
        float maxX = cameraAreaEnd.position.x - cameraWidthHalf;
        Vector3 cameraMax = new Vector3(maxX, myCamera.transform.position.y, myCamera.transform.position.z);

        while (true)
        {
            yield return null;

            if (!camearaControl) continue;
            if (!onMouse) continue;            

            // 마우스 x축 드래그값
            float xDelta = mousebefore.x - mouseNow.x;
            mousebefore = mouseNow;

            // 카메라 이동
            myCamera.transform.Translate(xDelta * moveSensitivity * 0.01f, 0, 0);

            // 카메라 이동제한
            if (myCamera.transform.position.x < cameraMin.x) // 최소영역 벗어남
            {
                //Debug.Log("camera min area");
                myCamera.transform.position = cameraMin;
            }
            if (myCamera.transform.position.x > cameraMax.x) // 최대영역 벗어남
            {
                //Debug.Log("camera max area");
                myCamera.transform.position = cameraMax;
            }
        }

        
    }
}
