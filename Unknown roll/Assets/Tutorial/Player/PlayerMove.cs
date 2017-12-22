using UnityEngine;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour
{
	public GameObject bulletPrefab;

	public override void OnStartLocalPlayer()
	{
		GetComponent<MeshRenderer> ().material.color = Color.red;
	}
		
	[Command]
	void CmdFire()
	{
		//command può essere eseguito solo dal server

		//Creo il bullet dal prefab
		var bullet = (GameObject)Instantiate(
			bulletPrefab,
			transform.position - transform.forward,
			Quaternion.identity);

		//Lo faccio muovere
		bullet.GetComponent<Rigidbody>().velocity= -transform.forward*4;

		//spawn per i client
		NetworkServer.Spawn(bullet);

		//Despawn 2 sec
		Destroy(bullet, 2.0f);
	}
	void Update()
	{
		if (!isLocalPlayer)
			return;

		var x = Input.GetAxis("Horizontal")*0.1f;
		var z = Input.GetAxis("Vertical")*0.1f;
		transform.Translate(x, 0, z);	

		if (Input.GetMouseButton(0))
		{
			CmdFire ();
		}

	}
				
		
}
