using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 기존 ManaResource 기능 + ui 표기 + 골드 관리
public class PlayerResourceManager : MonoBehaviour
{
    public static PlayerResourceManager Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                instance = FindObjectOfType<PlayerResourceManager>();
            }

            // 싱글톤 오브젝트를 반환
            return instance;
        }
    }
    private static PlayerResourceManager instance; // 싱글톤이 할당될 static 변수

    public ResourceControl ManaResource => manaResource;
    [SerializeField]
    ResourceControl manaResource;

    public ResourceControl SoulResource => soulResource;
    [SerializeField]
    ResourceControl soulResource;

    // 보유한 자원을 표기할 텍스트
    public Text manaText; 
    public Text soulText;

    private void Start()
    {
        // 리소스 변경 시 UI를 갱신하는 리스너 등록
        manaResource?.onSetResource.AddListener(() => SetResourceText(manaResource.CurrentResource, manaText));
        soulResource?.onSetResource.AddListener(() => SetResourceText(soulResource.CurrentResource, soulText));

        // 기존 자원량 불러오기
        int resourceAmount = SaveManager.SaveData.currentResource;
        soulResource.SetResource(resourceAmount);
    }

    // ui에 현재 정보 표기
    void SetResourceText(float resourceAmount, Text text)
    {
        if (text == null) return;

        // 다섯 자릿수에서 없는 단위는 0으로 채우기
        string str = ((int)resourceAmount).ToString("00000");
        text.text = str;
    }

    public void SaveCurrentResource()
    {
        int currentAmount = (int)soulResource.CurrentResource;
        SaveManager.SaveResource(currentAmount);
    }

    // 디버그용
    public void SetResource(int amount)
    {
        soulResource.SetResource(amount);
        SetResourceText(soulResource.CurrentResource, soulText);
        SaveCurrentResource();
    }
}
