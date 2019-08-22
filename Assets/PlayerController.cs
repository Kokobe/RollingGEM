using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public Transform startPosition;

    public void PutPlayerAtStart()
    {
        player.transform.position = startPosition.position;
    }

    public void EnablePlayer()
    {
        player.SetActive(true);
    }

    public void DisablePlayer()
    { 
        player.SetActive(false);
    }
}
