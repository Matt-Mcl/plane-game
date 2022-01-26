using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour {

    Movement plane1Script;
    bool canMove;
    bool dragging;
    Collider2D controlCollider;
    GameObject TheGameController;

    public Image[] childImages = new Image[8];
    public Button[] childButtons = new Button[8];

    // Use this for initialization
    void Awake() {
        for (int i = 0; i < 8; i++) {
            childImages[i] = transform.GetChild(i).GetComponent<Image>();
            childButtons[i] = transform.GetChild(i).GetComponent<Button>();
        }
    }

    // Start is called before the first frame update
    void Start(){
        controlCollider = GetComponent<Collider2D>();
        canMove = false;
        dragging = false;
        TheGameController = GameObject.Find("Plane1");
        plane1Script = TheGameController.GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update(){
        float rot = 50.0f * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, plane1Script.planeTransform.rotation, rot);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            if (controlCollider == Physics2D.OverlapPoint(mousePos)) {
                canMove = true;
            } else {
                canMove = false;
            } if (canMove) {
                dragging = true;
            }
        }

        if (dragging) {
            this.transform.position = mousePos;
        }

        if (Input.GetMouseButtonUp(0)) {
            canMove = false;
            dragging = false;
        }
    }
}
