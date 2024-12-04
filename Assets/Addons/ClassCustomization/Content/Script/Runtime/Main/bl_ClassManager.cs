using UnityEngine;
using System;
using MFPSEditor;
using MFPS.Addon.ClassCustomization;

public class bl_ClassManager : ScriptableObject
{

    [Header("Player Class")]
    public PlayerClass playerClass = PlayerClass.Assault;
    //When if new player and not have data saved
    //take this weapons ID for default
    [Header("Assault"), ScriptableDrawer]
    public bl_PlayerClassLoadout DefaultAssaultClass;
    [Header("Engineer"), ScriptableDrawer]
    public bl_PlayerClassLoadout DefaultEngineerClass;
    [Header("Support"), ScriptableDrawer]
    public bl_PlayerClassLoadout DefaultSupportClass;
    [Header("Recon"), ScriptableDrawer]
    public bl_PlayerClassLoadout DefaultReconClass;

    public bl_PlayerClassLoadout AssaultClass { get; set; }
    public bl_PlayerClassLoadout EngineerClass { get; set; }
    public bl_PlayerClassLoadout SupportClass { get; set; }
    public bl_PlayerClassLoadout ReconClass { get; set; }
    [HideInInspector] public int ClassKit = 0;
    public const string LOADOUT_KEY_FORMAT = "mfps.loadout.{0}";

    public PlayerClass CurrentPlayerClass
    {
        get => playerClass;
        set
        {
            playerClass = value;
            playerClass.SavePlayerClass();
            PlayerPrefs.SetInt(ClassKey.ClassKit, (int)value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void DeleteKeys()
    {
        PlayerPrefs.DeleteKey(string.Format(LOADOUT_KEY_FORMAT, PlayerClass.Assault));
        PlayerPrefs.DeleteKey(string.Format(LOADOUT_KEY_FORMAT, PlayerClass.Engineer));
        PlayerPrefs.DeleteKey(string.Format(LOADOUT_KEY_FORMAT, PlayerClass.Recon));
        PlayerPrefs.DeleteKey(string.Format(LOADOUT_KEY_FORMAT, PlayerClass.Support));
    }

    /// <summary>
    /// 
    /// </summary>
    public void Init()
    {
        GetID();
    }

    /// <summary>
    /// 
    /// </summary>
    void GetID()
    {
        ClassKit = PlayerPrefs.GetInt(ClassKey.ClassKit, 0);
        playerClass = PlayerClass.Assault.GetSavePlayerClass();
        string format = LOADOUT_KEY_FORMAT;

#if ULSP
        if (bl_DataBase.Instance != null)
        {
            ClassKit = bl_DataBase.Instance.LocalUser.metaData.rawData.ClassKit;
            string dbData = bl_DataBase.Instance.LocalUser.metaData.rawData.WeaponsLoadouts;

            if (!string.IsNullOrEmpty(dbData))
            {
                AssaultClass = Instantiate(DefaultAssaultClass);
                AssaultClass.FromString(dbData, 0);

                EngineerClass = Instantiate(DefaultEngineerClass);
                EngineerClass.FromString(dbData, 1);

                ReconClass = Instantiate(DefaultReconClass);
                ReconClass.FromString(dbData, 2);

                SupportClass = Instantiate(DefaultSupportClass);
                SupportClass.FromString(dbData, 3);
                return;
            }
            else
            {
                format = $"{bl_DataBase.Instance.LocalUser.NickName}.{format}";
            }         
        }
        else
        {
            Debug.Log("Use local data since player is not logged yet.");
        }
#endif

        ClassKit = PlayerPrefs.GetInt(ClassKey.ClassKit, 0);
        string key = string.Format(format, PlayerClass.Assault);

        string data = PlayerPrefs.GetString(key, DefaultAssaultClass.ToString());
        AssaultClass = Instantiate(DefaultAssaultClass);
        AssaultClass.FromString(data);

        key = string.Format(format, PlayerClass.Engineer);
        data = PlayerPrefs.GetString(key, DefaultEngineerClass.ToString());
        EngineerClass = Instantiate(DefaultEngineerClass);
        EngineerClass.FromString(data);

        key = string.Format(format, PlayerClass.Recon);
        data = PlayerPrefs.GetString(key, DefaultReconClass.ToString());
        ReconClass = Instantiate(DefaultReconClass);
        ReconClass.FromString(data);

        key = string.Format(format, PlayerClass.Support);
        data = PlayerPrefs.GetString(key, DefaultSupportClass.ToString());
        SupportClass = Instantiate(DefaultSupportClass);
        SupportClass.FromString(data);
    }

    public void SetUpClasses(bl_GunManager gm)
    {
        if (AssaultClass == null) { Init(); }

        bl_PlayerClassLoadout pcl = null;
        switch (playerClass)
        {
            case PlayerClass.Assault:
                pcl = AssaultClass;
                break;
            case PlayerClass.Recon:
                pcl = ReconClass;
                break;
            case PlayerClass.Engineer:
                pcl = EngineerClass;
                break;
            case PlayerClass.Support:
                pcl = SupportClass;
                break;
        }

        if (pcl == null)
        {
            Debug.LogError($"Player Class Loadout has not been assigned for the class {playerClass.ToString()}");
            return;
        }

        gm.PlayerEquip[0] = gm.GetGunOnListById(pcl.Primary);
        gm.PlayerEquip[1] = gm.GetGunOnListById(pcl.Secondary);
        gm.PlayerEquip[2] = gm.GetGunOnListById(pcl.Letal);
        gm.PlayerEquip[3] = gm.GetGunOnListById(pcl.Perks);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SaveClass(Action callBack = null)
    {
#if ULSP
        if (bl_DataBase.Instance != null)
        {
            string dbdata = $"{AssaultClass.ToString()},{EngineerClass.ToString()},{ReconClass.ToString()},{SupportClass.ToString()}";
            bl_DataBase.Instance.LocalUser.metaData.rawData.WeaponsLoadouts = dbdata;
            bl_DataBase.Instance.LocalUser.metaData.rawData.ClassKit = ClassKit;
            bl_DataBase.Instance.SaveUserMetaData(() => { callBack?.Invoke(); });
        }
        else
            callBack?.Invoke();
#else
        callBack?.Invoke();
#endif
        string key = string.Format(LOADOUT_KEY_FORMAT, PlayerClass.Assault);
        string data = AssaultClass.ToString();
        PlayerPrefs.SetString(key, data);

        key = string.Format(LOADOUT_KEY_FORMAT, PlayerClass.Engineer);
        data = EngineerClass.ToString();
        PlayerPrefs.SetString(key, data);

        key = string.Format(LOADOUT_KEY_FORMAT, PlayerClass.Recon);
        data = ReconClass.ToString();
        PlayerPrefs.SetString(key, data);

        key = string.Format(LOADOUT_KEY_FORMAT, PlayerClass.Support);
        data = SupportClass.ToString();
        PlayerPrefs.SetString(key, data);

        PlayerPrefs.SetInt(ClassKey.ClassKit, ClassKit);
        playerClass.SavePlayerClass();
    }

    public bool isEquiped(int gunID, PlayerClass playerClass)
    {
        switch (playerClass)
        {
            case PlayerClass.Assault:
                return (AssaultClass.Primary == gunID || AssaultClass.Secondary == gunID || AssaultClass.Perks == gunID || AssaultClass.Letal == gunID);
            case PlayerClass.Recon:
                return (ReconClass.Primary == gunID || ReconClass.Secondary == gunID || ReconClass.Perks == gunID || ReconClass.Letal == gunID);
            case PlayerClass.Engineer:
                return (EngineerClass.Primary == gunID || EngineerClass.Secondary == gunID || EngineerClass.Perks == gunID || EngineerClass.Letal == gunID);
            case PlayerClass.Support:
                return (SupportClass.Primary == gunID || SupportClass.Secondary == gunID || SupportClass.Perks == gunID || SupportClass.Letal == gunID);
        }
        return false;
    }

    private static bl_ClassManager _instance;
    public static bl_ClassManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<bl_ClassManager>("ClassManager") as bl_ClassManager;
            }
            return _instance;
        }
    }
}