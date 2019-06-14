using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDemo : MonoBehaviour
{
    //无返回类型用action
    Action<string> theAction;
    //有返回类型用func
    //前面是参数类型，后面是返回值类型
    Func<int, string> theFunc;

    private void Start()
    {
        theAction += M1;
        theAction += M2;

        theFunc += M3;
        theFunc += M4;

        //最终被调用的是M1M2M4

    }


    private void M1(string A)
    {
        print(A);
    }
    private void M2(string A)
    {
        print(A+"----"+A);
    }


    private string M3(int a)
    {
        return a + "---";
    }
    private string M4(int a)
    {
        return a + 998 + "yus";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            theAction("suck");
            print(theFunc(9998));
        }
    }
}
