using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharState
{
    Idle = 0,
    Move,
    GoToAttack
}

public struct Score
{
    public int Kill;
    public int Death;

    public override string ToString()
    {
        string ret = string.Empty;

        ret += "K : " + Kill.ToString() + " / D : " + Death.ToString();

        return ret;
    }
}

public enum DeathNote
{
    LastAttack = 0,
    LastMagic,

    Fallen,
    Count
}

public enum TeamKind
{
    Both = 0,
    TeamA,
    TeamB
}

public class Character : MonoBehaviour
{
    public CharState characterState = CharState.Idle;
    public float MoveSpeed;
    public float AttackSpeed;
    public Camera characterCamera;
    public bool CameraFollowMe = true;
    public float CameraDistance, CameraHeight;
    public GameObject WayPoint, Icons;
    public Score characterScore;
    public TeamKind characterTeam;
    public int mHP = 10, nHP, DMG = 1;

    private Vector3 moveDestination, RespawnPos;
    private GameObject AttackTarget;


    /*
    public string Name;
    public string Explane;
    public string EventName;
    public string objPath, emptyPath;
    public float CoolTime;
    */

    void Start()
    {
        characterScore = new Score { Kill = 0, Death = 0 };
        WayPoint = Instantiate(Resources.Load<GameObject>("Prefabs/WayPoint"));
        RespawnPos = transform.position;
        StartCoroutine(Respawn());

        for (int i = 0; i<Icons.transform.childCount-1; i++)
        {
            for (int j = 0; j<Icons.transform.GetChild(i).childCount; j++)
            {
                Icons.transform.GetChild(i).GetChild(j).GetComponent<IconManager>().SetData(new SkillItem { Name = "", Explane = "", EventName = "", objPath = "", emptyPath = "", CoolTime = 1f }, gameObject);
            }
        }
        
        int temp = 0;

        switch (characterTeam)
        {
            case TeamKind.Both:
                temp = 10;
                break;
            case TeamKind.TeamA:
                temp = 8;
                break;
            case TeamKind.TeamB:
                temp = 9;
                break;
        }

        gameObject.layer = temp;
        ViewMinimap(characterTeam);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //    characterCamera.GetComponent<SkillScript>().StartCoroutine(GetComponent<SkillScript>().Flash(gameObject));

        if (Input.GetKeyDown(KeyCode.Y))
            CameraFollowMe = !CameraFollowMe;

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000f))
            {
                if (hit.transform.CompareTag("Ground"))
                {
                    moveDestination = hit.point;
                    characterState = CharState.Move;
                    WayPoint.transform.position = hit.point;
                    WayPoint.GetComponent<MeshRenderer>().material.color = Color.white;
                    WayPoint.SetActive(true);
                }
                else if (hit.transform.CompareTag("Player"))
                {
                    if (!hit.transform.gameObject.layer.Equals(gameObject.layer))
                    {
                        moveDestination = hit.point;
                        characterState = CharState.GoToAttack;
                        WayPoint.transform.position = hit.point;
                        WayPoint.GetComponent<MeshRenderer>().material.color = Color.red;
                        WayPoint.SetActive(true);
                        AttackTarget = hit.transform.gameObject;
                    }
                    else
                    {
                        moveDestination = hit.point;
                        characterState = CharState.Move;
                        WayPoint.transform.position = hit.point;
                        WayPoint.GetComponent<MeshRenderer>().material.color = Color.white;
                        WayPoint.SetActive(true);
                    }
                }
            }
        }

        if (CameraFollowMe)
        {
            characterCamera.transform.position = transform.position + new Vector3(0, CameraHeight, CameraDistance);
            characterCamera.transform.LookAt(transform.position);
        }

        switch (characterState)
        {
            case CharState.Idle:
                IdleUpdate();
                break;
            case CharState.Move:
                MoveUpdate();
                break;
            case CharState.GoToAttack:
                GoToAttack();
                break;
        }
    }

    void GoToAttack()
    {
        Vector3 temp = new Vector3(moveDestination.x, 49.4837f, moveDestination.z);
        Vector3 framePos = Vector3.MoveTowards(transform.position, temp, Time.deltaTime * MoveSpeed);
        transform.position = framePos;

        transform.LookAt(temp);
        if (CameraFollowMe)
        {
            characterCamera.transform.position = transform.position + new Vector3(0, CameraHeight, CameraDistance);
            characterCamera.transform.LookAt(transform.position);
        }
        float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(WayPoint.transform.position.x, WayPoint.transform.position.z));
        if (distance < 0.5f)
        {
            characterState = CharState.Idle;
            WayPoint.SetActive(false);
            AttackTarget.transform.GetComponent<Character>().nHP -= DMG;
            if (AttackTarget.transform.GetComponent<Character>().nHP <= 0)
            {
                StartCoroutine(AttackTarget.transform.GetComponent<Character>().Death(DeathNote.LastAttack, gameObject));
                characterScore.Kill++;
                Debug.Log(name + " Score : " + characterScore.ToString());
            }
            AttackTarget = null;
        }
    }

    void IdleUpdate()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * MoveSpeed * 10);
        var newRotation = transform.rotation.eulerAngles;
        newRotation.x = 0;
        newRotation.z = 0;
        transform.rotation = Quaternion.Euler(newRotation);
    }

    void MoveUpdate()
    {
        Vector3 temp = new Vector3(moveDestination.x, 49.4837f, moveDestination.z);
        Vector3 framePos = Vector3.MoveTowards(transform.position, temp, Time.deltaTime * MoveSpeed);
        transform.position = framePos;

        transform.LookAt(temp);
        if (CameraFollowMe)
        {
            characterCamera.transform.position = transform.position + new Vector3(0, CameraHeight, CameraDistance);
            characterCamera.transform.LookAt(transform.position);
        }
        float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(WayPoint.transform.position.x, WayPoint.transform.position.z));
        if (distance < 0.5f)
        {
            characterState = CharState.Idle;
            WayPoint.SetActive(false);
        }
    }

    public IEnumerator Death(DeathNote note, GameObject obj = null)
    {
        switch (note)
        {
            case DeathNote.LastAttack:
                StartCoroutine(PrintMessage(new AOS_Message { Text = obj.name + " 님이 " + name + " 님을 일반 공격으로 처치하셨습니다.", Type = MessageType.KillDeath}));
                characterScore.Death++;
                yield return Respawn();
                break;
            case DeathNote.LastMagic:
                StartCoroutine(PrintMessage(new AOS_Message { Text = obj.name + " 님이 " + name + " 님을 주문으로 처치하셨습니다.", Type = MessageType.KillDeath }));
                characterScore.Death++;
                break;
            case DeathNote.Fallen:
                if (obj != null) Debug.Assert(true, "추락사의 경우 처치한 플레이어가 존재할 수 없으며, 해당 매개 인자를 생략해도 됩니다.");
                StartCoroutine(PrintMessage(new AOS_Message { Text = name + " 님이 필드를 벗어나 사망하셨습니다.", Type = MessageType.KillDeath }));
                characterScore.Death++;
                yield return Respawn();
                break;
            case DeathNote.Count:
                Debug.LogError("사망 원인으로 옳지 않은 매개 변수가 전달되었습니다.");
                Debug.LogError("사망 원인으로 DeathNote.Count를 사용 할 수 없습니다.");
                break;
        }
        yield return null;
    }

    public IEnumerator PrintMessage(AOS_Message Message)
    {
        Debug.Log(Message.ToString());
        yield return null;
    }

    public IEnumerator Respawn()
    {
        Debug.Log(name + " Score : " + characterScore.ToString());
        characterState = CharState.Idle;
        WayPoint.SetActive(false);
        GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        transform.position = RespawnPos;
        moveDestination = transform.position;
        characterCamera.transform.position = transform.position + new Vector3(0, CameraHeight, CameraDistance);
        characterCamera.transform.LookAt(transform.position);
        transform.rotation = Quaternion.identity;
        nHP = mHP;
        yield return null;
    }

    public void ViewMinimap(TeamKind kind)
    {
        List<string> layers = new List<string>();
        switch (kind)
        {
            case TeamKind.Both:
                layers.Add("Default");
                layers.Add("TransparentFX");
                layers.Add("Ignore Raycast");
                layers.Add("Water");
                layers.Add("TeamA");
                layers.Add("TeamB");
                layers.Add("Both");
                break;
            case TeamKind.TeamA:
                layers.Add("Default");
                layers.Add("TransparentFX");
                layers.Add("Ignore Raycast");
                layers.Add("Water");
                layers.Add("TeamA");
                layers.Add("Both");
                break;
            case TeamKind.TeamB:
                layers.Add("Default");
                layers.Add("TransparentFX");
                layers.Add("Ignore Raycast");
                layers.Add("Water");
                layers.Add("TeamB");
                layers.Add("Both");
                break;
            default:
                layers.Add("Default");
                layers.Add("TransparentFX");
                layers.Add("Ignore Raycast");
                layers.Add("Water");
                layers.Add("Both");
                break;
        }
        GameObject.Find("Minimap").GetComponent<Camera>().cullingMask = LayerMask.GetMask(layers.ToArray());
    }
}

public enum MessageType
{
    None = 0,

    KillDeath,

    Count
}

public struct AOS_Message
{
    public object Text;
    public MessageType Type;

    public override string ToString()
    {
        string ret = string.Empty;

        ret += Type.ToString();
        ret += " : " + Text;

        return ret;
    }
}