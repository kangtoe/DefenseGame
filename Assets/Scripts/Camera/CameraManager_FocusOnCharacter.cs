using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager_FocusOnCharacter : MonoBehaviour
{
    public static CameraManager_FocusOnCharacter Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                instance = FindObjectOfType<CameraManager_FocusOnCharacter>();
            }

            // 싱글톤 오브젝트를 반환
            return instance;
        }
    }
    private static CameraManager_FocusOnCharacter instance; // 싱글톤이 할당될 static 변수 

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
    public Transform FocusCharacter;

    float moveSpeed;
    
    void Start()
    {
        moveSpeed = FocusCharacter.GetComponent<Unit>().moveSpeed;
    }
    
    void Update()
    {
        Vector3 vec = myCamera.transform.position;

        if (FocusCharacter)
        {
            vec.x = FocusCharacter.position.x;
            myCamera.transform.position = vec;
        }
        else
        {
            // 입력에 따른 이동
            float xMove = moveSpeed * Time.deltaTime * InputManager.instance.MoveX;
            myCamera.transform.Translate(xMove, 0, 0);
        }        

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
