﻿using UnityEngine;

public class GuardScript : MonoBehaviour {

	public Transform player;

	public float attackDistance ;
	public float runningDistance ;
	public float speed ;
	public float walkingSpeed;

	public bool found;

	public BoxCollider vision;

	public GameObject target;
	public GameObject playerMan;
	public GameObject friend;
	public GameObject guard;

	public GameObject cp1;
	public GameObject cp2;
	public GameObject cp3;
	public GameObject cp4;

	public Vector3 initialPos;
	public Quaternion initialRot;

	GuardAnimHandler anim;

	private Vector3 position;
	private Vector3 direction;
	private int count;

	public bool north;
	public bool south;
	public bool east;
	public bool west;

	private bool comeback = false;
	void Start () {
		anim = GetComponent<GuardAnimHandler> ();
		if (guard.CompareTag ("hwG1")) {
			north = true;
			south = false;
			east = false;
			west = false;
		} else if (guard.CompareTag ("hwG2")) {
			north = false;
			south = true;
			east = false;
			west = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (found && (Vector3.Distance (player.position, transform.position) > attackDistance) && (player.transform.position.z > 17.0f)) {
			anim.ToRunning ();
			direction = player.position - transform.position;
			direction.y = 0;
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), 0.5f);
			transform.Translate (0, 0, speed);
		} else if ((Vector3.Distance (player.position, transform.position) <= attackDistance) && found) {
			anim.ToAttacking ();
		} else if (!found) {
			anim.ToWalking ();
			walking ();
		} else if (found && player.transform.position.z < 17.0f) {
			GameObject cp = nearestPoint ();
			if (cp.CompareTag ("cp3")) {
				north = false;
				south = true;
				east = false;
				west = false;
			}
			anim.ToWalking ();
			direction = cp.transform.position - transform.position;
			direction.y = 0;
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), 0.5f);
			transform.Translate (0, 0, walkingSpeed);
			comeback = true;
		}


		var tmp = playerMan.GetComponent<PlayerMovementController> ();
		if (tmp.dmg == 3)
        {
				var script = playerMan.GetComponent<PlayerInteractionController> ();
				script.Die ();
				found = false;
				transform.position = initialPos;
				transform.rotation = initialRot;
			if (guard.CompareTag ("hwG1")) {
				north = true;
				south = false;
				east = false;
				west = false;
			} else if (guard.CompareTag ("hwG2")) {
				north = false;
				south = true;
				east = false;
				west = false;
			}
			var tmp2 = friend.GetComponent<GuardScript> ();
			if (!tmp2.found) {
				tmp.dmg = 0;
			}
        }
	}

	private void OnTriggerEnter(Collider other){
		if ((!found) || (comeback)) {
			comeback = false;
			found = false;
			if (other.CompareTag ("cp1")) {
				if (south) {
					transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
					south = false;
					east = true;
				} else if (west) {
					transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
					east = false;
					north = true;
				}

			} else if (other.CompareTag ("cp2")) {
				if (north) {
					transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
					north = false;
					east = true;
				} else if (west) {
					transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
					west = false;
					south = true;
				}
			} else if (other.CompareTag ("cp3")) {
				if (south) {
					transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
					north = false;
					south = false;
					east = false;
					west = true;
				} else if (east) {
					transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
					north = true;
					south = false;
					east = false;
					west = false;
				}
			} else if (other.CompareTag ("cp4")) {
				if (east) {
					transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
					north = false;
					south = true;
					east = false;
					west = false;
				} else if (north) {
					transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
					north = false;
					south = false;
					east = false;
					west = true;
				}
			}
		}
	}
		
	private void walking()
    {
		if (north) {
			position = transform.position;
			position += new Vector3(0, 0, walkingSpeed);
			transform.position = position;
		} else if (south) {
			position = transform.position;
			position += new Vector3(0, 0, -walkingSpeed);
			transform.position = position;
		}else if (east) {
			position = transform.position;
			position += new Vector3(walkingSpeed, 0, 0);
			transform.position = position;
		}else if (west) {
			position = transform.position;
			position += new Vector3(-walkingSpeed, 0, 0);
			transform.position = position;
		}
	}

	private GameObject nearestPoint(){
		GameObject[] array = new GameObject[4];
		array [0] = cp1;
		array [1] = cp2;
		array [2] = cp3;
		array [3] = cp4;
		GameObject result = null;
		float min = Vector3.Distance (transform.position, cp1.transform.position);
		for (int i = 1; i < array.Length; i++) {
			if (Vector3.Distance (transform.position, array [i].transform.position) < min) {
				min = Vector3.Distance (transform.position, array [i].transform.position);
				result = array [i];
			}
		}
		return result;
	}

	private void beat()
	{
		CameraShaker script = target.GetComponent<CameraShaker> ();
		script.hit ();
	}

	private void kill()
	{
		var tmp = playerMan.GetComponent<PlayerMovementController> ();
		tmp.dmg++;
	}

}