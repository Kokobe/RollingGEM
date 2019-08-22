using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO; 
public class Win_Trigger : MonoBehaviour
{
    private SpriteRenderer sprite_renderer;
    public Sprite original_sprite;
    public Sprite new_sprite;
    public SocketIOComponent socket;
    public bool finished = false;
    public GameController game_controller;
    public CameraController cam_controller;
    public PlayerController player_controller;
    
    void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!finished)
        {
            Debug.Log("winner: " + game_controller.userId);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["winner"] = game_controller.userId; 
            socket.Emit("FINISHED", new JSONObject(data));
            

            sprite_renderer.sprite = new_sprite;
            cam_controller.move(13.5f);
            finished = true;
            player_controller.DisablePlayer();
        }
    }

    public void ResetTrigger()
    {
        sprite_renderer.sprite = original_sprite;
        finished = false;
    }
}
