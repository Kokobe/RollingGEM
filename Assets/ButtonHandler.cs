using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class ButtonHandler : MonoBehaviour
{
    public GameController game_controller;

    public void ReadyUp()
    {
        game_controller.Ready();
        gameObject.SetActive(false);
    }

    public void ToTitleScreen()
    {
        game_controller.GoBackToTitleScreen();
    }
}
