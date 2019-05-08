using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListUse : MonoBehaviour {
 

    public static List<Transform> demoList;
    void Start ()
    {
        Transform[] demoListForMake = this.transform.GetComponentsInChildren<Transform>();

        demoList = new List<Transform>();
        for (int i = 0; i < demoListForMake.Length; i++)
        {
            if (demoListForMake[i] != this.transform)
            {
                demoList.Add(demoListForMake[i]);
            }
        }
    }

}
