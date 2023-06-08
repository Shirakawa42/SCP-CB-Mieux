using System;
using UnityEngine;

public class MetroMgr : MonoBehaviour
{
    GameObject metro;
    GameObject GameManager;
    Generator generator;

    Boolean isMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
        generator = GameManager.GetComponent<Generator>();
        metro = gameObject;
    }

    private string getMetroCurrentStation(Vector2Int currentPos) {
        if (currentPos == generator.metroStations[Generator.MetroStation.Top]) {
            return "Top";
        } else if (currentPos == generator.metroStations[Generator.MetroStation.Bottom]) {
            return "Bottom";
        } else if (currentPos == generator.metroStations[Generator.MetroStation.Left]) {
            return "Left";
        } else if (currentPos == generator.metroStations[Generator.MetroStation.Right]) {
            return "Right";
        } else {
            return "Unknown";
        }
    }

    private Vector2Int getNextMetroStationPos(Vector2Int currentPos, String direction) {
        Vector2Int nextPos = currentPos;

        String heightPos = currentPos.y > ((generator.metroStations[Generator.MetroStation.Top].y + generator.metroStations[Generator.MetroStation.Bottom].y))/2 ? "T" : "B";
        String widthPos = currentPos.x > ((generator.metroStations[Generator.MetroStation.Left].x + generator.metroStations[Generator.MetroStation.Right].x))/2 ? "R" : "L";

        Debug.Log(getMetroCurrentStation(currentPos));

        if (heightPos == "T") {
            if (widthPos == "R") {
                // top (exclu) -> Right (exclu)
                if (direction == "LEFT") {
                    Debug.Log("Moving to Top station");
                    return generator.metroStations[Generator.MetroStation.Top];
                } else {
                    Debug.Log("Moving to Right station");
                    return generator.metroStations[Generator.MetroStation.Right]; 
                }
            } else {
                // Left (exclu) -> Top (inclu)
                if (direction == "LEFT") {
                    Debug.Log("Moving to Left station");
                    return generator.metroStations[Generator.MetroStation.Left];
                } else {
                    Debug.Log(currentPos == generator.metroStations[Generator.MetroStation.Top] ?
                        "Moving to Right station" : "Moving to Top station");
                    return currentPos == generator.metroStations[Generator.MetroStation.Top] ?
                        generator.metroStations[Generator.MetroStation.Right] : generator.metroStations[Generator.MetroStation.Top];
                }
            }
        } else {
            if (widthPos == "R") {
                // Right (Inclu) -> Bottom (inclu)
                if (direction == "LEFT") {
                    if (currentPos == generator.metroStations[Generator.MetroStation.Right]) {
                        Debug.Log("Moving to Top station");
                        return generator.metroStations[Generator.MetroStation.Top];
                    } else {
                        Debug.Log("Moving to Right station");
                        return generator.metroStations[Generator.MetroStation.Right];
                    }
                } else {
                    if (currentPos == generator.metroStations[Generator.MetroStation.Bottom]) {
                        Debug.Log("Moving to Left station");
                        return generator.metroStations[Generator.MetroStation.Left];
                    } else {
                        Debug.Log("Moving to Bottom station");
                        return generator.metroStations[Generator.MetroStation.Bottom];
                    }
                }
            } else {
                // Left (inclu) -> botton (exclu)
                if (direction == "LEFT") {
                    if (currentPos == generator.metroStations[Generator.MetroStation.Left]) {
                        Debug.Log("Moving to Top station");
                        return generator.metroStations[Generator.MetroStation.Top];
                    } else {
                        Debug.Log("Moving to Left station");
                        return generator.metroStations[Generator.MetroStation.Left];
                    }
                } else {
                    Debug.Log("Moving to Bottom station");
                    return generator.metroStations[Generator.MetroStation.Bottom];
                }
            }
        }
    }

    private void MoveLeft() {
        if (isMoving) {
            return ;
        }

        Vector2Int currentPos = new Vector2Int((int)metro.transform.position.x, (int)metro.transform.position.y);
        Vector2Int newPos = getNextMetroStationPos(currentPos, "LEFT");

        isMoving = true;

        metro.transform.localPosition = new Vector3(newPos.x, newPos.y, metro.transform.position.z);

        isMoving = false;
    }

    private void MoveRight() {
        if (isMoving) {
            return ;
        }

        Vector2Int currentPos = new Vector2Int((int)metro.transform.position.x, (int)metro.transform.position.y);
        Vector2Int newPos = getNextMetroStationPos(currentPos, "LEFT");

        isMoving = true;

        metro.transform.localPosition = new Vector3(newPos.x, newPos.y, metro.transform.position.z);

        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) {
            Debug.Log("Moving left");
            MoveLeft();   
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            Debug.Log("Moving right");
            MoveRight();
        }

    }
}
