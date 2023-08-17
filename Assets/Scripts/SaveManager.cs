using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;


[Serializable]
public class SaveData
{
    // 현재 자원
    public int currentResource;

    // 사용 중인 유닛 이름 리스트
    public List<string> usingUnitNames;

    // 사용 중인 스킬 이름 리스트
    public List<string> usingSkillNames;

    public SaveData()
    {
        currentResource = 0;
        usingUnitNames = null;
        usingSkillNames = null;
    }

    public bool IsUsing(GameObject go)
    {
        if (usingUnitNames != null && usingUnitNames.Exists(x => x == go.name)) return true;
        if (usingSkillNames != null && usingSkillNames.Exists(x => x == go.name)) return true;

        return false;
    }
}

public static class SaveManager
{
    public static SaveData SaveData
    {
        get
        {
            if (saveData == null) saveData = Load();
            return saveData;
        }
    }
    static SaveData saveData;

    // 세이브 데이터 경로와 이름
    static string SAVE_FILE_NAME => "save.dat";
    static string SAVE_PATH => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    // Resources 폴더 아래 유닛 폴더 상대 경로
    static string UNIT_PATH = "Units_f4/";

    #region 저장

    public static void Save(SaveData _data)
    {
        BinaryFormatter formatter = new BinaryFormatter();        
        FileStream stream = new FileStream(SAVE_PATH, FileMode.Create);        
        formatter.Serialize(stream, _data);
        stream.Close();

        saveData = _data;

        Debug.Log("saved on : " + SAVE_PATH);
    }

    // currentResource 필드만 수정
    public static void SaveResource(int amount)
    {
        // 기존 데이터 수정
        saveData.currentResource = amount;
        
        Save(saveData);
    }

    // usingUnitNames 필드만 수정
    public static void SaveUsingUnits(List<GameObject> usingUnits)
    {
        Debug.Log("SaveUsingUnits");        

        // 기존 데이터 수정
        List<string> names = GameObjectListToStringList(usingUnits);
        saveData.usingUnitNames = names;
        
        Save(saveData);
    }

    #endregion

    #region 불러오기

    public static SaveData Load()
    {
        if (File.Exists(SAVE_PATH))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SAVE_PATH, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;

            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file not found in" + SAVE_PATH);
            return new SaveData();
        }
    }

    #endregion

    // 디버그용? 데이터 지우기
    public static void ClearData()
    {
        if (File.Exists(SAVE_PATH)) File.Delete(SAVE_PATH);
    }

    #region 게임오브젝트 리스트 <-> 스트링 리스트 변환

    // GameObject List -> String List
    static public List<string> GameObjectListToStringList(List<GameObject> objects)
    {
        // gameObject 리스트 -> 각 오브젝트들의 이름 리스트 작성
        List<string> strs = new List<string>();
        foreach (GameObject go in objects)
        {
            // null 처리
            if (go == null) strs.Add(null);
            else strs.Add(go.name);
        }
        return strs;
    }

    // String List -> GameObject List
    static public List<GameObject> StringListToGameObjectList(List<string> strs)
    {
        // 구하여 반환할 리스트
        List<GameObject> objects = new List<GameObject>();

        // 유닛이름        
        foreach (string str in strs)
        {
            // 이름이 없는 경우, 빈칸
            if (str == null)
            {
                objects.Add(null);
                continue;
            }

            // 이름으로 게임 오브젝트 찾기 시도
            GameObject go = Resources.Load<GameObject>(UNIT_PATH + str);

            // 이름에 해당하는 오브젝트가 없는 경우
            if (go == null)
            {
                string log = null;
                log += "Resources.Load fail!" + " || ";
                log += "UNIT_PATH : " + UNIT_PATH + " || ";
                log += "name : " + str;
                Debug.Log(log);
                return null;
            }

            objects.Add(go);
        }
        return objects;
    }

    #endregion

    public static List<GameObject> GetUsingUnits()
    {
        return StringListToGameObjectList(SaveData.usingUnitNames);
    }

    public static List<GameObject> GetUsingSkills()
    {
        return StringListToGameObjectList(SaveData.usingSkillNames);
    }
}
