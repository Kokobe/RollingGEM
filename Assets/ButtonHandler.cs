using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class ButtonHandler : MonoBehaviour
{
    public GameController game_controller;
    public InputField game_pin_input;
    private static string characters = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMMOPQRSTUVWXYZ";
    private static int pin_length = 6;
    private static System.Random rnd = new System.Random();

    public void ReadyUp()
    {
        game_controller.Ready();
        gameObject.SetActive(false);
    }

    public void ToTitleScreen()
    {
        game_controller.GoBackToReadyScreen();
    }

    public void JoinGame()
    {
        if(game_pin_input.text.Length == 6)
        {
            game_controller.JoinGame(game_pin_input.text);
        }
    }

    public void CreateGame()
    {
        game_controller.JoinGame(GenerateRandomPin());
    }

    public static string GenerateRandomPin()
    {
        string result = "";
        for(int i = 0; i < pin_length; i++)
        {
            int randNum = rnd.Next(0, characters.Length - 1);
            result += characters.Substring(randNum, 1); 
        }
        return result;
    }
}
