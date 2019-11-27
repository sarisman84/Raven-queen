using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyScroller : MonoBehaviour
{
    [SerializeField] BountyOnWall[] bountyOnWalls;
    int selectedBounty = 0;

    private void Start()
    {
        bountyOnWalls[0].highlighted = true;
    }

    void ScrollLeft()
    {
        Debug.Log("peepee");
        bountyOnWalls[selectedBounty].highlighted = false;
        if (selectedBounty - 1 < 0)
        {
            selectedBounty = bountyOnWalls.Length - 1;
        }
        else
        {
            selectedBounty--;
        }

        bountyOnWalls[selectedBounty].highlighted = true;

    }


    private void Update()
    {
        
        if (Input.GetAxis("Horizontal") < 0)
        {
            ScrollLeft();
        }
    }

}
