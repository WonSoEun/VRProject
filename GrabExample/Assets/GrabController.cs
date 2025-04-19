using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GrabController : MonoBehaviour
{
    public enum GrabState
    {
        ready,
        inhand
    }

    public GrabState grabState;

    [Header("VR Input")]
    public SteamVR_Action_Boolean rightGrab;
    public SteamVR_Action_Boolean b_Button;

    private SteamVR_Input_Sources rHand;
    public SteamVR_Behaviour_Pose m_Pose;  

    private RaycastHit hit;
    private RaycastHit hit2;

    [Header("Object")]
    public GameObject grabMark;
    public GameObject guideLaser;
    public LayerMask layerMask;         //오브젝트의 레이어

    public GameObject socket;           //잡힐 물건의 기본위치

    private GameObject targetObject;    //잡혀질 물건이 될 오브젝트
    private Rigidbody targetRigidbody;  

    public GameObject handModel;

    private MeshRenderer guideLaserMat;


    [Header("Variable")]
    public float maxDistance;
    float adjustment_Distance;

    

    private void Start()
    {
        guideLaser.SetActive(false);
        grabMark.SetActive(false);

        grabState = GrabState.ready;

        rHand = SteamVR_Input_Sources.RightHand;
        guideLaserMat = guideLaser.transform.GetChild(0).GetComponent<MeshRenderer>();
        
    }

    private void Update()
    {
        if (b_Button.GetState(rHand) && grabState == GrabState.ready)
        {
            guideLaser.SetActive(true);
            ObjectGrabTarget();
        }

        if (b_Button.GetStateUp(rHand))
        {
            guideLaser.SetActive(false);
            grabMark.SetActive(false);
        }


        if (rightGrab.GetStateUp(rHand) && grabState == GrabState.inhand)
        {
            if (targetRigidbody != null)
            {
                targetRigidbody.isKinematic = false;
            }

            targetRigidbody.velocity = m_Pose.GetVelocity();
            targetRigidbody.angularVelocity = m_Pose.GetAngularVelocity();
            targetObject.transform.parent = null;

            handModel.SetActive(true);
            targetObject = null;
            targetRigidbody = null;
            grabState = GrabState.ready;
        }

    }


    private void ObjectGrabTarget()
    {
        Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.blue, 0); //0 - 초단위로 보이게 함. 
        Debug.DrawRay(transform.position, transform.forward * hit2.distance, Color.red, 0);


        //광선에 닿았을 때
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, layerMask))
        {
            grabMark.transform.position = hit.transform.position;
            guideLaserMat.material.color = Color.blue;
            grabMark.SetActive(true);
            
            if(rightGrab.GetStateDown(rHand))
            {
                CatchObj();
            }
        }
        else
        {
            guideLaserMat.material.color = Color.red;
            grabMark.SetActive(false);
        }

        //광선에 닿았을 때 BaceLaser길이 조정
        if (Physics.Raycast(transform.position, transform.forward, out hit2))
        {
            adjustment_Distance = hit2.distance;
            guideLaser.transform.localScale = new Vector3(0.005f, 0.005f, adjustment_Distance);
        }

    }

    public void CatchObj()
    {
        grabMark.SetActive(false);
        targetObject = hit.transform.gameObject;
        targetObject.transform.SetParent(transform.parent); // GrabManager의 자식객체 - targetObject

        targetObject.transform.position = socket.transform.position;
        targetObject.transform.rotation = socket.transform.rotation;

        if (targetObject.GetComponent<Rigidbody>() != null)
        {
            targetRigidbody = targetObject.GetComponent<Rigidbody>();
            targetRigidbody.isKinematic = true;
        }

        handModel.SetActive(false);
        guideLaser.SetActive(false);
        grabState = GrabState.inhand;
    }

}