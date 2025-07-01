using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveSystem
{
    private static string path = Application.persistentDataPath + "/save.dat";

    public static void Save(GameData data)
    {
        if (data == null)
        {
            Debug.LogError("Error: Impossible to save null data");
            return;
        }

        List<UpgradeDataSave> upgradeDataSaves = new List<UpgradeDataSave>();
        List<MeleeDataSave> meleeDataSaves = new List<MeleeDataSave>();

        foreach (UpgradeDataSave upgrade in data.upgradesUnlocked)
        {
            upgradeDataSaves.Add(upgrade);
        }

        foreach (MeleeDataSave melee in data.meleeWeaponsUnlocked)
        {
            meleeDataSaves.Add(melee);
        }

        GameData saveData = new GameData(data.crystalQuantity, data.meleeSelected)
        {
            upgradesUnlocked = upgradeDataSaves,
            meleeWeaponsUnlocked = meleeDataSaves,
        };
        saveData.sinnerNB = data.sinnerNB;
        saveData.meleeSelected = data.meleeSelected;


        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.dat";
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, saveData); 
        }
        Debug.Log("Save success");
    }




    public static GameData Load()
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("Save not found. New save created.");
            return new GameData(0, 0); //Return default value
        }
        else Debug.Log("Save found at : " + path);

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                GameData data = (GameData)formatter.Deserialize(stream);

                if (data == null)
                {
                    Debug.LogError("Error : loaded data is null !");
                    return new GameData(0, 0); //Return default value
                }
                Debug.Log("Data loaded");
                return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Can't load file : " + e.Message);
            return new GameData(0, 0); //Return default value
        }
    }

    public static void SetCrystalQuantity(int nb)
    {
        GameData data = Load();
        if(data == null)
        {
            Debug.LogError("Error : Impossible to load saved data");
            return;
        }
        data.crystalQuantity = nb;
        Save(data);
    }

    public static void AddUpgrade(UpgradeData upgrade)
    {
        GameData data = Load();
        data.upgradesUnlocked.Add(new UpgradeDataSave(upgrade));
        Save(data);
    }

    public static void AddMelee(MeleeData melee)
    {
        GameData data = Load();
        data.meleeWeaponsUnlocked.Add(new MeleeDataSave(melee));
        Save(data);
    }

    public static void SelectMelee(int id)
    {
        GameData data = Load();
        data.meleeSelected = id;
        Save(data);
    }
}


