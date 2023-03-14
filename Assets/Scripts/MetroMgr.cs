using System;
using System.Collections;
using UnityEngine;

public class MetroMgr : MonoBehaviour
{
    GameObject metro;
    GameObject GameManager;
    Generator generator;
    public Boolean isMoving = false;
    enum Direction { LEFT, RIGHT };

    Vector3 lastPosition;
    Vector3 onGoingPosition;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
        generator = GameManager.GetComponent<Generator>();
        metro = gameObject;
        lastPosition = metro.transform.position;
    }

    private string getMetroCurrentStation(Vector3 currentPos)
    {
        if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position)
        {
            return "Top";
        }
        else if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position)
        {
            return "Bottom";
        }
        else if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position)
        {
            return "Left";
        }
        else if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position)
        {
            return "Right";
        }
        else
        {
            return "Unknown";
        }
    }

    private Vector3 getNextMetroStationPos(Direction direction)
    {
        if (direction == Direction.LEFT)
        {
            // LastPosition == Top Station
            if (lastPosition == generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position)
            {
                if (metro.transform.position.x == generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position.x)
                    return generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position;
                else if (metro.transform.position.x > generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position.x)
                    return generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
                else
                    return generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position;
            }
            else if (lastPosition == generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position)
            {
                if (metro.transform.position.x == generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position.x)
                    return generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position;
                else if (metro.transform.position.x < generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position.x)
                    return generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position;
                else
                    return generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position;
            }
            else if (lastPosition == generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position)
            {
                if (metro.transform.position.z == generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position.z)
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position;
                }
                else if (metro.transform.position.z > generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position.z)
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position;
                }
                else
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position;
                }
            }
            else if (lastPosition == generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position)
            {
                if (metro.transform.position.z == generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position.z)
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
                }
                else if (metro.transform.position.z < generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position.z)
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position;
                }
                else
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
                }
            }
        }
        else if (direction == Direction.RIGHT)
        {
            // LastPosition == Top Station
            if (lastPosition == generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position)
            {
                if (metro.transform.position.x == generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position.x)
                    return generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position;
                else if (metro.transform.position.x < generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position.x)
                    return generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
                else
                    return generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position;
            }
            else if (lastPosition == generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position)
            {
                if (metro.transform.position.x == generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position.x)
                    return generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position;
                else if (metro.transform.position.x > generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position.x)
                    return generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position;
                else
                    return generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position;
            }
            else if (lastPosition == generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position)
            {
                if (metro.transform.position.z == generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position.z)
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
                }
                else if (metro.transform.position.z < generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position.z)
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position;
                }
                else
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
                }
            }
            else if (lastPosition == generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position)
            {
                if (metro.transform.position.z == generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position.z)
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position;
                }
                else if (metro.transform.position.z > generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position.z)
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position;
                }
                else
                {
                    return generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position;
                }
            }
        }

        return generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
    }

    IEnumerator MoveToPosition()
    {
        while (metro.transform.position != onGoingPosition && isMoving)
        {
            metro.transform.position = Vector3.MoveTowards(metro.transform.position, onGoingPosition, Time.deltaTime * 50f);
            yield return null;
        }
        lastPosition = onGoingPosition;
        isMoving = false;
    }

    private void Move(Direction direction)
    {
        if (isMoving)
        {
            return;
        }

        onGoingPosition = getNextMetroStationPos(direction);
        isMoving = true;

        StartCoroutine(MoveToPosition());
    }

    private void Stop()
    {
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Move(Direction.LEFT);
            Debug.Log("LEFT -> " + getMetroCurrentStation(metro.transform.position));
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Move(Direction.RIGHT);
            Debug.Log("AFTER RIGHT -> " + getMetroCurrentStation(metro.transform.position));
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Stop");
            Stop();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            metro.transform.position = generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            metro.transform.position = generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            metro.transform.position = generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            metro.transform.position = generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position;
        }
    }
}
