using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private float starSpeed = 10;
    private Vector3 startPosition;
    private float repeatLength;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        repeatLength = GetComponent<BoxCollider>().size.y * 5;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * starSpeed * Time.deltaTime);
        if (transform.position.z < startPosition.z - repeatLength)
        {
            transform.position = startPosition;
        }
    }
}
