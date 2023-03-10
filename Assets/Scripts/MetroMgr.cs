using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEditor.IMGUI.Controls;
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

    private string getMetroCurrentStation(Vector3 currentPos) {
        if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position) {
            return "Top";
        } else if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position) {
            return "Bottom";
        } else if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position) {
            return "Left";
        } else if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position) {
            return "Right";
        } else {
            return "Unknown";
        }
    }

    private Vector3 getNextMetroStationPos(Vector3 currentPos, String direction) {
        Debug.Log(getMetroCurrentStation(currentPos));

        if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position) {
            if (direction == "LEFT") {
                return generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position;
            } else {
                return generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position;
            }
        } else if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position) {
            if (direction == "LEFT") {
                return generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position;
            } else {
                return generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position;
            }
        } else if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position) {
            if (direction == "LEFT") {
                return generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position;
            } else {
                return generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
            }
        } else if (currentPos == generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position) {
            if (direction == "LEFT") {
                return generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position;
            } else {
                return generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position;
            }
        } else {
            return currentPos;
        }
        
        // @TODO: Better generation
        // String heightPos = currentPos.z > ((generator.metroStationPrefabs[Generator.MetroStation.Top].transform.position.z + generator.metroStationPrefabs[Generator.MetroStation.Bottom].transform.position.z))/2 ? "T" : "B";
        // String widthPos = currentPos.x > ((generator.metroStationPrefabs[Generator.MetroStation.Left].transform.position.x + generator.metroStationPrefabs[Generator.MetroStation.Right].transform.position.x))/2 ? "R" : "L";

        // if (heightPos == "T") {
        //     if (widthPos == "R") {
        //         // top (exclu) -> Right (exclu)
        //     } else {
        //         // Left (exclu) -> Top (inclu)
        //     }
        // } else {
        //     if (widthPos == "R") {
        //         // Right (Inclu) -> Bottom (inclu)
        //     } else {
        //         // Left (inclu) -> botton (exclu)
        //     }
        // }
    }

    private void MoveLeft() {
        if (isMoving) {
            return ;
        }

        Vector3 newPos = getNextMetroStationPos(metro.transform.position, "LEFT");

        isMoving = true;

        metro.transform.position = newPos;

        isMoving = false;
    }

    private void MoveRight() {
        if (isMoving) {
            return ;
        }

        Vector3 newPos = getNextMetroStationPos(metro.transform.position, "RIGHT");

        isMoving = true;

        metro.transform.position = newPos;

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
