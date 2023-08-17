using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum Scene
{ 
    Undefined = 0,
    Title,
    Play,
    Shop,
    Armory, // Unit&Skill Upgrade
    Base, // Hero&Base Upgrade
}

public class CustomSceneManager : MonoBehaviour
{
    public static CustomSceneManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<CustomSceneManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private static CustomSceneManager m_instance; // 싱글톤이 할당될 static 변수

    [Header("Scene 이름 상수")]
    [SerializeField]
    string TITLE_SCENE_NAME = "TitleScene";
    [SerializeField]
    string PLAY_SCENE_NAME = "PlayScene";
    [SerializeField]
    string SHOP_SCENE_NAME = "UnitScene";

    [Header("시작 시 씬 이름 표기")]
    [SerializeField]
    bool showSceneOnStart = true;

    [Header("현재 Scene")]
    public Scene currentScene;

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

        // 현재 씬 알아오기
        string name = SceneManager.GetActiveScene().name;
        currentScene = SceneStrToEnum(name);
    }

    public void ToScene(string sceneName)
    {
        StartCoroutine(SceneChangeCr(sceneName));
    }

    #region Scene Sting <-> Enum 변환
    string SceneEnumToStr(Scene scene)
    {
        if (scene == Scene.Title) return TITLE_SCENE_NAME;
        if (scene == Scene.Shop) return SHOP_SCENE_NAME;
        if (scene == Scene.Play) return PLAY_SCENE_NAME;        

        Debug.Log("No Enum Type Found: " + scene);
        return null;
    }
    Scene SceneStrToEnum(string str)
    {
        if (str == TITLE_SCENE_NAME) return Scene.Title;
        if (str == SHOP_SCENE_NAME) return Scene.Shop;
        if (str == PLAY_SCENE_NAME) return Scene.Play;        

        //Debug.Log("No Available Str: " + str);
        return Scene.Undefined;
    }

    #endregion

    // scene 에서 나갈때 호출, 인수는 전환 대상 scene
    IEnumerator SceneChangeCr(string seceneName)
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
        //Debug.Log("SceneIn");

        if (showSceneOnStart)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            TextMaker.instance.CreateCameraText(sceneName, 120, 0.75f, 0.33f);
        }        

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
