using System;
using System.Collections;
using UnityEngine;

public class Interrogation : MonoBehaviour
{
    public static Interrogation Instance { get; private set; }
    private NpcInerrogation currentInterrogatedNpc = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartInterrigation(NpcInerrogation npc)
    {
        if (currentInterrogatedNpc != null)
            StopInterrigation();

        currentInterrogatedNpc = npc;
        //Display the npc's mask with his configuration
        
    }

    public void StopInterrigation()
    {
        //Stop displaying the npc's mask
        currentInterrogatedNpc?.StopInterrogation();
        currentInterrogatedNpc = null;
    }
}