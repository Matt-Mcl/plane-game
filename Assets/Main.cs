using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    Movement plane1Script;
    AIMovement plane2Script;
    Controls controlsScript;

    GameObject TheGameController;
    Collider2D plane1BoxCollider;
    Collider2D Plane1PolyCollider;
    Collider2D plane2BoxCollider;
    Collider2D Plane2PolyCollider;
    Collider2D battlefieldCollider;
    Text gameStateText;

    // Initialise a backwards move as first move to prevent immediate back moves
    int plane1LastMove = 6;
    
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

    // Start is called before the first frame update
    void Start() {
        TheGameController = GameObject.Find("Plane1");
        plane1Script = TheGameController.GetComponent<Movement>();
        plane1BoxCollider = TheGameController.GetComponent<BoxCollider2D>();
        Plane1PolyCollider = TheGameController.GetComponent<PolygonCollider2D>();
        TheGameController = GameObject.Find("Plane2");
        plane2Script = TheGameController.GetComponent<AIMovement>();
        plane2BoxCollider = TheGameController.GetComponent<BoxCollider2D>();
        Plane2PolyCollider = TheGameController.GetComponent<PolygonCollider2D>();
        TheGameController = GameObject.Find("Controls");
        controlsScript = TheGameController.GetComponent<Controls>();
        TheGameController = GameObject.Find("GameStateText");
        gameStateText = TheGameController.GetComponent<Text>();
        battlefieldCollider = GetComponent<Collider2D>();

        CheckMoves();
    }

    // Update is called once per frame
    void Update() {

    }

    public void PlayerMove(int moveIndex) {
        ToggleButtons(false);
        plane2Script.AiMove();
        plane1Script.target = plane1Script.planeTransform.TransformPoint(moves[moveIndex].Item1);
        plane1Script.rotation = plane1Script.rotation * Quaternion.Euler(0, 0, moves[moveIndex].Item2);
        plane1LastMove = moveIndex;

        StartCoroutine(CheckWinner());
    }

    void CheckMoves() {
        for (int i = 0; i < moves.Length; i++) {
            Vector3 newPosition = plane1Script.planeTransform.TransformPoint(moves[i].Item1);
            bool moveInBattlefield = battlefieldCollider.bounds.Contains(newPosition);
            // Checks if move is out of bounds or if it's a backwards move that was used last turn
            if (!moveInBattlefield || (plane1LastMove > 5 && i > 5)) {
                controlsScript.childImages[i].color = new Color(0.7f, 0, 0, 1);
                controlsScript.childButtons[i].interactable = false;
            } else {
                controlsScript.childImages[i].color = new Color(0.275f, 0.275f, 0.275f, 1);
                controlsScript.childButtons[i].interactable = true;
            }
        }
    }

    public void ToggleButtons(bool toggle) {
        for (int i = 0; i < 8; i++) {
            controlsScript.childButtons[i].interactable = toggle;
        }
    }

    IEnumerator CheckWinner() {
        yield return new WaitForSeconds(1);

        bool plane1CanShoot = plane2BoxCollider.bounds.Intersects(Plane1PolyCollider.bounds);
        bool plane2CanShoot = plane1BoxCollider.bounds.Intersects(Plane2PolyCollider.bounds);

        if (plane1CanShoot && !plane2CanShoot) {
            gameStateText.text = "Player wins";
            print("Player wins");
        } else if (!plane1CanShoot && plane2CanShoot) {
            gameStateText.text = "AI wins";
            print("AI wins");
        } else if (plane1CanShoot && plane2CanShoot) {
            gameStateText.text = "Both can shoot";
            print("Both can shoot");
        } else {
            gameStateText.text = "Neither can shoot";
            print("Neither can shoot");
        }


        CheckMoves();
    }
}
