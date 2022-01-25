using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    Movement plane1Script;
    Main mainScript;
    Vector3 target;
    Quaternion rotation;
    public Transform planeTransform;
    Collider2D battlefieldCollider;
    GameObject TheGameController;

    // Initialise a backwards move as first move to prevent immediate back moves
    int lastMove = 6;

    Tuple<float, float>[,] directionMatrix = new Tuple<float, float>[8, 8];
    int[,] payoffMatrix = new int[8, 8];

    Tuple<Vector3, int>[] moves = {
        Tuple.Create(new Vector3(0, 4, 0), 0),
        Tuple.Create(new Vector3(1.5f, 3.5f, 0), -45),
        Tuple.Create(new Vector3(-1.5f, 3.5f, 0), 45),
        Tuple.Create(new Vector3(0, 5, 0), 0),
        Tuple.Create(new Vector3(2, 4.5f, 0), -30),
        Tuple.Create(new Vector3(-2, 4.5f, 0), 30),
        Tuple.Create(new Vector3(-1, -1, 0), 180),
        Tuple.Create(new Vector3(1, -1, 0), 180)
    };

    // Use this for initialization
    void Awake () {
        planeTransform = transform;

        target = transform.TransformPoint(0, 0, 0);
        rotation = transform.rotation;        
    }
    
    // Start is called before the first frame update
    void Start() {
        TheGameController = GameObject.Find("Plane1");
        plane1Script = TheGameController.GetComponent<Movement>();
        TheGameController = GameObject.Find("Battlefield");
        mainScript = TheGameController.GetComponent<Main>();
        battlefieldCollider = mainScript.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        float step = 100.0f * Time.deltaTime;
        float rot = 5.0f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rot);
    }

    public void AiMove() {
        FillMatrix(directionMatrix);

        // PrintDirectionMatrix(directionMatrix);

        CreatePayoff(directionMatrix, payoffMatrix);

        CheckMoves(payoffMatrix);

        // PrintPayoffMatrix(payoffMatrix);

        ChooseMove(payoffMatrix);
    }

    public Tuple<float, float> CreateData(int i, int j) {
        Vector3 newPlayerPosition = plane1Script.planeTransform.TransformPoint(moves[i].Item1.x, moves[i].Item1.y, moves[i].Item1.z);
        Vector3 newAIPosition = transform.TransformPoint(moves[j].Item1.x, moves[j].Item1.y, moves[j].Item1.z);
        int playerRot = moves[i].Item2;
        int aiRot = moves[j].Item2;

        //dirToPlayer
        float aiNewAngle = Mathf.Deg2Rad * (transform.eulerAngles.z + aiRot + 90);
        Vector3 aiNewVector = new Vector3(Mathf.Cos(aiNewAngle), Mathf.Sin(aiNewAngle), 0);
        Vector3 heading1 = newPlayerPosition - newAIPosition;
        float dirToPlayer = AngleDir(heading1, aiNewVector);
        //dirToAI
        float pNewAngle = Mathf.Deg2Rad * (plane1Script.planeTransform.eulerAngles.z + playerRot + 90);
        Vector3 pNewVector = new Vector3(Mathf.Cos(pNewAngle), Mathf.Sin(pNewAngle), 0);
        Vector3 heading2 = newAIPosition - newPlayerPosition;
        float dirToAi = AngleDir(heading2, pNewVector);

        return new Tuple<float, float>(dirToPlayer, dirToAi);
    }

    public void FillMatrix(Tuple<float, float>[,] directionMatrix) {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                directionMatrix[i, j] = CreateData(i, j);
            }
        }
    }

    public void PrintDirectionMatrix(Tuple<float, float>[,] matrix) {
        print("-------------------");
        for(int i = 0; i < 8; i++) {
            string row = "";
            for(int j = 0; j < 8; j++) { 
                row = row + "|" + matrix[i, j];
            }
            print(row);
        }
        print("-------------------");
    }

    public void PrintPayoffMatrix(int[,] matrix) {
        print("-------------------");
        for(int i = 0; i < 8; i++) {
            string row = "";
            for(int j = 0; j < 8; j++) { 
                row = row + "|" + matrix[i, j];
            }
            print(row);
        }
        print("-------------------");
    }

    public int[,] CreatePayoff(Tuple<float, float>[,] directionMatrix, int[,] payoffMatrix) {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                float dtp = directionMatrix[i, j].Item1;
                float dta = directionMatrix[i, j].Item2;
                payoffMatrix[i, j] = GetPayoff(dtp, dta);
            }
        }
        return payoffMatrix;
    }

    public int GetPayoff(float dtp, float dta) {
        // TODO simplify this?
        if (dtp >= -45 && dtp <= 45) {
            if (dta >= -45 && dta <= 45) {
                return -1;
            } else if (Math.Abs(dta) >= 45 && Math.Abs(dta) <= 135) {
                return 2;
            } else if (dta <= -135 || dta >= 135) {
                return 3;
            }
        } else if (Math.Abs(dtp) >= 45 && Math.Abs(dtp) <= 135) {
            if (dta >= -45 && dta <= 45) {
                return -2;
            } else if (Math.Abs(dta) >= 45 && Math.Abs(dta) <= 135) {
                return 0;
            } else if (dta <= -135 || dta >= 135) {
                return 0;
            }
        } else if (dtp <= -135 || dtp >= 135) {
            if (dta >= -45 && dta <= 45) {
                return -3;
            } else if (Math.Abs(dta) >= 45 && Math.Abs(dta) <= 135) {
                return 0;
            } else if (dta <= -135 || dta >= 135) {
                return 0;
            }
        }
        return -100;
    }

    public void ChooseMove(int[,] payoffMatrix) {
        // Ai = columns, Player = rows
        int max = -100;
        List<int> aiMoves = new List<int>();
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                int item = payoffMatrix[i, j];
                if (item > max) {
                    aiMoves.Clear();
                    aiMoves.Add(j);
                    max = item;
                } else if (item == max) {
                    aiMoves.Add(j);
                }
            }
        }
        // foreach (int item in aiMoves) {
        //     print("Potential move: " + item);
        // }
        System.Random rnd = new System.Random();
        int rand = rnd.Next(0, aiMoves.Count);

        int move = aiMoves[rand];
        lastMove = move;

        target = transform.TransformPoint(moves[move].Item1);
        rotation = transform.rotation * Quaternion.Euler(0, 0, moves[move].Item2);
    }


    public float AngleDir(Vector3 targetDir, Vector3 up) {
        float cosAngle = Vector3.Dot(targetDir, up) / (targetDir.magnitude * up.magnitude);
        float turnAngle = Mathf.Acos(cosAngle);
        int sign;
        if (Vector3.Cross(up, targetDir).z > 0) {
            sign = 1;
        }
        else {
            sign = -1;
        }
        turnAngle = turnAngle * sign;
        turnAngle = turnAngle * Mathf.Rad2Deg;
        return turnAngle;
    }

    void CheckMoves(int[,] payoffMatrix) {
        // Prevent the AI leaving the battlefield
        for (int i = 0; i < 8; i++) {
            Vector3 newPosition = transform.TransformPoint(moves[i].Item1);
            if (!battlefieldCollider.bounds.Contains(newPosition)) {
                for (int j = 0; j < 8; j++) {
                    payoffMatrix[j, i] = -100;
                }
            }
        }
        // Prevent the AI turning around twice in a row
        if (lastMove > 5) {
            for (int i = 6; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    payoffMatrix[j, i] = -100;
                }
            }
        }
    }
}
