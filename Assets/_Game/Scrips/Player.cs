﻿using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask BrickBlock;
    [SerializeField] private Vector3 StartPoint;
    [SerializeField] public GameObject brickPrefab;
    [SerializeField] public GameObject yellow;
    [SerializeField] public GameObject StartGame;
    [SerializeField] public GameObject WinGame;
    [SerializeField] public GameObject RePlayGame;
    [SerializeField] private int countBrick;
    [SerializeField] private List<GameObject> cloneBrick = new List<GameObject>();

    private float brickHeight;
    private bool isMoving = false;
    private bool isCheck = false;
    private Vector3 currentPos;
    private Vector3 targetPos;
    private Vector3 savePoint;

    private void Start()
    {
        if (!isCheck)
        {
            isMoving = true;
            StartGame.SetActive(true);
        }
        countBrick = 0;
        brickHeight = 0.3f;
        transform.position = new Vector3(StartPoint.x, StartPoint.y, StartPoint.z);
        //SpointMap map = new SpointMap();
        //cl = new List<GameObject>();
    }

    private void Update()
    {
        Control();

       

                //cloneBrick.Clone<SpointMap>
                /* for (CloneBrickList)
                 {
                     Vector3.Distance(this.transform.position, cloneBrick[i].pos)
                         {
                         cloneBrick[i].tag ===
                     }

                 }*/
                //1 duyệt qua tất cả các list
                //2 check xem trong list đó những cái gì là gạch
                //3 khi đi lại gần các viên gạch thì làm gì...
                /* if (spointM != null)
                 {
                     foreach (GameObject brick in spointM.ListBrick)
                     {
                         //float concac = Vector3.Distance(transform.position, brick.transform.position);
                         *//*if (brick.tag == "brick")
                         {
                             HandleBrick(brick);
                         }*//*
                         if (Vector3.Distance(transform.position, brick.transform.position) < 0.1f)
                         {
                             Debug.Log(1);
                         }


                     }
                 }*/


            }

    public void OnInit()
    {
        UIManager.instance.SetBrick(countBrick);

    }

    public void StartBrick()
    {
        isMoving = false;
        StartGame.SetActive(false);
    }
    public void Wingame()
    {
        SceneManager.LoadScene(0);
    }
    public void RePlaygame()
    {
        SceneManager.LoadScene(0);
    }

    private void Control()
    {
        if (!isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                targetPos = Input.mousePosition;
                Vector3 moveDir = targetPos - currentPos;
                moveDir = GetDirection(moveDir);
                if (moveDir != Vector3.zero)
                {
                    StopAllCoroutines();
                    StartCoroutine(Move(moveDir));
                    StartCoroutine(CheckWall(moveDir));
                }
            }
        }
    }

    private Vector3 GetDirection(Vector3 direction)
    {
        float horizontal = Mathf.RoundToInt(direction.x);
        float vertical = Mathf.RoundToInt(direction.y);

        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            return new Vector3(horizontal * 3f, 0f, 0f).normalized;
        }
        else if (Mathf.Abs(vertical) > Mathf.Abs(horizontal))
        {
            return new Vector3(0f, 0f, vertical * 3f).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private IEnumerator Move(Vector3 direction)
    {
        while (true)
        {
            isMoving = true;
            Vector3 movement = new Vector3(direction.x, 0f, direction.z);
            transform.position += movement * speed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator CheckWall(Vector3 direction)
    {
        while (true)
        {
            RaycastHit hit;
            Vector3 raycastPos = transform.position;
            if (Physics.Raycast(raycastPos, direction, out hit, 0.5f, wallLayer))
            {
                StopAllCoroutines();
                isMoving = false;
            }
            yield return null;
        }
    }
   
    private void OnTriggerEnter(Collider collision)
    {

        if (collision.tag == "brick")
        {
            countBrick += 1;
            UIManager.instance.SetBrick(countBrick);
            Destroy(collision.gameObject);
            GameObject newBrick = Instantiate(brickPrefab, transform);
            newBrick.transform.position -= new Vector3(0, countBrick * brickHeight + 1.5f, 0);
            transform.position += new Vector3(0, brickHeight, 0);
            cloneBrick.Add(newBrick);
        }
        if (collision.tag == "StopPoint")
        {
            StopAllCoroutines();
            isMoving = false;
        }
        if (collision.tag == "win")
        {
            StopAllCoroutines();
            isMoving = false;
            WinGame.SetActive(true);
        }
        if (collision.tag == "unBrick")
        {
            countBrick -= 1;
            UIManager.instance.SetBrick(countBrick);
            if (countBrick == 0)
            {
                StopAllCoroutines();
                isMoving = true;
                RePlayGame.SetActive(true);
            }
            if (cloneBrick.Count > 0)
            {
                GameObject lastBrick = cloneBrick[cloneBrick.Count - 1];
                transform.position -= new Vector3(0,brickHeight, 0);
                Destroy(lastBrick.transform.gameObject);
                GameObject line = Instantiate(yellow, new Vector3(transform.position.x, transform.position.y - brickHeight * countBrick - 0.3f, transform.position.z + 1.1f), Quaternion.Euler(90, 0, 0));
                cloneBrick.RemoveAt(cloneBrick.Count - 1);
            }

        }

    }
}
