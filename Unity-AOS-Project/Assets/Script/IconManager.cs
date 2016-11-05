using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IconManager : MonoBehaviour
{
    public KeyCode keyCode;
    public GameObject ObjIcon, ObjEmptyIcon, Player;
    public Icon icon;
    public SkillItem Data;

    private float NowTime = 0.0f;

    public void SetData(SkillItem dat, GameObject obj)
    {
        Data = dat;
        icon = new Icon(dat.objPath, dat.emptyPath);
        Player = obj;
    }

    public void Update()
    {

        if (Input.GetKeyDown(keyCode))
        {
            if (Data.Name != "")
            {
                if (ObjEmptyIcon.GetComponent<Image>().fillAmount == 0.0f)
                {
                    if (Data.EventName != "")
                        Camera.main.GetComponent<SkillScript>().StartCoroutine(Data.EventName, Player);
                    else Debug.LogAssertion(Data.Name + "의 코루틴이 비어있습니다.");
                    ObjEmptyIcon.GetComponent<Image>().fillAmount = 1;
                    StartCoroutine(CoolTime(0.01f));
                    Debug.Log(Data.Name + "이 시전되었습니다.");
                }
                else
                {
                    Debug.Log(Data.Name + "의 쿨타임이 가동 중입니다.");
                }
            }
            else Debug.LogAssertion(keyCode.ToString() + "에 할당된 것이 없습니다.");
        }
    }

    public IEnumerator CoolTime(float Delay)
    {
        NowTime += Delay;
        ObjEmptyIcon.GetComponent<Image>().fillAmount -= Delay / Data.CoolTime;
        yield return new WaitForSeconds(Delay);
        if (ObjEmptyIcon.GetComponent<Image>().fillAmount > 0.0f) StartCoroutine(CoolTime(Delay));
    }

}

public struct Icon
{
    public Sprite objIcon, emptyIcon;

    public Icon(string objPath, string emptyPath, string defaultPath = "Icons/")
    {
        objIcon = Resources.Load<Sprite>(defaultPath + objPath);
        emptyIcon = Resources.Load<Sprite>(defaultPath + emptyPath);
    }
}

public struct SkillItem
{
    public string Name;
    public string Explane;
    public string EventName;
    public string objPath, emptyPath;
    public float CoolTime;
}