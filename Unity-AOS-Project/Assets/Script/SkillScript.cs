using UnityEngine;
using System.Collections;

public class SkillScript : MonoBehaviour {

	public IEnumerator Flash(GameObject obj)
    {
        Character ch = obj.GetComponent<Character>();
        Ray ray = ch.characterCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                Vector3 temp = new Vector3(hit.point.x, 49.4837f, hit.point.z);
                obj.transform.LookAt(temp);
                obj.transform.position = temp;
                if (ch.CameraFollowMe)
                {
                    ch.characterCamera.transform.position = obj.transform.position + new Vector3(0, ch.CameraHeight, ch.CameraDistance);
                    ch.characterCamera.transform.LookAt(obj.transform.position);
                }
            }
            ch.characterState = CharState.Idle;
            ch.WayPoint.SetActive(false);
        }
        yield return null;
    }

}
