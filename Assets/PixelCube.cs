using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class PixelCube : MonoBehaviour
{
    public CubeController cubeCtrl;
    public List<PixelCube> nearbyCubes = new List<PixelCube>();
    public List<PixelCube> otherPartCubes = new List<PixelCube>();
    public MeshRenderer meshRender;
    public MeshFilter meshFilter;

    public bool isLinked;
    internal bool isInit;

    private void OnValidate()
    {
        meshRender = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => isInit == true);
    }

    [ContextMenu("GetNearbyLinkedCubes")]
    void GetNearbyLinkedCubes()
    {
        nearbyCubes.Clear();
        GetLink(Vector2.up);
        GetLink(Vector2.down);
        GetLink(Vector2.left);
        GetLink(Vector2.right);

        bool GetLink(Vector2 direction)
        {
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;
            bool isHit = Physics.Raycast(ray, out hit, 1f);
            if (isHit)
            {
                var pixelCube = hit.collider.GetComponent<PixelCube>();
                if (pixelCube != null)
                {
                    nearbyCubes.Add(pixelCube);
                }
            }
            return isHit;
        }
    }

    private void OnEnable()
    {
        cubeCtrl.OnCutCube += Handle_OnCutCube;
    }

    private void OnDisable()
    {
        cubeCtrl.OnCutCube -= Handle_OnCutCube;
    }

    private void Handle_OnCutCube(PixelCube cutCube)
    {
        if (nearbyCubes.Contains(cutCube))
        {
            Debug.Log($"{gameObject.name} unlink {cutCube.gameObject.name}");
            nearbyCubes.Remove(cutCube);

            // Recalculate otherPartCubes.
            otherPartCubes.Clear();
            RecursiveFindCube();
        }
    }



    internal void Hit()
    {
        isLinked = false;
        gameObject.SetActive(false);
        CubeController.instance.OnCutCube?.Invoke(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Handles.color = Color.blue;
        Handles.Label(new Vector3(transform.position.x, transform.position.y, -1.1f), $"{gameObject.name}");
        foreach (var cube in nearbyCubes)
        {
            Gizmos.DrawLine(transform.position, cube.transform.position);
        }
    }

    [ContextMenu("RecursiveFindCube")]
    void RecursiveFindCube()
    {
        bool isStop = false;
        while (!isStop)
        {
            isStop = FindLinkedCubeMesh(otherPartCubes);
        }

        //if (otherPartCubes.Contains(this))
        //    otherPartCubes.Remove(this);

        var color = Random.ColorHSV(); ;
        foreach (var cube in otherPartCubes)
        {
            cube.meshRender.material.color = color;
        }
    }

    [ContextMenu("CombineMesh")]
    void CombineMesh()
    {
        CombineInstance[] combine = new CombineInstance[otherPartCubes.Count];
        int i = 0;
        while (i < otherPartCubes.Count)
        {
            combine[i].mesh = otherPartCubes[i].meshFilter.sharedMesh;
            combine[i].transform = otherPartCubes[i].meshFilter.transform.localToWorldMatrix;
            otherPartCubes[i].meshFilter.gameObject.SetActive(false);
            i++;
        }
        cubeCtrl.combineGo.GetComponent<MeshFilter>().mesh = new Mesh();
        cubeCtrl.combineGo.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        cubeCtrl.combineGo.SetActive(true);
        cubeCtrl.combineGo.AddComponent<BoxCollider>();
        cubeCtrl.combineGo.AddComponent<Rigidbody>();
    }

    bool FindLinkedCubeMesh(List<PixelCube> cubes)
    {
        bool isStop = false;
        for (int i = 0; i < nearbyCubes.Count; i++)
        {
            var cube = nearbyCubes[i];
            if (cubes.Contains(cube)) continue;
            cubes.Add(cube);
            cube.FindLinkedCubeMesh(cubes);
        }

        isStop = true;
        return isStop;
    }
}
