using UnityEngine;
using System.Collections.Generic;

public class UICameraFollower : MonoBehaviour
{
    public static readonly List<Transform> uiList = new();

    void LateUpdate()
    {
        if(Camera.main == null) { return; }

        Quaternion camRot = Camera.main.transform.rotation;

        for (int i = uiList.Count - 1; i >= 0; i--)
        {
            if (uiList[i] == null)
            {
                uiList.RemoveAt(i); // 破棄済みのTransformを削除
                continue;
            }
            uiList[i].rotation = camRot;
        }
    }
}
