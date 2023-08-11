using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SaveData
{ 
    
}

public class SaveManager : MonoBehaviour
{
    SaveManager Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                instance = FindObjectOfType<SaveManager>();
            }

            // 싱글톤 오브젝트를 반환
            return instance;
        }
    }
    private static SaveManager instance; // 싱글톤이 할당될 static 변수    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
