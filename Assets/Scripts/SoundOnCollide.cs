using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollide : MonoBehaviour
{
    public string[] sounds;
    int[] FileIDs;
    int SoundID;
    private void Awake()
    {
        AndroidNativeAudio.makePool();
        FileIDs = new int[sounds.Length];

        for(int i = 0; i < FileIDs.Length; i++)   
        {
            string s = sounds[i];
            FileIDs[i] = AndroidNativeAudio.load(s);
        }
    
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Play native audion
        var i = Random.Range(0, FileIDs.Length);
        AndroidNativeAudio.play(FileIDs[i]);
    }

    void OnApplicationQuit()
    {
        // Clean up when done
        foreach (int FileID in FileIDs)
        {
            AndroidNativeAudio.unload(FileID);
        }
        AndroidNativeAudio.releasePool();
    }

}
