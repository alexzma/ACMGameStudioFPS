using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
namespace UnityEngine.Networking
{

    public class NetworkScript : MonoBehaviour
    {
        public NetworkManager manager;
        public Text text;

        // Start is called before the first frame update
        void Start()
        {
            manager = GameObject.Find("Network Manager2").GetComponent<NetworkManager>();
            //manager = GetComponent<NetworkManager>();
        }

        void Awake()
        { }

        // Update is called once per frame
        void Update()
        { }

        public void StartServer()
        {
            manager.StartServer();
        }

        public void StartHost()
		{
            manager.StartHost();
		}

        public void StartClient()
        {
            manager.StartClient();
        }

        public void SetAddress()
		{
            manager.networkAddress = text.text;
		}
    }
}