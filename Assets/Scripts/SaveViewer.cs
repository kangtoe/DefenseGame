using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveViewer : MonoBehaviour
{
    static SaveViewer instance;

    [SerializeField]
    SaveData data;

    // Start is called before the first frame update
    void Start()
    {
        if (!instance)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        data = SaveManager.CurrentData;
    }
}
