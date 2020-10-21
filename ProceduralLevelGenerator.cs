using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevelGenerator : MonoBehaviour
{
    public Vector3 startRoomSize;
    public int roomCount;
    public int minRoomSize;
    public int maxRoomSize;

    public Transform roomsParent;
    public GameObject floorPrefab;
    public bool regenerate = false;
    bool generateWalls = false;
    int wallWait = 2;

    public float wallHeight;
    public float doorWidth;
    public float doorHeight;
    public float doorThickness;
    public GameObject doorPrefab,wallPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
        //print(Vector3.Cross(Vector3.right, Vector3.up));
        //print(Vector3.Cross(Vector3.up, Vector3.forward));
    }
    private void Update()
    {
        if (regenerate == true)
        {
            DestroyFloor();
            GenerateFloor();

            generateWalls = true;
            regenerate = false;
           
        }
        if (generateWalls && wallWait == 0)
        {
            GenerateDoorWalls();
            GenerateOtherWalls();
            wallWait = 2;
            generateWalls = false;

        }
        else if (generateWalls)
        {
            wallWait--;
        }

    }
    private Vector3 RandomScale(Vector3 nextDirection, Transform currentRoom, int i)
    {
        Vector3 scale = Vector3.zero;
        if (i > 1)
        {
            if (nextDirection.x != 0)
            {
                scale.z = Random.Range(minRoomSize,(int)currentRoom.localScale.z + 1);
                scale.x = Random.Range(minRoomSize, maxRoomSize + 1);
            }
            else if (nextDirection.z != 0)
            {
                scale.x = Random.Range(minRoomSize, (int)currentRoom.localScale.x + 1);
                scale.z = Random.Range(minRoomSize, maxRoomSize + 1);
            }
            else
            {
                scale.x = Random.Range(minRoomSize, maxRoomSize + 1);
                scale.z = Random.Range(minRoomSize, maxRoomSize + 1);
            }

            
        }
        else
        {
            scale.x = Random.Range(minRoomSize, maxRoomSize + 1);
            scale.z = Random.Range(minRoomSize, maxRoomSize + 1);
        }
        return scale;
    }

    Vector3 RandomDirection(Vector3 ignore)
    {
        // don't generate in "ignore" direction
        // Direction guide (0 +x-axis, 90 +z-axis, 180 -x-axis, 270 -z-axis)

        int oldAngle = (int)Vector3.SignedAngle(Vector3.right, ignore, Vector3.up);
        //print(oldAngle);
        oldAngle -= 180;
        oldAngle *= -1;
        //print(oldAngle);

        int newAngle = Random.Range(0, 4) * 90;
        while (newAngle == oldAngle)
        {
            newAngle = Random.Range(0, 4) * 90;
        }
        //print("New:   "+newAngle);
        Vector3 direction = new Vector3( (int)Mathf.Cos(Mathf.Deg2Rad * (float)newAngle), 0, (int)Mathf.Sin(Mathf.Deg2Rad * (float)newAngle));
        return direction;
    }

    void GenerateFloor()
    {
        Vector3 nextDirection;
        Transform currentRoom;
        nextDirection = Vector3.forward;
        GameObject GO;
        GO = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity, roomsParent);
        GO.transform.localScale = startRoomSize;
        GO.name = "Foor_" + 0;
        currentRoom = GO.transform;

        for (int i = 0; i < roomCount; i++)
        {
            Vector3 scale;
            Vector3 nextPos;


            nextDirection = RandomDirection(nextDirection);
            //print("Next Direction: " + nextDirection);

            scale = RandomScale(nextDirection, currentRoom, i);
            scale.y = 1;
            //print("Scale" + scale);
            nextPos = currentRoom.position;
            nextPos += Vector3.Project(currentRoom.localScale + scale, nextDirection) * 0.5f;

            //print("NextPos:  " + nextPos);
            GO = Instantiate(floorPrefab, nextPos, Quaternion.identity, roomsParent);
            GO.transform.localScale = scale;
            GO.name = "Foor_" + (i+1);
            currentRoom = GO.transform;
            //print(nextDirection);


        }
    }
    void DestroyFloor()
    {
        foreach (Transform child in roomsParent)
        {
            Destroy(child.gameObject);
            
        }
    }
    void GenerateDoorWalls()
    {

        for (int i = 0;i<roomsParent.childCount - 1;i++) 
        {
            Transform currentRoom = roomsParent.GetChild(i);
            Transform nextRoom = roomsParent.GetChild(i + 1);
            Vector3 nextDir = nextRoom.position - currentRoom.position; // finding the direction of the next room to determine where to put the door
            //doorHeight is placed at half the scale distance in the direction of the next block

            Vector3 posOffset = Vector3.Project(currentRoom.localScale, nextDir) * 0.5f;
            Vector3 heightOffset = Vector3.up * (doorHeight * 0.5f + currentRoom.localScale.y*0.5f);
            Vector3 doorPos = currentRoom.position + posOffset + heightOffset;
            

            
            Quaternion doorRot =  Quaternion.LookRotation(nextDir.normalized, Vector3.up);

            GameObject GO = Instantiate(doorPrefab, doorPos, doorRot);
            GO.transform.localScale = new Vector3(doorWidth, doorHeight, doorThickness);
            GO.name = "Door_" + i;
            GO.transform.SetParent(currentRoom, true);
            //--------DONE CREATING DOOR--------//
             
            //door filling - stuff that goes between the door and ceiling
            Vector3 fillingScale = GO.transform.lossyScale;
            fillingScale.y = wallHeight - (doorHeight + currentRoom.localScale.y * 0.5f);
            Vector3 fillingPos = doorPos;
            fillingPos.y = wallHeight - fillingScale.y * 0.5f;

            GO = Instantiate(wallPrefab, fillingPos, doorRot);
            GO.name = "Wall_" + 0;
            GO.transform.localScale = fillingScale;
            GO.transform.SetParent(currentRoom, true);

            // Now the rest of the wall
            float wall0_Width = (OppositeScale(currentRoom.localScale, nextRoom.localScale, nextDir) - doorWidth) / 2f;
            
            Vector3 wall0_Scale = new Vector3(wall0_Width, wallHeight, doorThickness); // door width is the door thickness
            Vector3 wall01_pos = GO.transform.position + GO.transform.right * ((doorWidth / 2f) + wall0_Width / 2) ; // the wall-section to the right of the door
            wall01_pos.y = wallHeight * 0.5f;
            Vector3 wall02_pos = GO.transform.position + GO.transform.right * (-1f)*((doorWidth / 2f) + wall0_Width / 2) + Vector3.up * wallHeight * 0.5f; // the wall-section to the left of the door
            wall02_pos.y = wallHeight * 0.5f;
            GO = Instantiate(wallPrefab, wall01_pos, doorRot);
            GO.name = "Wall_" + 0+1;
            GO.transform.localScale = wall0_Scale;
            GO.transform.SetParent(currentRoom, true);
            GO = Instantiate(wallPrefab, wall02_pos, doorRot);
            GO.name = "Wall_" + 0+2;
            GO.transform.localScale = wall0_Scale;
            GO.transform.SetParent(currentRoom, true);
            
            //Now the other walls 

        }
    }
    void GenerateOtherWalls()
    {
        for (int i = 0; i < roomsParent.childCount; i++)
        {
            
            
            if (i == 0)
            {
                Transform currentRoom = roomsParent.GetChild(i);
                Transform nextRoom = roomsParent.GetChild(i + 1);
                Vector3 nextDir = (nextRoom.position - currentRoom.position);
                GenerateFirstLast(i,nextDir);
            }
            else if(i == roomsParent.childCount - 1)
            {
                Transform currentRoom = roomsParent.GetChild(i);
                Transform lastRoom = roomsParent.GetChild(i - 1);
                Vector3 lastDir = (lastRoom.position - currentRoom.position);
                GenerateFirstLast(i,lastDir);
            }
            else
            {
                
                GenrateMiddleWalls(i);

            }

        }
    }

    private Vector3 GenrateMiddleWalls(int i)
    {
        Transform currentRoom = roomsParent.GetChild(i);
        Transform nextRoom = roomsParent.GetChild(i + 1);
        Vector3 nextDir = (nextRoom.position - currentRoom.position);
        
        Transform lastRoom = roomsParent.GetChild(i - 1);
        Vector3 lastDir = (lastRoom.position - currentRoom.position);

        
        for (int j = 0; j < 3; j++)
        {
            OppositeScale(currentRoom.localScale, nextDir, out nextDir);
            float angle = Vector3.SignedAngle(nextDir, lastDir, Vector3.up);

            if (angle != 0)
            {
                Vector3 wallPos = currentRoom.position + (nextDir * 0.5f) + Vector3.up * wallHeight / 2f;
                Quaternion wallRotation = Quaternion.LookRotation(nextDir);
                GameObject GO = Instantiate(wallPrefab, wallPos, wallRotation);
                GO.name = "Wall_" + (j + 1);
                float wallWidth = OppositeScale(currentRoom.localScale, nextDir);
                GO.transform.localScale = new Vector3(wallWidth, wallHeight, doorThickness);
                GO.transform.SetParent(currentRoom, true);
            }

        }

        return nextDir;
    }

    private void GenerateFirstLast(int i,Vector3 nextDir)
    {
        Transform currentRoom = roomsParent.GetChild(i);
        for (int j = 0; j < 3; j++)
        {

            OppositeScale(currentRoom.localScale, nextDir, out nextDir);
            Vector3 wallPos = currentRoom.position + (nextDir * 0.5f) + Vector3.up * wallHeight / 2f;
            Quaternion wallRotation = Quaternion.LookRotation(nextDir);
            GameObject GO = Instantiate(wallPrefab, wallPos, wallRotation);
            GO.name = "Wall_" + (j + 1);
            float wallWidth = OppositeScale(currentRoom.localScale, nextDir);
            GO.transform.localScale = new Vector3(wallWidth, wallHeight, doorThickness);
            GO.transform.SetParent(currentRoom, true);

        }
    }

    // returns the scale of object in the other dir of input vector
    float OppositeScale(Vector3 scale1, Vector3 scale2, Vector3 inputDir)
    {
        float output1 = 0, output2 = 0;

        Vector3 cross = Vector3.Cross(Vector3.up, inputDir.normalized);
        output1 = Mathf.Abs(Vector3.Dot(scale1, cross));
        output2 = Mathf.Abs(Vector3.Dot(scale2, cross));

        output1 = (output2 >= output1) ? output2 : output1;
        return output1;
    }
    float OppositeScale(Vector3 scale1, Vector3 inputDir)
    {
        float output1 = 0;

        Vector3 cross = Vector3.Cross(Vector3.up, inputDir.normalized);
        output1 = Mathf.Abs(Vector3.Dot(scale1, cross));
        return output1;
    }
    float OppositeScale(Vector3 scale, Vector3 inputDir, out Vector3 output)
    {
        
        Vector3 cross = Vector3.Cross(Vector3.up, inputDir.normalized);

        output = Mathf.Abs(Vector3.Dot(scale, cross)) * cross;
        return output.magnitude;
    }
}
