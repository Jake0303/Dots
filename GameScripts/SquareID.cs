using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SquareID : NetworkBehaviour
{
    [SyncVar]
    public string squareID;
    [SerializeField]
    private Transform myTransform;
    // Use this for initialization
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        SetIdentity();
    }

    void SetIdentity()
    {
        if (myTransform.name == "" || myTransform.name == "CenterSquare(Clone)")
        {
            myTransform.name = squareID;
        }
    }
}
