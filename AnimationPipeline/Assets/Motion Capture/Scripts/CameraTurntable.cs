using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurntable : MonoBehaviour
{
   [SerializeField]
    public float yrot = 0;
    public float xrot = 0;
    void Update()
    {
        transform.localEulerAngles = new Vector3(xrot, yrot, 0);
    }
}
