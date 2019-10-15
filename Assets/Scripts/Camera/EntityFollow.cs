using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFollow : MonoBehaviour
{
    // Start is called before the first frame update
    Transform target;

    public GameObject SetCameraFollowTo { set => target = value.transform; }
    Vector3 velocity = Vector3.zero;
    Vector3 offset;
    public float smoothSpeed = 0.125f;



    public static EntityFollow instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        offset = new Vector3(0, transform.position.y, transform.position.z);
    }


    
    // Update is called once per frame
    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPos = target.transform.position + offset;
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed);
        transform.position = smoothedPos;


    }
}
