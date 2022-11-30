using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public Camera cam;
    public List<PixelCube> pixelCubes = new List<PixelCube>();
    public int separatedPartCount = 0;
    public GameObject combineGo;

    static public CubeController instance;

    public Action<PixelCube> OnCutCube;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        List<PixelCube> partCubes = new List<PixelCube>();
        foreach (var cube in pixelCubes)
        {
            cube.cubeCtrl = this;
            cube.isLinked = true;
            cube.isInit = true;

            // Calculate other part cubes. Without its cube.
            partCubes.Clear();
            partCubes.AddRange(pixelCubes);
            partCubes.Remove(cube);
            cube.otherPartCubes.AddRange(partCubes);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool isHit = Physics.Raycast(ray, out hit, 100f);
            if (isHit)
            {
                Debug.Log($"Hit {hit.collider.gameObject}", hit.collider.gameObject);
                var pixelCube = hit.collider.GetComponent<PixelCube>();
                if (pixelCube != null)
                {
                    pixelCube.Hit();
                }
            }
        }
    }
}
