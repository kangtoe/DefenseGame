using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager_FocusOnCharacter : MonoBehaviour
{
    [Space]
    // ī�޶� ���� ���� ����
    public bool camearaControl = true;
    [Space]

    [SerializeField]
    Camera myCamera;

    // ���� ����
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

    // ���콺 x�� �巡�� ���� ����Ͽ� ī�޶� �̵�
    // ī�޶� �̵� ���� ����
    void LimitCameraMove()
    {
        // ī�޶� �ʺ��� ������ ������ǥ�� ���ϱ�
        Vector2 cameraCenter = myCamera.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
        Vector2 cameraRightEnd = myCamera.ViewportToWorldPoint(new Vector2(1f, 0.5f));
        float cameraWidthHalf = (cameraRightEnd - cameraCenter).x;

        // ī�޶� �ּ�, �ִ� ��ġ
        float minX = cameraAreaStart.position.x + cameraWidthHalf;
        Vector3 cameraMin = new Vector3(minX, myCamera.transform.position.y, myCamera.transform.position.z);
        float maxX = cameraAreaEnd.position.x - cameraWidthHalf;
        Vector3 cameraMax = new Vector3(maxX, myCamera.transform.position.y, myCamera.transform.position.z);


        // ī�޶� �̵�����
        if (myCamera.transform.position.x < cameraMin.x) // �ּҿ��� ���
        {
            //Debug.Log("camera min area");
            myCamera.transform.position = cameraMin;
        }
        if (myCamera.transform.position.x > cameraMax.x) // �ִ뿵�� ���
        {
            //Debug.Log("camera max area");
            myCamera.transform.position = cameraMax;
        }
    }
}
