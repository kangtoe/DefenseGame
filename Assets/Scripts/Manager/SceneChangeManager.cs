using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<SceneChangeManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private static SceneChangeManager m_instance; // 싱글톤이 할당될 static 변수

    public Image blackOutImage; // scene 전환 시 fill 값을 조절하여 효과를 줌

    // scene 전환 효과 중인가?
    // blackOutImage의 fill 값을 조정하는 코루틴 중복 실행 방지
    bool isChangeEffecting = false;

    // Start is called before the first frame update
    void Start()
    {
        // 타이틀 scene에서는 진입효과 넣지 않음
        //if (SceneManager.GetActiveScene().name != "TitleScene")
        StartCoroutine(SceneIn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // title scene, play 버튼에서 호출
    // play scene, panel의 재시작 버튼에서 호출
    public void GameStart()
    {
        StartCoroutine(SceneChange("PlayScene"));
    }

    public void ToTitleScene()
    {
        StartCoroutine(SceneChange("TitleScene"));
    }

    // scene 에서 나갈때 호출, 인수는 전환 대상 scene
    IEnumerator SceneChange(string seceneName)
    {
        Time.timeScale = 1f; // GamePause가 먼저 호출된 경우 대비

        if (isChangeEffecting) yield break;
        isChangeEffecting = true;
        blackOutImage.gameObject.SetActive(true);

        float fill = 0;
        while (fill < 1)
        {
            fill += Time.deltaTime;
            blackOutImage.fillAmount = fill;
            yield return new WaitForEndOfFrame();
        }

        isChangeEffecting = false;
        SceneManager.LoadScene(seceneName);
    }
    // scene에 새롭게 진입했을 떄 호출
    IEnumerator SceneIn()
    {
        if (isChangeEffecting) yield break;
        isChangeEffecting = true;
        blackOutImage.gameObject.SetActive(true);

        float fill = 1;
        while (fill > 0)
        {
            fill -= Time.deltaTime;
            blackOutImage.fillAmount = fill;
            yield return new WaitForEndOfFrame();
        }

        isChangeEffecting = false;
        blackOutImage.gameObject.SetActive(false);
    }
}
