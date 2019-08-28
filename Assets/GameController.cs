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
    public GameObject ready_screen;
    public GameObject finish_screen;
    public Text waitingText;
    public Button startButton;
    public Text finishText;
    public Text gamePinText;
    public Text playersText;

    public Win_Trigger win_trigger;
    public bool gameStarted = false;
    public string userId;
    public string GAME_PIN;
    public JSONObject game_pin_jsonobj;
    void Awake()
    {
        bindControllers();   
        userId = System.Guid.NewGuid().ToString();
        StartCoroutine(ConnectToServer());

        socketIO.On("USER JOINED GAME", OnUserJoinedGame);
        socketIO.On("USER LEFT GAME", OnUserLeftGame);
        socketIO.On("CONNECTION READY", OnConnectionEstablished);
        socketIO.On("CONNECTION NOT READY", OnConnectionNotEstablished);
        socketIO.On("BOTH PLAYERS READY", StartGame);
        socketIO.On("RESULT", OnResultReceived);
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

    public void JoinGame(string game_pin)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["game_pin"] = game_pin;
        game_pin_jsonobj = new JSONObject(data);
        socketIO.Emit("JOIN GAME", game_pin_jsonobj);
        GAME_PIN = game_pin;
        gamePinText.text = "Game Pin = " + GAME_PIN;

        title_screen.SetActive(false);
        ready_screen.SetActive(true);
    }

    void OnUserJoinedGame(SocketIOEvent obj)
    {
        string players = obj.data.GetField("players").ToString();
        playersText.text = "Players = " + players;
    }

    void OnUserLeftGame(SocketIOEvent obj)
    {
        string players = obj.data.GetField("players").ToString();
        playersText.text = "Players = " + players;
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
        socketIO.Emit("READY TO PLAY", game_pin_jsonobj);
    }

    private void StartGame(SocketIOEvent obj)
    {
        cam_controller.move(-13.5f);
        player_controller.PutPlayerAtStart();
        player_controller.EnablePlayer();
        gameStarted = true;
        ready_screen.SetActive(false);
    }

    void OnResultReceived(SocketIOEvent obj)
    {
        string winner_id = obj.data.GetField("winner").ToString();
        winner_id = winner_id.Substring(1, winner_id.Length - 2);
      
        if (winner_id == userId)
            finishText.text = "You won!";
        else
            finishText.text = "You lost :(";

        ShowFinishScreen();
    }


    public void ShowFinishScreen()
    {
        ready_screen.SetActive(false);
        finish_screen.SetActive(true);
    }

    public void GoBackToReadyScreen()
    {
        ready_screen.SetActive(true);
        finish_screen.SetActive(false);

        startButton.gameObject.SetActive(true);
        win_trigger.ResetTrigger();

        socketIO.Emit("RESET READY PLAYERS", game_pin_jsonobj);
    }
}
