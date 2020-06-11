using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR//在编译器环境下运行
using UnityEditor;
#endif

using UnityEngine;

public class cccc : MonoBehaviour
{
#if UNITY_EDITOR//在编译器环境下运行
    void OnValidate()

    {

        Debug.Log("there is a change");

    }

    void Reset()

    {

         if(this.gameObject.GetComponents<cccc>().Length > 1)
        {
            EditorUtility.DisplayDialog("error", "There is already a cccc script", "known");
            DestroyImmediate(this);
        }

    }
#endif
}
