using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private static GameManager m_instance; // 싱글톤이 할당될 static 변수    

    public GameObject pausePanel; // 게임 오버 ui
    public GameObject overPanel; // 게임 오버 ui
    public GameObject clearPanel; // 게임 클리어 ui
    public Image blackOutImage; // scene 전환 시 fill 값을 조절하여 효과를 줌

    public Transform playerUnits; // 모든 플레이어 유닛의 부모
    public Transform enemyUnits; // 모든 적 유닛의 부모

    public bool isPlaying = true; // 게임이 진행 중인가? clear, over 이후 fasle 

    // 활동 영역 제한 (unit이 이를 벗어나면 삭제)
    public Transform UnitAreaStartX;
    public Transform UnitAreaEndX;

    // Start is called before the first frame update
    void Start()
    {
        if(overPanel) overPanel.SetActive(false);
        if(clearPanel) clearPanel.SetActive(false);
        if(pausePanel) pausePanel.SetActive(false);
    }

    public void GamePause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GameResume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        isPlaying = false;
        overPanel.SetActive(true);
        // 모든 아군 유닛 사망
        KillAllChildUnit(playerUnits);
    }

    public void GameClear()
    {
        isPlaying = false;
        clearPanel.SetActive(true);
        // 모든 적군 유닛 사망
        KillAllChildUnit(enemyUnits);
    }    

    // parent의 모든 자식의 Unit에 즉사에 해당하는 피해를 줌 
    public void KillAllChildUnit(Transform parent)
    {
        int cnt = parent.childCount;

        for (int i = 0; i < cnt; i++)
        {
            // 자식이 unit 스크립트를 가지고 있는 경우, unit에게 즉사에 해당하는 피해
            Unit unit = parent.GetChild(i).GetComponentInChildren<Unit>();
            if (unit) unit.OnHit(99999);
        }
    }

    // 해당 위치가 Unit area 안에 있는 지 확인 (unit이 자신의 위치를 확인하기 위해 호출)
    public bool CheckInUnitArea(Vector3 pos)
    {
        if (pos.x < UnitAreaStartX.position.x) return false;
        if (UnitAreaEndX.position.x < pos.x) return false;

        return true;
    }
}
