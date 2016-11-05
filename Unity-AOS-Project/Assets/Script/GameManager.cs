using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager
{
    private static GameManager Instance = null;
    public List<SkillItem> SkillList = new List<SkillItem>();
    public List<SkillItem> ItemList = new List<SkillItem>();

    public static GameManager Get()
    {
        if (Instance == null) Instance = new GameManager();
        return Instance;
    }

}
