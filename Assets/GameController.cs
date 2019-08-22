using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class GameController : MonoBehaviour
{
    CameraController cam_controller;
    PlayerController player_controller;
    SocketIOComponent socketIO;

    public GameObject title_screen;
    public GameObject finish_screen;
    public Text waitingText;
    public Button startButton;
    public Text finishText;

    public Win_Trigger win_trigger;
    public bool gameStarted = false;
    public string userId;

    void Awake()
    {
        bindControllers();   
        userId = System.Guid.NewGuid().ToString();
        StartCoroutine(ConnectToServer());

        socketIO.On("CONNECTION READY", OnConnectionEstablished);
        socketIO.On("CONNECTION NOT READY", OnConnectionNotEstablished);
        socketIO.On("BOTH PLAYERS READY", StartGame);
        socketIO.On("RESULT", OnResultReceived);

    }
    void OnResultReceived(SocketIOEvent obj)
    {
        //test.SetActive(true);
        string winner_id = obj.data.GetField("winner").ToString();
        winner_id = winner_id.Substring(1, winner_id.Length - 2);
        Debug.Log(winner_id);
        Debug.Log(userId);
        Debug.Log(userId == winner_id);

        if (winner_id == userId)
            finishText.text = "You won!";
        else
            finishText.text = "You lost :(";

        ShowFinishScreen();
    }

    void bindControllers()
    {
        cam_controller = GameObject.Find("Camera Controller").GetComponent<CameraController>();
        player_controller = GameObject.Find("Player State Controller").GetComponent<PlayerController>();
        socketIO = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
    }

    private IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(1f);
        socketIO.Emit("USER_CONNECT");
    }

    void OnConnectionEstablished(SocketIOEvent obj)
    {
        if (!gameStarted)
        {
            waitingText.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
        }
    }

    void OnConnectionNotEstablished(SocketIOEvent obj)
    {
        if (!gameStarted)
        {
            waitingText.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }
    }

    public void Ready()
    {
        socketIO.Emit("READY TO PLAY");
    }

    private void StartGame(SocketIOEvent obj)
    {
        cam_controller.move(-13.5f);
        player_controller.PutPlayerAtStart();
        player_controller.EnablePlayer();
        gameStarted = true;
        title_screen.SetActive(false);
    }

    public void ShowFinishScreen()
    {
        title_screen.SetActive(false);
        finish_screen.SetActive(true);
    }

    public void GoBackToTitleScreen()
    {
        title_screen.SetActive(true);
        finish_screen.SetActive(false);

        startButton.gameObject.SetActive(true);
        win_trigger.ResetTrigger();
        socketIO.Emit("RESET READY PLAYERS");
    }
}
