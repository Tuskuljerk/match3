using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    
    public GameObject cubePrefab;
    public List<Material> cubesColorList = new List<Material>();
    public int xSize, ySize;
    private bool _isShifting;
    public bool IsShifting
    {
        get
        {
            return _isShifting;
        }
        set
        {
            _isShifting = value;
        }
    }
    
    private GameObject[,] _cubes;
    
    
    private void Awake()
    {
        instance = GetComponent<BoardManager>();
        SetBoard();
    }

    void SetBoard()
    {
        float xOffset =  1.7f;
        float yOffset = 1.7f;
        _cubes = new GameObject[xSize, ySize];
        float startX = transform.position.x;
        float startY = transform.position.y;


        Material[] _previousLeft = new Material[ySize];
        Material _previousBelow = null;

        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                GameObject newCube = Instantiate(cubePrefab, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), cubePrefab.transform.rotation);
                _cubes[x, y] = newCube;
                newCube.transform.parent = transform;

                List<Material> _allowedCubeColor = new List<Material>();
                _allowedCubeColor.AddRange(cubesColorList);

                _allowedCubeColor.Remove(_previousLeft[y]);
                _allowedCubeColor.Remove(_previousBelow);

                Material _newMesh = _allowedCubeColor[Random.Range(0, _allowedCubeColor.Count)];
                newCube.GetComponent<Renderer>().sharedMaterial = _newMesh;

                _previousLeft[y] = _newMesh;
                _previousBelow = _newMesh;
            }
        }
    }


    public IEnumerator SearchNull()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (_cubes[x, y].GetComponent<Renderer>().enabled == false)
                {
                    yield return StartCoroutine(MoveDown(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < xSize; x ++)
        {
            for (int y = 0; y < ySize; y++)
            {
                _cubes[x, y].GetComponent<CubeController>().ClearAllMatches();
            }
        }
    }

    private IEnumerator MoveDown(int x, int yStart, float shiftDelay = .08f)
    {
        IsShifting = true;
        List<Renderer> _renders = new List<Renderer>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {
            Renderer render = _cubes[x, y].GetComponent<Renderer>();
            if (render.enabled == false)
            {
                nullCount++;
            }
            _renders.Add(render);
            
        }

        for (int i = 0; i < nullCount; i++)
        {
            UIManager.instance.Score += 25;
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < _renders.Count - 1; k++)
            {
                _renders[k].enabled = true;
                _renders[k + 1].enabled = true;
                _renders[k].sharedMaterial = _renders[k + 1].sharedMaterial;
                _renders[k + 1].sharedMaterial = NewMaterial(x, ySize - 1);
            }
        }
        IsShifting = false;
    }


    private Material NewMaterial(int x, int y)
    {

        List<Material> _possibleCubes = new List<Material>();
        _possibleCubes.AddRange(cubesColorList);

        if(x > 0)
        {
            _possibleCubes.Remove(_cubes[x - 1, y].GetComponent<Renderer>().sharedMaterial);
        }
        if(x < xSize - 1)
        {
            _possibleCubes.Remove(_cubes[x + 1, y].GetComponent<Renderer>().sharedMaterial);
        }
        if (y > 0 )
        {
            _possibleCubes.Remove(_cubes[x, y - 1].GetComponent<Renderer>().sharedMaterial);
        }

        return _possibleCubes[Random.Range(0, _possibleCubes.Count)];

    }
}
