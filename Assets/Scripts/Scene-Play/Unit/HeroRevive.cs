using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroRevive : MonoBehaviour
{
    public static HeroRevive Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                instance = FindObjectOfType<HeroRevive>();
            }

            // 싱글톤 오브젝트를 반환
            return instance;
        }
    }
    private static HeroRevive instance; // 싱글톤이 할당될 static 변수 

    [SerializeField]
    Text reviveText;

    [SerializeField]
    int reviveTime = 10;

    [SerializeField]
    GameObject reviveEffect;

    [SerializeField]
    GameObject heroPrefab;

    // 부활지점 = 카메라 중앙 + Offset
    [SerializeField]
    float revivePos = -4.86f;

    // Start is called before the first frame update
    void Start()
    {
        reviveText.gameObject.SetActive(false);
        Revive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }    

    public void StartReviveCr()
    {
        StartCoroutine(ReviveCr());
    }

    void Revive()
    {
        Debug.Log("revive");
        
        // 생성
        Vector3 point = new Vector3 (Camera.main.transform.position.x, revivePos, revivePos);
        GameObject go = Instantiate(heroPrefab, point, heroPrefab.transform.rotation);

        // 카메라 추적 설정
        CameraManager_FocusOnCharacter.Instance.FocusCharacter = go.transform;

        // 부활 이팩트
        Vector3 effectPoint = point + new Vector3(0, 3.6f, 0.001f);
        GameObject vfx = Instantiate(reviveEffect, effectPoint, Quaternion.identity);
        vfx.GetComponent<FollowTarget>().target = go.transform;
    }

    IEnumerator ReviveCr()
    {
        reviveText.gameObject.SetActive(true);
        int waitTime = reviveTime;

        while (waitTime >= 0)
        {
            reviveText.text = "REVIVE IN : " + waitTime;
            waitTime--;
            yield return new WaitForSeconds(1);
        }

        reviveText.gameObject.SetActive(false);
        Revive();
    }
}
