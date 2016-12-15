using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

	private Rigidbody rigidBody;
	private Vector3 deltaCamPos;

	private Button buttonMoveForward;
	private Button buttonMoveBack;
	private Button buttonRotateLeft;
	private Button buttonRotateRight;
	private Button buttonRotateTurretLeft;
	private Button buttonRotateTurretRight;
	private Button buttonShoot;

	private int simpleControl = 0;
	private float simpleControlDelay = 0;
	public float simpleControlMaxDelay;


	public GameObject body;
	public GameObject turret;

	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	public float maxShootDelay;
	private float currentShootDelay;

	void Start(){

		currentShootDelay = 0.0f;

		rigidBody = GetComponent<Rigidbody> ();

		deltaCamPos = Camera.main.transform.position - transform.position;

		buttonMoveForward = GameObject.Find("ButtonMoveForward").GetComponent<Button>();
		buttonMoveForward.onClick.AddListener(clickedForward);
		buttonMoveBack = GameObject.Find("ButtonMoveBack").GetComponent<Button>();
		buttonMoveBack.onClick.AddListener(clickedBack);
		buttonRotateRight = GameObject.Find("ButtonRotateRight").GetComponent<Button>();
		buttonRotateRight.onClick.AddListener(clickedRight);
		buttonRotateLeft = GameObject.Find("ButtonRotateLeft").GetComponent<Button>();
		buttonRotateLeft.onClick.AddListener(clickedLeft);

		buttonRotateTurretRight = GameObject.Find("ButtonRotateTurretRight").GetComponent<Button>();
		buttonRotateTurretRight.onClick.AddListener(clickedTurretRight);
		buttonRotateTurretLeft = GameObject.Find("ButtonRotateTurretLeft").GetComponent<Button>();
		buttonRotateTurretLeft.onClick.AddListener(clickedTurretLeft);

		buttonShoot = GameObject.Find("ButtonShoot").GetComponent<Button>();
		buttonShoot.onClick.AddListener(clickedShoot);

	}

	void clickedForward(){
		simpleControl = 1;
		simpleControlDelay = simpleControlMaxDelay;
	}
	void clickedBack(){
		simpleControl = 2;
		simpleControlDelay = simpleControlMaxDelay;
	}
	void clickedLeft(){
		simpleControl = 3;
		simpleControlDelay = simpleControlMaxDelay;
	}
	void clickedRight(){
		simpleControl = 4;
		simpleControlDelay = simpleControlMaxDelay;
	}
	void clickedTurretRight(){
		simpleControl = 5;
		simpleControlDelay = simpleControlMaxDelay;
	}
	void clickedTurretLeft(){
		simpleControl = 6;
		simpleControlDelay = simpleControlMaxDelay;
	}
	void clickedShoot(){
		simpleControl = 7;
		simpleControlDelay = simpleControlMaxDelay;
	}

	void Update(){

		if(!isLocalPlayer){
			return;
		}

		var turretRotation = 0.0f;

		if (Input.GetKey(KeyCode.RightArrow) || simpleControl == 5) {
			turretRotation = Time.deltaTime * 120.0f;
		} else if (Input.GetKey(KeyCode.LeftArrow) || simpleControl == 6) {
			turretRotation = Time.deltaTime * -120.0f;
		}

		if (Input.GetKey (KeyCode.W) || simpleControl == 1) {
			rigidBody.MovePosition (transform.position + transform.forward * Time.deltaTime * 4.0f );	// do przodu
		} else if (Input.GetKey (KeyCode.S) || simpleControl == 2) {
			rigidBody.MovePosition (transform.position + transform.forward * Time.deltaTime * -4.0f ); // do tyłu
		}

		var x = 0.0f;

		if (Input.GetKey (KeyCode.A) || simpleControl == 3) {
			x = -1.0f * Time.deltaTime * 40.0f;	// obrót w prawo
		} else if (Input.GetKey (KeyCode.D) || simpleControl == 4) {
			x = 1.0f * Time.deltaTime * 40.0f;	// obrót w lewo
		}

		turret.transform.Rotate (0, turretRotation, 0);	// obracaj wieżyczkę

		transform.Rotate(0, x, 0); // obracaj czołg

		// update shoot delay
		if (currentShootDelay > 0.0f) {
			currentShootDelay -= Time.deltaTime;
		} else {
			currentShootDelay = 0.0f;
		}

		// update simpleControl delay
		if (simpleControlDelay > 0.0f) {
			simpleControlDelay -= Time.deltaTime;
		} else {
			simpleControlDelay = 0.0f;
			simpleControl = 0;
		}

		if ((Input.GetKeyDown (KeyCode.Space) || simpleControl == 7) && currentShootDelay == 0.0f) {
			CmdFire();
			currentShootDelay = maxShootDelay;
		}

	}

	public override void OnStartLocalPlayer(){

		body.GetComponent<MeshRenderer>().material.color = Color.green;

	}

	void LateUpdate(){

		if(!isLocalPlayer){
			return;
		}
		
		Camera.main.transform.position = transform.position + deltaCamPos;	// podążaj kamerą za graczem

	}

	[Command]
	void CmdFire(){

		var bullet = (GameObject)Instantiate (
			             bulletPrefab,
			             bulletSpawn.position,
			             bulletSpawn.rotation);

		bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * 6;

		NetworkServer.Spawn (bullet);	// utwórz obiekt na klientach

		// Zniszcz pocisk po dwóch sekundach
		Destroy(bullet, 4.0f);

	}

}