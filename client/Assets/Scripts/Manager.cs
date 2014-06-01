using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using SimpleJSON;

public class Manager : MonoBehaviour {

    private String server = "localhost";
    private int port = 3000;
    private NetworkStream stream;
    private bool received = true;

    public GameObject networkEntityPrefab;
    public GameObject packetPrefab;
    public GameObject particleTrailPrefab;

    // By how much to scale up geo coordinates
    // in the game space.
    public float distanceScale = 5.0f;

	// Use this for initialization
	void Start () {
        print("starting...");

        TcpClient client = new TcpClient();
        client.Connect(server, port);

        stream = client.GetStream();
        stream.ReadTimeout = 10;
	}

	// Update is called once per frame
	void Update () {
        if (received == true) {
            received = false;
            byte[] b = {0x20};
            stream.Write(b, 0, b.Length);
            receiveData();
        }
	}

    void receiveData() {
        String r = readTcpStream();
        if (r != "") {
            // If this is not an empty array...
            if (r != "[]") {

                // Locate or create the Network Entities.
                // We create a random y-value to add some 3D dynamism to the entity positioning.
                foreach(JSONNode N in JSON.Parse(r).AsArray) {
                    // From
                    GameObject fromEntity = GameObject.Find(N["from"]["ip"]);
                    if (!fromEntity) {
                        fromEntity = (GameObject)Instantiate(networkEntityPrefab, new Vector3(processPoint(N["from"]["x"].AsFloat), UnityEngine.Random.Range(-100f, 100f), processPoint(N["from"]["y"].AsFloat)), Quaternion.identity);

                        fromEntity.name = N["from"]["ip"];
                    }
                    NetworkEntityScript fromEntityScript = fromEntity.GetComponent<NetworkEntityScript>();
                    fromEntityScript.ping();

                    // To
                    GameObject toEntity = GameObject.Find(N["to"]["ip"]);
                    if (!toEntity) {
                        toEntity = (GameObject)Instantiate(networkEntityPrefab, new Vector3(processPoint(N["to"]["x"].AsFloat), UnityEngine.Random.Range(-100f, 100f), processPoint(N["to"]["y"].AsFloat)), Quaternion.identity);

                        toEntity.name = N["to"]["ip"];
                    }
                    NetworkEntityScript toEntityScript = toEntity.GetComponent<NetworkEntityScript>();
                    toEntityScript.ping();

                    // Send a packet between the entities.
                    GameObject packet = (GameObject)Instantiate(packetPrefab, fromEntity.transform.position, Quaternion.identity);
                    GameObject packetTrail = (GameObject)Instantiate(particleTrailPrefab, fromEntity.transform.position, Quaternion.identity);
                    packetTrail.transform.parent = packet.transform;
                    StartCoroutine(MovePacket(packet, fromEntity.transform.position, toEntity.transform.position, 0.2f));
                }
            }
        }
        received = true;
    }

    // Apply some modifications to the point as needed.
    float processPoint(float point) {
        return point * distanceScale * UnityEngine.Random.Range(-4f, 4f);
    }

    IEnumerator MovePacket(GameObject packet, Vector3 startPos, Vector3 endPos, float time) {
        float i = 0.0f;
        float rate = 1.0f/time;
        while (i < 1.0f) {
            i += Time.deltaTime * rate;
            packet.transform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }
        Destroy(packet);
    }

    String readTcpStream() {
        String data = "";
        try {
            if (stream.DataAvailable) {
                int b;
                while ((b = stream.ReadByte()) != 0) {
                    data += (char)b;
                }
            }
        } catch (Exception e) {
            Debug.Log("Exception: " + e);
        }
        return data;
    }
}
