using System;
using System.Collections;
using System.Collections.Generic;
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
    float speed = 60f;
    float rotationSpeed = 20f;

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


    private void MoveUpdate(Vector3 targetPosition)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }

    private void OnMoveComplete(Quaternion targetRotation)
    {
        transform.rotation = targetRotation;
    }

    IEnumerator MoveToPosition(Direction direction)
    {
        Boolean firstDirection = false;
        Boolean xPos = metro.transform.position.x > onGoingPosition.x ? true : false;
        Boolean zPos = metro.transform.position.z > onGoingPosition.z ? true : false;

        if (metro.transform.position.z == generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position.z || metro.transform.position.z == generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position.z)
        {
            firstDirection = true;
        }

        if (firstDirection)
        {
            Vector3 firstPosition = xPos ? new Vector3(onGoingPosition.x + Generator.TILE_SIZE, metro.transform.position.y, metro.transform.position.z) : new Vector3(onGoingPosition.x - Generator.TILE_SIZE, metro.transform.position.y, metro.transform.position.z);

            while (isMoving && ((xPos && metro.transform.position.x != onGoingPosition.x + Generator.TILE_SIZE) || (!xPos && metro.transform.position.x != onGoingPosition.x - Generator.TILE_SIZE)))
            {
                metro.transform.position = Vector3.MoveTowards(metro.transform.position, firstPosition, Time.deltaTime * speed);
                yield return null;
            }


            // Define the target position and rotation
            Vector3 targetPosition = new Vector3(onGoingPosition.x, metro.transform.position.y, metro.transform.position.z - Generator.TILE_SIZE);
            Quaternion targetRotation = Quaternion.Euler(0f, 135f, 0f);

            Hashtable args = new Hashtable();

            args.Add("position", new Vector3(onGoingPosition.x, metro.transform.position.y, metro.transform.position.z - Generator.TILE_SIZE));
            args.Add("time", 1f);
            args.Add("easetype", iTween.EaseType.linear);
            args.Add("onupdate", "MoveUpdate");
            args.Add("onupdateparams", targetPosition);
            args.Add("oncomplete", "OnMoveComplete");
            args.Add("oncompleteparams", targetRotation);

            iTween.MoveTo(this.gameObject, args);



            // Vector3 secondPosition = new Vector3(metro.transform.position.x, metro.transform.position.y, onGoingPosition.z);

            // while (isMoving && metro.transform.position.z != onGoingPosition.z)
            // {
            //     metro.transform.position = Vector3.MoveTowards(metro.transform.position, secondPosition, Time.deltaTime * speed);
            //     yield return null;
            // }
        }
        else
        {
            Vector3 firstPosition = zPos ? new Vector3(metro.transform.position.x, metro.transform.position.y, onGoingPosition.z + Generator.TILE_SIZE) : new Vector3(metro.transform.position.x, metro.transform.position.y, onGoingPosition.z - Generator.TILE_SIZE);

            while (isMoving && ((zPos && metro.transform.position.z != onGoingPosition.z + Generator.TILE_SIZE) || (!zPos && metro.transform.position.z != onGoingPosition.z - Generator.TILE_SIZE)))
            {
                metro.transform.position = Vector3.MoveTowards(metro.transform.position, firstPosition, Time.deltaTime * speed);
                yield return null;
            }

            // I have a metro, his position is metro.transform.position, I'm at a corner, I want it to go through the corner (which the end is at Vector3(onGoingPosition.x, metro.transform.position.y, metro.transform.position.z)) and then go to the next position (which is Vector3(onGoingPosition.x, onGoingPosition.y, onGoingPosition.z))
            // Code it but it should be smooth as a metro

            Vector3 secondPosition = new Vector3(onGoingPosition.x, metro.transform.position.y, metro.transform.position.z);

            while (isMoving && metro.transform.position.x != onGoingPosition.x)
            {
                metro.transform.position = Vector3.MoveTowards(metro.transform.position, secondPosition, Time.deltaTime * speed);
                yield return null;
            }
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

        StartCoroutine(MoveToPosition(direction));
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
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Move(Direction.RIGHT);
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
