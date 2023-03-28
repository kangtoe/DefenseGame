using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager_FocusOnCharacter : MonoBehaviour
{
    [Space]
    // 카메라 조작 가능 여부
    public bool camearaControl = true;
    [Space]

    [SerializeField]
    Camera myCamera;

    // 영역 제한
    [SerializeField]
    Transform cameraAreaStart;
    [SerializeField]
    Transform cameraAreaEnd;

    [SerializeField]
    Transform FocusCharacter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vec = myCamera.transform.position;

        vec.x = FocusCharacter.position.x;
        myCamera.transform.position = vec;

        LimitCameraMove();
    }

    // 마우스 x축 드래그 값에 비례하여 카메라 이동
    // 카메라 이동 영역 제한
    void LimitCameraMove()
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
