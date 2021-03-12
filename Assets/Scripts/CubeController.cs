using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CubeController : MonoBehaviour
{
    private float _speedRotation = 120f;
 

    private static CubeController previousSelected = null;
    private bool isSelected = false;

    private MeshRenderer curMat;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    
    private bool _matchFound = false;

    
    private void Start()
    {
        curMat = GetComponent<MeshRenderer>();
    }



    private void OnMouseDown()
    {
        if(BoardManager.instance.IsShifting)
        {
            return;
        }

        if(isSelected)
        {
            Deselect();
        } else
        {
            if (previousSelected == null)
                Select();
            else
            {
                if(GetAllAdjacentCubes().Contains(previousSelected.gameObject))
                {
                    MoveCube(previousSelected.curMat);
                    previousSelected.ClearAllMatches();
                    previousSelected.Deselect();
                    ClearAllMatches();
                } else
                {
                    previousSelected.GetComponent<CubeController>().Deselect();
                    Select();
                }
            }
        }
    }


    private void Select()
    {
        isSelected = true;
        previousSelected = gameObject.GetComponent<CubeController>();
        //Debug.Log("Selected");
        StartCoroutine("Rotate");
    }


    private void Deselect()
    {
        isSelected = false;
        previousSelected = null;
        //Debug.Log("Deselected");
        StopCoroutine("Rotate");
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * _speedRotation);
            yield return null;
        }
    }

    public void MoveCube(MeshRenderer curMat2 )
    {
        if ( curMat.sharedMaterial == curMat2.sharedMaterial)
            return;

        Material tempMat = curMat2.sharedMaterial;
        curMat2.sharedMaterial = curMat.sharedMaterial;
        curMat.sharedMaterial = tempMat;
        UIManager.instance.MoveCounter--;
    }

    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, castDir, out hit);
        if (hit.collider != null)
        {
            //Debug.Log("Raycast hit");
            return  hit.collider.gameObject;
        }
        else
        {
            //Debug.Log("Raycast null");
        }
        return null;
    }
    
    private List<GameObject> GetAllAdjacentCubes()
    {
        List<GameObject> adjacentCubes = new List<GameObject>();
        for(int i=0; i < adjacentDirections.Length; i++)
        {
            adjacentCubes.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentCubes;
    }

   
    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matching = new List<GameObject>();
        RaycastHit hit;
        Physics.Raycast(transform.position, castDir, out hit);
       
        while (hit.collider != null &&  hit.collider.GetComponent<Renderer>().sharedMaterial == curMat.sharedMaterial)
        {
            matching.Add(hit.collider.gameObject);
            //Debug.Log("mathcing add");
            Physics.Raycast(hit.collider.transform.position, castDir, out hit);
            //Debug.Log("hit raycast from another cube");
        }  return matching;
    }





    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matching = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matching.AddRange(FindMatch(paths[i]));
        }
        if (matching.Count >= 2)
        {
            for (int i=0; i < matching.Count; i++)
            {
                matching[i].GetComponent<Renderer>().enabled = false;
            }
            _matchFound = true;
        }
    }

    public void ClearAllMatches()
    {
        if (curMat.enabled == false)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (_matchFound)
        {
            curMat.enabled = false;
            _matchFound = false;
            StopCoroutine(BoardManager.instance.SearchNull());
            StartCoroutine(BoardManager.instance.SearchNull());
        }
    }

}
