using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    public GameObject horizontal_block;
    public Transform starting_point;
    public float vertical_spacing = .2f;
    public int blocks = 15;
    private Vector3 horizontal_block_size;

    // Start is called before the first frame update
    void Start()
    {
        horizontal_block_size = horizontal_block.GetComponent<Renderer>().bounds.size;
        for(int i = 0; i < blocks; i++)
        {
            int randInt = Random.Range(0, 3);
            switch (randInt)
            {
                case 0:
                    SpawnInHorizontalRange(-.3f, -3f, i);
                    break;
                case 1:
                    SpawnInHorizontalRange(.3f, 3f, i);
                    break;
                case 2:
                    SpawnWithGap(.5f, 2f, i);
                    break;
                default:
                    break;
            }
            
        }
    }

    float SpawnInHorizontalRange(float min, float max, int i)
    {
        float x = Random.Range(min, max);
        Instantiate(horizontal_block, new Vector3(x, starting_point.position.y + i *
                (vertical_spacing + horizontal_block_size.y), 0), Quaternion.identity, transform);
        return x;
    }

    void SpawnWithGap(float min_gap_space, float max_gap_space, int i)
    {
        float gap_space = Random.Range(min_gap_space, max_gap_space);
        float x1 = SpawnInHorizontalRange(-.3f, -3f, i);
        float x2 = x1 + horizontal_block_size.x + gap_space;
        SpawnInHorizontalRange(x2, x2, i);
    }
}
