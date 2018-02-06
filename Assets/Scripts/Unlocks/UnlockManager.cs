using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum UnlockableType
{
    HOUSE,
    SKYSCRAPER,
    SHOP,
    WEAPON,
    VEHICLE,
    ZOMBIE,
};

public class UnlockManager : MonoBehaviour
{
	public static UnlockManager instance;
    public Unlockable[] allUnlockables;
    public Unlockable[] startUnlocked;
    bool initialised = false;
    Dictionary<UnlockableType, List<GameObject>> unlockables = new Dictionary<UnlockableType, List<GameObject>>();

	void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
		instance = this;

        DontDestroyOnLoad(gameObject);
        Init();
	}

    void Init()
    {
        if (initialised) return;
        initialised = true;
        LoadUnlocks();
        foreach (Unlockable u in allUnlockables)
        {
            if (!unlockables.ContainsKey(u.type)) unlockables.Add(u.type, new List<GameObject>());
            unlockables[u.type].Add(u.gameObject);
        }
    }

    public void ResetUnlocks()
    {
        for(int i = 0; i < allUnlockables.Length; i++)
        {
            allUnlockables[i].unlocked = false;
        }

        for(int i = 0; i < startUnlocked.Length; i++)
        {
            startUnlocked[i].unlocked = true;
        }

        SaveUnlocks();
    }

    const int PP_TRUE = 1;
    const int PP_FALSE = 0;
    const string PP_UNLOCKS_SAVED = "Unlocks Saved";

    void LoadUnlocks()
    {
        if (PlayerPrefs.GetInt(PP_UNLOCKS_SAVED, PP_FALSE) == PP_FALSE)
        {
            return;
        }
        foreach (Unlockable u in allUnlockables)
        {
            u.unlocked = (PlayerPrefs.GetInt(u.unlockableID) == PP_TRUE);
        }
    }

    void SaveUnlocks()
    {
        foreach (Unlockable u in allUnlockables)
        {
            PlayerPrefs.SetInt(u.unlockableID, u.unlocked ? PP_TRUE : PP_FALSE);
        }
        PlayerPrefs.SetInt(PP_UNLOCKS_SAVED, PP_TRUE);
        PlayerPrefs.Save();
    }

    public Unlockable UnlockRandom()
    {
        IEnumerable<Unlockable> locked = allUnlockables.Where(u => !u.unlocked);
        if (locked.Count() == 0)
        {
            return null;
        }
        Unlockable unlockable = locked.ElementAt(Random.Range(0, locked.Count()));
        unlockable.unlocked = true;
        SaveUnlocks();
        return unlockable;
    }

    public void UnlockItem(Unlockable item)
    {
        item.unlocked = true;
        PlayerPrefs.SetInt(item.unlockableID, item.unlocked ? PP_TRUE : PP_FALSE);
        PlayerPrefs.SetInt(PP_UNLOCKS_SAVED, PP_TRUE);
        PlayerPrefs.Save();
    }
	
	void Update()
    {
		
	}

    public GameObject[] GetUnlockedItems(UnlockableType type)
    {
        return unlockables[type].FindAll(u => u.GetComponent<Unlockable>().unlocked).ToArray();
    }

    public int GetLockedItemCount()
    {
        int lockedCount = 0;
        foreach (Unlockable item in allUnlockables)
        {
            if (!item.unlocked)
                lockedCount++;
        }
        return lockedCount;
    }

    public Unlockable[] GetLockedItems()
    {
        List<Unlockable> lockedItemsList = new List<Unlockable>();
        foreach(Unlockable item in allUnlockables)
        {
            if (!item.unlocked)
                lockedItemsList.Add(item);
        }

        return lockedItemsList.ToArray();
    }
}
