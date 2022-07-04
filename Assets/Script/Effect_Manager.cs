using System.Collections;
using UnityEngine;

public class Effect_Manager : MonoBehaviour
{
    readonly float destroy_time = 0.5f;

    public void Display_Effect(GameObject Effect_Obj,GameObject Parent_Obj)
    {
        GameObject Effect_Obj_Prefab = Instantiate(Effect_Obj, Effect_Obj.transform.localPosition, Quaternion.identity, Parent_Obj.transform);
        Effect_Obj_Prefab.transform.localPosition = Effect_Obj.transform.localPosition;
        StartCoroutine( Destroy_Effect(Effect_Obj_Prefab,destroy_time) );
    }

    IEnumerator Destroy_Effect(GameObject Effect_Obj_Prefab, float time)
    {
        Destroy(Effect_Obj_Prefab, time);
        yield return null;
    }
}
