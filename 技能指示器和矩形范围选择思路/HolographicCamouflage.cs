using Magic.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//全息伪装仪的基础逻辑
//之后可能会根据基类等等的情况进行大量的修改
public class HolographicCamouflage : MonoBehaviour {

    //范围指示
    private GameObject hintCircleInstance;
    //选择指示
    private GameObject hintSelectInstance;
    //选择生效范围
    public float areaLength = 200f;
    //选择生效宽度
    public float areaWidth = 100f;
    //操作者
    public Transform user;
    //当前选择的角度
    private Vector3 rotationSelect;
    //选择之后的效果
    public Material effectMaterial;
    //保存原有的材质
    private Material saveMaterial;


    //这个道具开始使用时的操作
    public void OnStartUse()
    {
        GameObject hintCircle = LoadAsset<GameObject>("effect/Prefab/common/quan_hero.x" , null);
        GameObject hintSelect = LoadAsset<GameObject>("effect/Prefab/common/chang_hero.x", null);
        if (hintCircle)
        {
            hintCircleInstance = GameObject.Instantiate(hintCircle);
            hintCircleInstance.transform.localScale = new Vector3(areaLength / 50, 1f, areaLength / 50);
            hintCircleInstance.transform.localPosition = user.transform.localPosition;
        }
        else
        {
            CLog.Log("范围指示预设物无法加载");
        }
        if (hintSelect)
        {
            hintSelectInstance = GameObject.Instantiate(hintSelect);
            hintSelectInstance.transform.localScale = new Vector3(areaWidth / 100 , 1f, areaLength/100);
            hintSelectInstance.transform.localPosition = user.transform.localPosition;
        }
        else
        {
            CLog.Log("选择指示预设物无法加载");
        }
    }

    //在玩家选择物体时的操作
    public void OnUsing()
    {
        //先用鼠标来模拟摇杆和键盘
        Vector3 offset = new Vector3( (Input.mousePosition.x - Screen.width/2)/ (Screen.width / 2), 0f,  (Input.mousePosition.y - Screen.height/2) / (Screen.height / 2));
        rotationSelect = GetRotation(offset) ;
        hintSelectInstance.transform.LookAt(rotationSelect + user.transform.position);
        MakeCheck(areaLength/100 , areaWidth / 100);
    }

    //在完结结束选择，也就是这个道具真正发挥作用的时候
    public void OnEndUse()
    {
        //真正的选择的逻辑
        Destroy(hintCircleInstance);
        Destroy(hintSelectInstance);
    }

    //加载资源的设定方法
    public T LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
            return IResourceManager.Instance.LoadAsset<T>(bundleName, assetName);
        else
            return Resources.Load<T>(IResourceManager.Instance.AssetBundleNameToResourcePath(bundleName, assetName));
#else
            return IResourceManager.Instance.LoadAsset<T>(bundleName, assetName);
#endif
    }

    //修正旋转朝向，传入的参数可能是m_jotystickOffset或者鼠标操作
    private Vector3 GetRotation(Vector3 offset)
    {
        float y = Camera.main.transform.rotation.eulerAngles.y;
        return Quaternion.Euler(0, y, 0) * offset;
    }

    Transform selected = null;
    //检查mapData的数据，看看哪一个被选中了
    private void MakeCheck(float checkDistance , float checkWidth)
    {
        if (selected)
        {
            MeshRenderer theRender  = selected.transform.GetComponent<MeshRenderer>();
            if (theRender)
            {
                theRender.material = saveMaterial;
            }
            selected = null;
        }

        List<Transform> find = new List<Transform>();
        List<Transform> useCheck = ListUse.demoList;//.FindAll( X=> Vector3.Distance(X.transform.position , user.transform.position) <= checkDistance);
        for (int i = 0; i < useCheck.Count; i++)
        {
            //做一个角度的检查
            //目标坐标减去自身坐标
            Vector3 thisToAim = useCheck[i].transform.position - user.transform.position;
            thisToAim = new Vector3(thisToAim.x, 0f, thisToAim.z);
            float dot = Vector3.Dot(rotationSelect.normalized , thisToAim);
            Vector3 checkRight = Quaternion.Euler(0f, 90f, 0f) * rotationSelect.normalized;
            if (dot >= 0 && dot < checkDistance)
            {
                float widthDistance = Vector3.Dot(checkRight , thisToAim);
                if (Mathf.Abs(widthDistance) < checkWidth)
                {
                    find.Add(useCheck[i].transform);
                }
            }
        }

        if (find.Count > 0)
        {
            find.Sort((x, y) => { return Vector3.Distance(x.position, user.position).CompareTo(Vector3.Distance(y.position, user.position)); });
            selected = find[0];
        }
        else
        {
            selected = null;
        }

        if(selected)
        {
            MeshRenderer theRender = selected.transform.GetComponent<MeshRenderer>();
            if (theRender)
            {
                saveMaterial = theRender.material;
                theRender.material = effectMaterial;
                print(selected.transform.name +" is selected");
            }
        }
    }



	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnStartUse();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnEndUse();
        }
        if (Input.GetMouseButton(0))
        {
            OnUsing();
        }
		
	}
}
