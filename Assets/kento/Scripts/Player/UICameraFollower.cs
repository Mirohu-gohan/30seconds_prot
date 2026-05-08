using UnityEngine;
using System.Collections.Generic;

public class UICameraFollower : MonoBehaviour
{
    public static readonly List<Transform> uiList = new();

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void LateUpdate()
    {
        if (_cam == null)
        {
            _cam = Camera.main;
            if (_cam == null) return;
        }

        Quaternion camRot = _cam.transform.rotation;

        for (int i = uiList.Count - 1; i >= 0; i--)
        {
            if (uiList[i] == null) { uiList.RemoveAt(i); continue; }
            uiList[i].rotation = camRot;
        }
    }
}
