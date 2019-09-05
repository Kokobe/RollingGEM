using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject horizontal_block;
    public Transform starting_point;
    public float vertical_spacing = 0.5f;
    public int blocks = 12;
    private Vector3 horizontal_block_size;

    // Start is called before the first frame update
    void Start()
    {
        horizontal_block_size = horizontal_block.GetComponent<Renderer>().bounds.size;
    }

    void ClearMaze()
    {
        for (int i = 1; i < transform.childCount; i++) //clear all but the "Starting Point"
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void GenerateMaze(ArrayList arr)
    {
        ClearMaze();
        int i = 0;
        foreach (var item in arr)
        {
            if (item.GetType() == typeof(List<float>))
            {
                var val = (List<float>) item;
                SpawnWithGap(val[0], val[1], i);

            } else
            {
                SpawnHorizontal((float) item, i);
            }
            i++;
        }
    }

    void SpawnHorizontal(float x, int i)
    {
        Instantiate(horizontal_block, new Vector3(x, starting_point.position.y + i *
                (vertical_spacing + horizontal_block_size.y), 0), Quaternion.identity, transform);
    }

    void SpawnWithGap(float gap_space, float x1, int i)
    {
        SpawnHorizontal(x1, i);
        float x2 = x1 + horizontal_block_size.x + gap_space;
        SpawnHorizontal(x2, i);
    }
}
