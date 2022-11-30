using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PixelCube : MonoBehaviour
{
    public List<PixelCube> linkedFromCubes = new List<PixelCube>();
    public List<PixelCube> otherPartCubes = new List<PixelCube>();

    public bool isLinked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ContextMenu("GetLinkedCubes")]
    void GetLinkedCubes()
    {
        linkedFromCubes.Clear();
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
                    linkedFromCubes.Add(pixelCube);
                }
            }
            return isHit;
        }
    }

    private void OnEnable()
    {
        CubeController.instance.OnCutCube += Handle_OnCutCube;
    }

    private void OnDisable()
    {
        CubeController.instance.OnCutCube -= Handle_OnCutCube;
    }

    private void Handle_OnCutCube(PixelCube cutCube)
    {
        if (linkedFromCubes.Contains(cutCube))
        {
            Debug.Log($"{gameObject.name} unlink {cutCube.gameObject.name}");
            linkedFromCubes.Remove(cutCube);

        }

        // Recalculate otherPartCubes.
        otherPartCubes.Clear();
        foreach (var cube in CubeController.instance.pixelCubes)
        {
            foreach (var linkCube in linkedFromCubes)
            {
                bool contain = otherPartCubes.Contains(linkCube);
            }
        }

        //// Check for the cutCube's linked cubes.
        //foreach (var linkedCube in cutCube.linkedFromCubes)
        //{

        //}

        //for (int i = 0; i < cutCube.linkedFromCubes.Count; i++)
        //{

        //}
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
        foreach (var cube in linkedFromCubes)
        {
            Gizmos.DrawLine(transform.position, cube.transform.position);
        }
    }
}
