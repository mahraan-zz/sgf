﻿using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

  public int player_number;
  public string player_color;

  public enum orientation { up, down, left, right };
  public orientation player_orientation;

  Animator player_animator;
  Rigidbody2D body;

  public Animator slash_animator;
  public Animator side_slash_animator;
  public Animator up_slash_animator;
  public Animator down_slash_animator;
  public Animator shield_animator;
  
  public GameObject slash;
  public GameObject side_slash;
  public GameObject up_slash;
  public GameObject down_slash;

  public float speed;
  public float thrust;
  public float acceleration;
  public int grounded;

  bool move_left = false;
  bool move_right = false;
  bool move_up = false;
  bool move_down = false;

  Vector2 right, left, down, up;

  AudioSource sound;
  public AudioClip gunshot;
  public GameObject bullet;

  GameObject bulletGO;

  public float shotVelocity = 5f, numBullets = 1;
  float fireRate = 1.5f, nextFire = 0f;
  string lastDirection = "right";

  void Start(){
    player_animator = GetComponent<Animator>();
    body = gameObject.GetComponent<Rigidbody2D>();
    grounded = 0;
    player_orientation = orientation.down;
    body.gravityScale = 0;    
    slash.GetComponent<BoxCollider2D>().enabled = false;
    side_slash.GetComponent<BoxCollider2D>().enabled = false;
    up_slash.GetComponent<BoxCollider2D>().enabled = false;
    down_slash.GetComponent<BoxCollider2D>().enabled = false;
  }
  
  void Update(){

    // gravity vectors
    right = new Vector2(acceleration, 0f);
    left = new Vector2(-acceleration, 0f);
    down = new Vector2(0f, -acceleration);
    up = new Vector2(0f, acceleration);

    // swap gravity orientation
    if((Input.GetButtonDown("Controller " + player_number + " Y Button") || Input.GetKey(KeyCode.W)) && player_orientation != orientation.up) { 
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.up;
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 180f);
      player_animator.Play("Swap");
    }
    if((Input.GetButtonDown("Controller " + player_number + " A Button") || Input.GetKey(KeyCode.S)) && player_orientation != orientation.down){
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.down;
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -transform.localEulerAngles.y, 0f);
      player_animator.Play("Swap");
    }
    if((Input.GetButtonDown("Controller " + player_number + " X Button") || Input.GetKey(KeyCode.A)) && player_orientation != orientation.left){
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.left;
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, -90f);
      player_animator.Play("Swap");
    }
    if((Input.GetButtonDown("Controller " + player_number + " B Button") || Input.GetKey(KeyCode.D)) && player_orientation != orientation.right){
      body.velocity = new Vector2(0f, 0f);
      player_orientation = orientation.right;
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, 90f);
      player_animator.Play("Swap");
    }

    // attack
    if(Input.GetAxis("Controller " + player_number + " Right Trigger") >= 0.9 || Input.GetKey(KeyCode.Space)){
      Attack();
    }

    // shoot
    if((Input.GetAxis("Controller " + player_number + " Right Bumper") >= 0.9 || Input.GetKey(KeyCode.LeftShift)) && Time.time > nextFire && numBullets > 0){
      Shoot();
    }

    // block
    if(Input.GetAxis("Controller " + player_number + " Left Trigger") >= 0.9 || Input.GetKey(KeyCode.Q)){
      Block();
    }

    // super slash for shits and gigs
    if(Input.GetButtonDown("Controller " + player_number + " Left Bumper") || Input.GetKey(KeyCode.F)){
      down_slash_animator.Play("Slash");
      up_slash_animator.Play("Slash");
      slash_animator.Play("Slash");
      player_animator.Play("Attack");
      slash.GetComponent<BoxCollider2D>().enabled = true;
      up_slash.GetComponent<BoxCollider2D>().enabled = true;
      down_slash.GetComponent<BoxCollider2D>().enabled = true;
    }

    move_left = false;
    move_right = false;
    move_up = false;
    move_down = false;

    // move right
    if(Input.GetAxis("Controller " + player_number + " Left Stick X Axis") >= 0.9f || Input.GetKey(KeyCode.RightArrow)){
      if(player_orientation == orientation.up){
        move_left = true;
        lastDirection = "left";
      }
      else if(player_orientation == orientation.left){
        move_up = true;
        lastDirection = "up";
      }
      else if(player_orientation == orientation.right){
        move_down = true;
        lastDirection = "down";
      }
      else{
        move_right = true;
        lastDirection = "right";
      }
    }

    // move left
    if(Input.GetAxis("Controller " + player_number + " Left Stick X Axis") <= -0.9f || Input.GetKey(KeyCode.LeftArrow)){
      if(player_orientation == orientation.up){
        move_right = true;
        lastDirection = "right";
      }
      else if(player_orientation == orientation.left){
        move_down = true;
        lastDirection = "down";
      }
      else if(player_orientation == orientation.right){
        move_up = true;
        lastDirection = "up";
      }
      else{
        move_left = true;
        lastDirection = "left";
      }
    }

    // move up
    if(Input.GetAxis("Controller " + player_number + " Left Stick Y Axis") <= -0.9f || Input.GetKey(KeyCode.UpArrow)){
      if(player_orientation == orientation.up){
        move_down = true;
        lastDirection = "down";
      }
      else if(player_orientation == orientation.left){
        move_left = true;
        lastDirection = "left";
      }
      else if(player_orientation == orientation.right){
        move_right = true;
        lastDirection = "right";
      }
      else{
        move_up = true;
        lastDirection = "up";
      }
    }

    // move down
    if(Input.GetAxis("Controller " + player_number + " Left Stick Y Axis") >= 0.9f || Input.GetKey(KeyCode.DownArrow)){
      if(player_orientation == orientation.up){
        move_up = true;
        lastDirection = "up";
      }
      else if(player_orientation == orientation.left){
        move_right = true;
        lastDirection = "right";
      }
      else if(player_orientation == orientation.right){
        move_left = true;
        lastDirection = "left";
      }
      else{
        move_down = true;
        lastDirection = "down";
      }
    }

    // apply movement
    if(move_right){
      Run(true);
    }
    if(move_left){
      Run(false);
    }

    // crouch
    player_animator.SetBool("crouched", false);
    if(move_down){
      Crouch();
    }
    

    if(!Input.anyKey){
      player_animator.SetBool("run", false);
    }

    if(!player_animator.GetBool("attack")){
      slash.GetComponent<BoxCollider2D>().enabled = false;
      side_slash.GetComponent<BoxCollider2D>().enabled = false;
      up_slash.GetComponent<BoxCollider2D>().enabled = false;
      down_slash.GetComponent<BoxCollider2D>().enabled = false;
    }

    // respawn
    if(respawn && respawning){
      respawning = false;
      StartCoroutine(Blink());
    }

  }

  void FixedUpdate(){

    // apply gravity
    if(player_orientation == orientation.down){
      body.AddForce(down);
    }
    else if(player_orientation == orientation.up){
      body.AddForce(up);
    }
    else if(player_orientation == orientation.left){
      body.AddForce(left);
    }
    else if(player_orientation == orientation.right){
      body.AddForce(right);
    }
    
    // apply jump
    if(move_up && grounded == 1){
      Jump();
    }

  }

  void Crouch(){
    if(player_animator.GetBool("grounded")){
      player_animator.SetBool("crouched", true);
      player_animator.Play("Down");
    }
  }

  void Run(bool right){

    bool can_move = true;
    can_move = !(player_animator.GetBool("attack") && grounded == 1);
    can_move = can_move && !(player_animator.GetBool("block") && grounded == 1);


    if(can_move){
      
      float rotation = 0f;
      float flip = 1f;

      if(player_orientation == orientation.up || player_orientation == orientation.down){
        if(!right){
          rotation = 180f;
        }
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotation, transform.localEulerAngles.z);
      }

      else{

        if(!right){
          rotation = 180f;
          if(player_orientation == orientation.right){
            flip = -1f;
          }
        }
        else if(player_orientation == orientation.left){
          flip = -1f;
        }

        transform.localEulerAngles = new Vector3(0f, rotation, 90f * flip);

      }

      if(!player_animator.GetBool("crouched")){
        transform.localPosition += transform.right * speed * Time.deltaTime;
      }

      if(player_animator.GetBool("grounded")){
        player_animator.SetBool("run", true);
      }

    }
  }

   void Attack(){

      if(!player_animator.GetBool("attack") && !respawn){

        player_animator.SetBool("attack", true);

        if(!player_animator.GetBool("grounded")){
          if(move_left || move_right){
            player_animator.Play("Side_Attack");
            side_slash_animator.Play("Slash");
            side_slash.GetComponent<BoxCollider2D>().enabled = true;
          }
          else if (move_down){
            player_animator.Play("Down_Attack");
            down_slash_animator.Play("Slash");
            down_slash.GetComponent<BoxCollider2D>().enabled = true;
          }
          else if (move_up){
            player_animator.Play("Up_Attack");
            up_slash_animator.Play("Slash");
            up_slash.GetComponent<BoxCollider2D>().enabled = true;
          }
          else {
            player_animator.Play("Attack");
            slash_animator.Play("Slash");
            slash.GetComponent<BoxCollider2D>().enabled = true;
          }
        }
        else {
          player_animator.Play("Attack");
          slash_animator.Play("Slash");
          slash.GetComponent<BoxCollider2D>().enabled = true;
        }
      }

    }
    
    void Shoot(){

      sound.PlayOneShot(gunshot);
      nextFire = Time.time + fireRate;

      Vector3 pos = transform.position, rot = transform.rotation.eulerAngles;
      rot.x = 0;
      rot.y = 0;

      // bullet rotation
      if(lastDirection == "up" && player_orientation == orientation.down ||
        (lastDirection == "up" && player_orientation == orientation.up) ||
        (lastDirection == "left" && player_orientation == orientation.left) ||
        (lastDirection == "right" && player_orientation == orientation.right)){
        rot.z = 90;
      }
      else if(lastDirection == "left" && player_orientation == orientation.down ||
             (lastDirection == "right" && player_orientation == orientation.up) ||
             (lastDirection == "down" && player_orientation == orientation.left) ||
             (lastDirection == "up" && player_orientation == orientation.right)){
        rot.z = 180;
      }
      else if(lastDirection == "down" && player_orientation == orientation.down ||
             (lastDirection == "down" && player_orientation == orientation.down) ||
             (lastDirection == "right" && player_orientation == orientation.left) ||
             (lastDirection == "left" && player_orientation == orientation.right)){
        rot.z = 270;
      }
      else if((lastDirection == "down" && player_orientation == orientation.down) ||
             (lastDirection == "left" && player_orientation == orientation.up) ||
             (lastDirection == "up" && player_orientation == orientation.left) ||
             (lastDirection == "down" && player_orientation == orientation.right)){
        rot.z = 0;
      }


      // bullet initiation
      if((lastDirection == "right" && player_orientation == orientation.down) || 
         (lastDirection == "left" && player_orientation == orientation.up) ||
         (lastDirection == "down" && player_orientation == orientation.right) ||
         (lastDirection == "up" && player_orientation == orientation.left)){
        pos.x = transform.position.x + 0.15f;
        bulletGO = Instantiate(bullet, pos, Quaternion.Euler(rot)) as GameObject;
        bulletGO.GetComponent<Rigidbody2D>().velocity = Vector3.right * shotVelocity;
      }
      else if((lastDirection == "left" && player_orientation == orientation.down) || 
              (lastDirection == "right" && player_orientation == orientation.up) ||
              (lastDirection == "up" && player_orientation == orientation.right) ||
              (lastDirection == "down" && player_orientation == orientation.left)){
        pos.x = transform.position.x - 0.15f;
        bulletGO = Instantiate(bullet, pos, Quaternion.Euler(rot)) as GameObject;
        bulletGO.GetComponent<Rigidbody2D>().velocity = Vector3.left * shotVelocity;
      }
      else if((lastDirection == "up" && player_orientation == orientation.down) ||
              (lastDirection == "down" && player_orientation == orientation.up) ||
              (lastDirection == "right" && player_orientation == orientation.right) ||
              (lastDirection == "left" && player_orientation == orientation.left)){
        pos.y = transform.position.y + 0.15f;
        bulletGO = Instantiate(bullet, pos, Quaternion.Euler(rot)) as GameObject;
        bulletGO.GetComponent<Rigidbody2D>().velocity = Vector3.up * shotVelocity;
      }
      else if((lastDirection == "down" && player_orientation == orientation.down) || 
              (lastDirection == "up" && player_orientation == orientation.up) ||
              (lastDirection == "left" && player_orientation == orientation.right) ||
              (lastDirection == "right" && player_orientation == orientation.left)){
        pos.y = transform.position.y - 0.15f;
        bulletGO = Instantiate(bullet, pos, Quaternion.Euler(rot)) as GameObject;
        bulletGO.GetComponent<Rigidbody2D>().velocity = Vector3.down * shotVelocity;
      }
    }

  void Block(){
    player_animator.Play("Block");
    player_animator.SetBool("block", true);
    shield_animator.Play("Shield");
  }

  void Jump(){
    player_animator.Play("Jump");
    player_animator.SetBool("run", false);
    player_animator.SetBool("jump", true);
    body.AddForce(transform.up * thrust);   
  }

  void OnCollisionEnter2D(Collision2D coll){
    GameObject other = coll.gameObject;

    if(other.tag == "ground"){
      grounded += 1;
      player_animator.SetBool("grounded", true);
    }
  }

  void OnCollisionExit2D(Collision2D coll){
    // leaving ground from jump
    if(coll.gameObject.tag == "ground"){
      grounded -= 1;
      if(grounded <= 0){
        grounded = 0;
        player_animator.SetBool("grounded", false);
        if(!player_animator.GetBool("jump")){
          player_animator.Play("Falling");
          print("yo");
        }
      }
    }
  }

  void OnTriggerEnter2D(Collider2D col){
    if (col.tag == "slash" && !respawn){
      KillPlayer();
    }
  }

  bool respawn = false, respawning = false;
  void KillPlayer(){
    player_animator.Play("Death");
    body.velocity = new Vector2(0f, 0f);
    player_orientation = orientation.down;
    StartCoroutine(Wait());
  }

  public Vector3 offscreen = new Vector3(-1000, -1000, -1000);
  
  IEnumerator Wait(){
    yield return new WaitForSeconds(0.75f);
    transform.position = offscreen;
    yield return new WaitForSeconds(2f);
    //transform.position = Level.S.findRespawn();
    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -transform.localEulerAngles.y, 0f);
    transform.position = Level.S.respawnPoints[UnityEngine.Random.Range(0, Level.S.respawnPoints.Length)];
    respawn = true;
    respawning = true;
  }

  IEnumerator Blink(){

    transform.GetComponent<Renderer>().enabled = false;
    yield return new WaitForSeconds(0.2f);
    transform.GetComponent<Renderer>().enabled = true;
    yield return new WaitForSeconds(0.5f);

    transform.GetComponent<Renderer>().enabled = false;
    yield return new WaitForSeconds(0.2f);
    transform.GetComponent<Renderer>().enabled = true;
    yield return new WaitForSeconds(0.5f);

    transform.GetComponent<Renderer>().enabled = false;
    yield return new WaitForSeconds(0.2f);
    transform.GetComponent<Renderer>().enabled = true;
    yield return new WaitForSeconds(0.75f);

    transform.GetComponent<Renderer>().enabled = false;
    yield return new WaitForSeconds(0.1f);
    transform.GetComponent<Renderer>().enabled = true;
    yield return new WaitForSeconds(1f);
    respawn = false;

  }

}
