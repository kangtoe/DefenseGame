using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType
{ 
    Undefinded = 0,
    Spwan,
    Upgrade,
    Empty = 98,
}

// 버튼 초기화
// 버튼에 생산 등록한 유닛의 생산 시간 조절& ui반영
// 버튼 fade 이미지(delay time ui)의 filled 영역 조절
public class ButtonContoller : MonoBehaviour
{
    // 생산할 유닛 미리보기 이미지
    //public Image unitIcon; 
    // 유닛 아이콘 관련 값
    //public Vector2 iconPos; // sprite x축, y축 조정값
    //public Vector2 iconSize; // sprite의 사이즈

    // 버튼 타입 : 클릭 시 동작 정의
    public ButtonType buttonType;

    // 버튼 ui
    public Button button;    
    public Image fadeImage; // filled 영역 조절할 이미지
    public Image lockImage; // 유닛 사용 잠금 이미지
    public Image OutlineImage; // filled 영역 조절할 이미지
    public Text costText; // 유닛의 생산 비용을 표기하는 텍스트
    public Text equiptText; // 유닛의 사용 여부를 표기하는 텍스트
    public Text levelText; // 유닛 강화 단계 표시

    public bool IsEquipted => isEquipted;
    bool isEquipted = false;
    
    public bool IsUnlocked => upgradable && upgradable.CurrentLevel > 0;

    public GameObject UnitPrefab => unitPrefab;
    // 스폰할 유닛 관련 변수
    GameObject unitPrefab; // 버튼에 스폰을 등록된 유닛 프리팹 (unit pallet에서 초기화)    
    Spwanable spwanable;
    Upgradable upgradable;
    //Unit unit;

    // 스폰 관련 변수
    float lastSpwanTime = 0f; // 마지막으로 유닛을 스폰한 시간
    bool isSpwanCooltime = false; // 스폰 쿨타임 중인가? (스폰 불가 시간인가?)

    // 플레이어 관리자
    UnitSpwaner spwanManager;
    ResourceControl manaManager;

    #region 유니티 라이프 사이클
    // Start is called before the first frame update
    void Start()
    {        
        //InitButton();
    }

    // Update is called once per frame
    void Update()
    {
        // 스폰 딜레이 시간 중에만 fade 이미지 fill값 갱신
        if (!isSpwanCooltime) return;

        //Debug.Log("isSpwanCooltime: "+ isSpwanCooltime); 
    }

    #endregion
    
    public void InitButton(GameObject spwanUnit)
    {
        Debug.Log("InitButton");

        //unit = unitPrefab.GetComponent<Unit>();
        unitPrefab = spwanUnit;
        spwanable = unitPrefab.GetComponent<Spwanable>();
        upgradable = unitPrefab.GetComponent<Upgradable>();
        spwanManager = PlayerSpwaner.Instance;
        manaManager = PlayerResourceManager.Instance.ManaResource;

        // 유닛 레벨 표기
        levelText.text = "lv." + upgradable.CurrentLevel;
        levelText.enabled = IsUnlocked;
        
        // 유닛 생산 비용 표기        
        costText.text = spwanable.price.ToString();
        costText.enabled = IsUnlocked;

        // 사용 중 텍스트 활성화 여부
        equiptText.enabled = IsEquipted;
        
        // 잠금 이미지 활성화 여부
        lockImage.enabled = !IsUnlocked;                                

        switch (buttonType)
        {
            case ButtonType.Spwan:
                equiptText.enabled = false;
                levelText.enabled = false;
                // onClick에 리스너 등록
                button.onClick.AddListener(TrySpwanUnit);                
                // 시작 시 스폰 쿨타임 적용
                StartCoroutine(SyncFilledImage());
                break;

            case ButtonType.Upgrade:
                button.onClick.AddListener(delegate {
                    // 현재 관리 중인 오브젝트로 지정                    
                    ArmoryManager.Instance.SetSelectedUnit(this);   
                });                                
                break;

            default:
                Debug.Log("유효하지 않은 버튼 타입 : " + buttonType);
                break;            
        }  
    }

    // 사용 여부 반전
    public void ToggleEquipted()
    {
        isEquipted =! isEquipted;

        equiptText.enabled = isEquipted;

        if (isEquipted)
        { 
            // 스폰 리스트에 추가
        }
        if (!isEquipted)
        { 
            // 스폰 리스트에서 제거
        }
    }

    #region 스폰버튼 처리

    // button의 onClick에서 호출됨
    void TrySpwanUnit()
    {
        Debug.Log("TrySpwanUnit");

        // 아직 스폰 딜레이 시간 중
        if (isSpwanCooltime) return;

        // 게임 메니저에서 돈이 부족해서 스폰에 실패한 경우
        if (!manaManager.TrySpendResource(spwanable.price))
        {
            TextMaker.instance.CreateCameraText("Not Enough Mana!");
            return;
        }

        // 스폰 성공 처리
        spwanManager.SpwanUnit(unitPrefab);
        lastSpwanTime = Time.time;
        isSpwanCooltime = true;
        StartCoroutine(SyncFilledImage());
    }

    // fade 이미지 fillAmount 갱신
    // isSpwanCooltime 체크
    IEnumerator SyncFilledImage()
    {
        float ratio; // fill 비율 (쿨타임 시작시 1, 종료시 0)
        float leftSpwanTime = spwanable.spwanCooltime;

        isSpwanCooltime = true; // 스폰 쿨타임 중인가

        while (true)
        {
            // 남은 쿨타임 / 전체 쿨타임 비율 구해서 적용
            ratio = leftSpwanTime / spwanable.spwanCooltime;
            fadeImage.fillAmount = ratio;

            leftSpwanTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();

            // 스폰 쿨타임 종료
            if (leftSpwanTime <= 0)
            {
                fadeImage.fillAmount = 0;
                isSpwanCooltime = false;
                break;
            }
        }
    }

    #endregion

    #region 상점버튼 처리

    public void OnSelected()
    {
        OutlineImage.enabled = true;
    }

    public void OnDeselected()
    {
        OutlineImage.enabled = false;
    }

    #endregion
}
