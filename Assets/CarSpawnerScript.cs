using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnerScript : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCar());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnCar()
    {
        while (true)
        {
            float spawnTime = Random.Range(2, 10);
            yield return new WaitForSeconds(spawnTime);

            int randomIndex = Random.Range(0, carPrefabs.Length);
            GameObject newCar = Instantiate(carPrefabs[randomIndex], transform.position, Quaternion.identity);

            Rigidbody rb = newCar.GetComponent<Rigidbody>();

            newCar.transform.rotation = Quaternion.LookRotation(this.transform.forward, Vector3.up);
            rb.AddForce(new Vector3(this.transform.forward.x * speed, 0, 0), ForceMode.Impulse);

            Destroy(newCar, 3);
        }
    }
}
