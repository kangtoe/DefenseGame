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

    public StringIntDictionary upgradeInfos;

    public SaveData()
    {
        currentResource = 0;
        usingUnitNames = new List<string>();
        usingSkillNames = new List<string>();
        upgradeInfos = new StringIntDictionary();
    }

    public bool IsUsing(GameObject go)
    {
        if (usingUnitNames != null && usingUnitNames.Exists(x => x == go.name)) return true;
        if (usingSkillNames != null && usingSkillNames.Exists(x => x == go.name)) return true;

        return false;
    }

    public bool IsUnlocked(GameObject go)
    {
        string _name = go.name;
        if (upgradeInfos.ContainsKey(_name) && upgradeInfos[_name] >= 1) return true;            

        return false;
    }
}

public static class SaveManager
{
    public static SaveData CurrentData
    {
        get
        {
            if (currentData == null) currentData = Load();
            return currentData;
        }
    }
    static SaveData currentData;

    // 세이브 데이터 경로와 이름
    static string SAVE_FILE_NAME => "save.dat";
    static string SAVE_PATH => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    // Resources 폴더 아래 유닛 폴더 상대 경로
    static string UNIT_PATH = "Units_f4/";

    // Resources 폴더 아래 유닛 폴더 상대 경로
    static string SKILL_PATH = "Skills/";

    #region 저장

    public static void Save(SaveData _data)
    {
        BinaryFormatter formatter = new BinaryFormatter();        
        FileStream stream = new FileStream(SAVE_PATH, FileMode.Create);        
        formatter.Serialize(stream, _data);
        stream.Close();

        currentData = _data;

        //Debug.Log("saved on : " + SAVE_PATH);
    }

    // currentResource 필드만 수정
    public static void SaveResource(int amount)
    {
        Debug.Log("SaveResource : " + amount);

        // 기존 데이터 수정
        currentData.currentResource = amount;
        
        Save(currentData);
    }

    // usingUnitNames 필드만 수정
    public static void SaveUsingUnits(List<GameObject> usingUnits)
    {
        Debug.Log("SaveUsingUnits");        

        // 기존 데이터 수정
        List<string> names = GameObjectListToStringList(usingUnits);
        currentData.usingUnitNames = names;
        
        Save(currentData);
    }

    // usingSkillNames 필드만 수정
    public static void SaveUsingSkills(List<GameObject> usingSkills)
    {
        Debug.Log("SaveUsingUnits");

        // 기존 데이터 수정
        List<string> names = GameObjectListToStringList(usingSkills);
        currentData.usingSkillNames = names;

        Save(currentData);
    }

    // upgradeInfos 하나의 요소 업데이트/추가
    public static void UpdateLevelInfo(string name, int level)
    {
        Debug.Log("UpdateLevelInfo || " + name  + " level :" + level);

        // 기존 데이터 수정
        {
            Dictionary<string, int> upgradeInfos = CurrentData.upgradeInfos;

            // null인 경우 초기화
            if (upgradeInfos == null) upgradeInfos = new Dictionary<string, int>();

            //string name = go.name;
            if (upgradeInfos.ContainsKey(name))
            {
                // 기존 key가 있는 경우, 해당 오브젝트의 레벨만 변경
                upgradeInfos[name] = level;
            }
            else
            {
                // 기존 key가 없는 경우, 추가하기
                upgradeInfos.Add(name, level);
            }
        }
        
        // 수정사항 저장
        Save(currentData);
    }

    #endregion

    #region 불러오기

    public static SaveData Load()
    {
        Debug.Log("Load");

        if (File.Exists(SAVE_PATH))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SAVE_PATH, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            currentData = data;
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
    static public List<GameObject> StringListToGameObjectList(List<string> strs, string path)
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
            GameObject go = Resources.Load<GameObject>(path + str);

            // 이름에 해당하는 오브젝트가 없는 경우
            if (go == null)
            {
                string log = null;
                log += "Resources.Load fail!" + " || ";
                log += "path : " + path + " || ";
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
        List<string> list = CurrentData.usingUnitNames;
        if (list is null) list = new List<string>();
        return StringListToGameObjectList(list, UNIT_PATH);
    }

    public static List<GameObject> GetUsingSkills()
    {
        List<string> list = CurrentData.usingSkillNames;
        if (list is null) list = new List<string>();
        return StringListToGameObjectList(list, SKILL_PATH);
    }

    public static int GetUpgradeLevel(string name)
    {
        int level;
        Dictionary<string, int> upgradeInfos = CurrentData.upgradeInfos;

        if (upgradeInfos == null)
        {
            // null인 경우
            Debug.Log("upgradeInfos == null");
            level = 0;  
        } 
        else
        {
            if (upgradeInfos.ContainsKey(name))
            {
                // 기존 key가 있는 경우
                level = upgradeInfos[name];
            }
            else
            {
                // 기존 key가 없는 경우
                //Debug.Log(name +" : not found");
                level = 0;
            }
        }                

        //Debug.Log("GetUpgradeLevel || " + name + " level :" + level);
        return level;
    }
}
