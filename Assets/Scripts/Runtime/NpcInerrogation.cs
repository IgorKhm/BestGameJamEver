using System;
using Unity.VisualScripting;
using UnityEngine;

public class NpcInerrogation : MonoBehaviour
{
    public NpcController NpcController;
    private float initialNpcMoveSpeed;
    
    private void Start()
    {
        NpcController = GetComponent<NpcController>();
        initialNpcMoveSpeed = NpcController.moveSpeed;
    }

    private void OnMouseDown()
    {
        NpcController.moveSpeed = 0;
        Interrogation.Instance.StartInterrigation(this);
    }

    private void OnMouseUp()
    {
        Interrogation.Instance.StopInterrigation();
    }

    public void StopInterrogation()
    {
        NpcController.moveSpeed = initialNpcMoveSpeed;
    }
}
