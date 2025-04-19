using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InventoryController : MonoBehaviour
{
    enum InventoryState
    {
        in_Inven,
        out_Inven
    }

    private InventoryState invenState;

    [Header("VR Input")]
    public SteamVR_Action_Boolean rightGrab;
    private SteamVR_Input_Sources rHand;

    [Header("Object")]
    public GameObject camera_Socket;
    public GameObject gun;
    public GameObject controller_Collider_Socket;
    public GameObject controller_Collider_Obj;
    public GameObject r_Controller_socket;

    [Header("Material")]
    public Material basicInventoryMat;

    private MeshRenderer onInventoryMat;

    private BoxCollider gun_Collider1;
    private BoxCollider gun_Collider2;
    private BoxCollider controller_Collider;

    [Header("Script")]
    public GrabController grabControll;


    bool on_Inven = false;
    bool out_Inven = false;

    private void Start()
    {

        rHand = SteamVR_Input_Sources.RightHand;

        onInventoryMat = gameObject.transform.GetComponent<MeshRenderer>();

        gun_Collider1 = gun.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<BoxCollider>();
        gun_Collider2 = gun.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<BoxCollider>();
        controller_Collider = controller_Collider_Obj.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        gameObject.transform.position = camera_Socket.transform.position;
        gameObject.transform.rotation = camera_Socket.transform.rotation;

        controller_Collider_Obj.transform.position = controller_Collider_Socket.transform.position;
        controller_Collider_Obj.transform.rotation = controller_Collider_Socket.transform.rotation;

        if (on_Inven && invenState == InventoryState.in_Inven)
        {
            gun.transform.position = gameObject.transform.position;
            gun.transform.rotation = gameObject.transform.rotation;

            onInventoryMat.material.color = Color.blue;

            if (gun.GetComponent<Rigidbody>() != null)
            {
                gun.GetComponent<Rigidbody>().useGravity = false;
            }

            gun_Collider1.isTrigger = true;
            gun_Collider2.isTrigger = true;

        }
        else
        {
            onInventoryMat.material = basicInventoryMat;

            gun_Collider1.isTrigger = false;
            gun_Collider2.isTrigger = false;

        }

        if (out_Inven)
        {
            controller_Collider.isTrigger = true;
            gun.transform.position = r_Controller_socket.transform.position;
            gun.transform.rotation = r_Controller_socket.transform.rotation;

            grabControll.grabState = GrabController.GrabState.inhand;

            grabControll.CatchObj();

            if (rightGrab.GetStateUp(rHand))
            {
                gun.GetComponent<Rigidbody>().useGravity = true;
                controller_Collider.isTrigger = false;
                out_Inven = false;
            }
        }


    }
    private void OnTriggerEnter(Collider other)
    {

        if (rightGrab.GetStateUp(rHand) && other.CompareTag("gun") && grabControll.grabState == GrabController.GrabState.ready)
        {
            on_Inven = true;
            invenState = InventoryState.in_Inven;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RHand_Controller") && rightGrab.GetState(rHand) && invenState == InventoryState.in_Inven)
        {
            out_Inven = true;
            invenState = InventoryState.out_Inven;
            on_Inven = false;
        }
    }
}