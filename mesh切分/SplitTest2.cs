using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitTest2 : MonoBehaviour
{
    Mesh ParentMesh;//以后应该改写到GameMode里 MeshA和MeshB作为对象属性 初始化时深拷贝ParentMesh
    Mesh MeshA;
    Mesh MeshB;
    Mesh FallMesh;
    MeshFilter StayMesh;
    MeshCollider StayMeshCollider;
    public float ZLength;
    public float XWidth;
    public float YHeight;
    // Use this for initialization
    private void Awake()
    {
        ParentMesh = new Mesh();
        MeshA = new Mesh();
        MeshB = new Mesh();
        CreateParentMeshCube();
        RenewMesh(ParentMesh, MeshA);
        RenewMesh(ParentMesh, MeshB);
        FallMesh = new Mesh();
    }

    void Start ()
    {
        StayMesh = GetComponent<MeshFilter>();
        StayMeshCollider = GetComponent<MeshCollider>();
        StayMesh.mesh = MeshA;
        StayMeshCollider.sharedMesh = StayMesh.mesh;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Triggered");
        var Slide = other.transform.position/*(other.transform.position - transform.position)*/;
        OnCutX(Slide);
        RenewMesh(CompareMeshParaX(MeshA, MeshB), StayMesh.mesh);
        //往下掉的块以后调整成对象池放在gamemode里
        #region 替代部分
        GameObject fallingpiece = new GameObject();
        fallingpiece.AddComponent<MeshFilter>();
        fallingpiece.AddComponent<MeshRenderer>();
        fallingpiece.AddComponent<MeshCollider>();
        fallingpiece.AddComponent<Rigidbody>();
        fallingpiece.GetComponent<MeshFilter>().mesh = FallMesh;
        fallingpiece.GetComponent<MeshCollider>().sharedMesh = FallMesh;
        fallingpiece.GetComponent<MeshCollider>().convex = true;
        fallingpiece.GetComponent<MeshRenderer>().sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        fallingpiece.transform.SetParent(transform);
        fallingpiece.transform.localPosition = Vector3.zero;
        #endregion
        RenewMesh(StayMesh.mesh, MeshA);
        RenewMesh(StayMesh.mesh, MeshB);
        StayMeshCollider.sharedMesh = StayMesh.mesh;



    }
    void CreateParentMeshCube()
    {
        var PosSilde = new Vector3(XWidth, YHeight, ZLength);
        var NewVertices = new Vector3[]
        {
            //front
            new Vector3(0, 0, 0), new Vector3(0, YHeight, 0), new Vector3(XWidth, YHeight, 0),new Vector3(XWidth, YHeight, 0), new Vector3(XWidth, 0, 0), new Vector3(0, 0, 0),
            //back
            new Vector3(0, 0, ZLength), new Vector3(XWidth, YHeight, ZLength), new Vector3(0, YHeight, ZLength),new Vector3(XWidth, YHeight, ZLength), new Vector3(0, 0, ZLength), new Vector3(XWidth, 0, ZLength),
            //right
            new Vector3(XWidth, 0, 0), new Vector3(XWidth, YHeight, 0), new Vector3(XWidth, 0, ZLength),new Vector3(XWidth, YHeight, ZLength), new Vector3(XWidth, 0, ZLength), new Vector3(XWidth, YHeight, 0),
            //left
            new Vector3(0, 0, 0), new Vector3(0, 0, ZLength), new Vector3(0, YHeight, 0),new Vector3(0, YHeight, ZLength), new Vector3(0, YHeight, 0), new Vector3(0, 0, ZLength),
            //top
            new Vector3(0, YHeight, 0), new Vector3(0, YHeight, ZLength), new Vector3(XWidth, YHeight, 0),new Vector3(XWidth, YHeight, 0), new Vector3(0, YHeight, ZLength), new Vector3(XWidth, YHeight, ZLength),
            //bottom
            new Vector3(0, 0, 0), new Vector3(XWidth, 0, 0), new Vector3(0, 0, ZLength),new Vector3(XWidth, 0, 0), new Vector3(XWidth, 0, ZLength), new Vector3(0, 0, ZLength)
        };
        var TriangleNumber = new int[NewVertices.Length];
        for (int i = 0; i < NewVertices.Length; i++)
        {
            NewVertices[i] -= PosSilde / 2f;
            TriangleNumber[i] = i;
        }
        ParentMesh.vertices = NewVertices;
        ParentMesh.triangles = TriangleNumber;
        var uvtemp = RenewUV(NewVertices);
        ParentMesh.uv = uvtemp;
        
        //uvtemp.CopyTo(ParentMesh.uv,0);
        ParentMesh.RecalculateBounds();
        ParentMesh.RecalculateNormals();
        ParentMesh.RecalculateTangents();
    }
    Vector2[] RenewUV(Vector3[] verticeinput)
    {
        var PosSilde = new Vector3(XWidth, YHeight, ZLength);
        var temp = new Vector3[verticeinput.Length];
        verticeinput.CopyTo(temp, 0);
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] += PosSilde / 2f;
        }
        var uvtemp = new Vector2[temp.Length];
        for (int i = 0; i < uvtemp.Length;)
        {
            if (temp[i].x == temp[i + 1].x && temp[i].x == temp[i + 2].x)
            {
                uvtemp[i] = new Vector2(temp[i].y / YHeight, temp[i].z / ZLength);
                uvtemp[i + 1] = new Vector2(temp[i + 1].y / YHeight, temp[i + 1].z / ZLength);
                uvtemp[i + 2] = new Vector2(temp[i + 2].y / YHeight, temp[i + 2].z / ZLength);

            }
            else if (temp[i].y == temp[i + 1].y && temp[i].y == temp[i + 2].y)
            {
                uvtemp[i] = new Vector2(temp[i].x / XWidth, temp[i].z / ZLength);
                uvtemp[i + 1] = new Vector2(temp[i + 1].x / XWidth, temp[i + 1].z / ZLength);
                uvtemp[i + 2] = new Vector2(temp[i + 2].x / XWidth, temp[i + 2].z / ZLength);
            }
            else if (temp[i].z == temp[i + 1].z && temp[i].z == temp[i + 2].z)
            {
                uvtemp[i] = new Vector2(temp[i].x / XWidth, temp[i].y / YHeight);
                uvtemp[i + 1] = new Vector2(temp[i + 1].x / XWidth, temp[i + 1].y / YHeight);
                uvtemp[i + 2] = new Vector2(temp[i + 2].x / XWidth, temp[i + 2].y / YHeight);
            }
            else
            {
                uvtemp[i] = new Vector2(temp[i].x / XWidth, temp[i].y / YHeight);
                uvtemp[i + 1] = new Vector2(temp[i + 1].x / XWidth, temp[i + 1].y / YHeight);
                uvtemp[i + 2] = new Vector2(temp[i + 2].x / XWidth, temp[i + 2].y / YHeight);
            }
            i += 3;
        }
        return uvtemp;
    }
    void RenewMesh(Mesh meshtorenew)
    {
        meshtorenew.uv = RenewUV(meshtorenew.vertices);
        meshtorenew.RecalculateBounds();
        meshtorenew.RecalculateNormals();
        meshtorenew.RecalculateTangents();
    }
    void RenewMesh(Mesh input,Mesh meshtorenew)
    {
        var temp = new Vector3[input.vertices.Length];
        var tritemp = new int[input.triangles.Length];
        input.vertices.CopyTo(temp,0);
        input.triangles.CopyTo(tritemp, 0);
        meshtorenew.vertices = temp;
        meshtorenew.triangles = tritemp;
        meshtorenew.uv = RenewUV(meshtorenew.vertices);
        meshtorenew.RecalculateBounds();
        meshtorenew.RecalculateNormals();
        meshtorenew.RecalculateTangents();
    }
    void OnCutX(Vector3 slide)
    {
        var Temp = MeshA.vertices;
        for (int i = 0; i < MeshA.vertices.Length; i++)
        {
            if (Temp[i].z > slide.z)
            {
                Temp[i].z = slide.z;
            }
        }
        MeshA.vertices = Temp;
        Temp = MeshB.vertices;
        for (int i = 0; i < MeshB.vertices.Length; i++)
        {
            if (Temp[i].z <= slide.z)
            {
                Temp[i].z = slide.z;
            }
        }
        MeshB.vertices = Temp;
        RenewMesh(MeshA);
        RenewMesh(MeshB);
    }
    Mesh CompareMeshParaX(Mesh A, Mesh B)
    {
        if ((A.vertices[24] - A.vertices[25]).magnitude >= (B.vertices[24] - B.vertices[25]).magnitude)
        {
            RenewMesh(B, FallMesh);
            return A;
        }
        else
        {
            RenewMesh(A, FallMesh);
            return B;
        }
    }
}
