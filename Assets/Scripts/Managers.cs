﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerManager))]       // Гарантируем существование различных диспетчеров
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(WeatherManager))]
[RequireComponent(typeof(ImagesManager))]
public class Managers : MonoBehaviour {

    // Статические свойства, которыми остальнйо код пользуется для доступа к диспетчерам
    public static PlayerManager Player { get; private set; }
    public static InventoryManager Inventory { get; private set; }
    public static WeatherManager Weather { get; private set; }
    public static ImagesManager Images { get; private set; }

    private List<IGameManager> _startSequence; // Список диспетчеров, который просматривается в цикле во время стартовой последовательности

    void Awake()
    {
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        Weather = GetComponent<WeatherManager>();
        Images = GetComponent<ImagesManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(Inventory);
        _startSequence.Add(Weather);
        _startSequence.Add(Images);

        StartCoroutine(StartupManagers()); // Асирнхронно загружаем статовую последовательность
    }
        
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator StartupManagers()
    {
        NetworkService network = new NetworkService();

        foreach (IGameManager manager in _startSequence)
        {
            manager.Startup(network);
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules) // Пока не заработали все диспетчеры
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence)
            {
                if(manager.status == ManagerStatus.Started)
                {
                    numReady++;
                }
            }

            if(numReady > lastReady)
            {
                Debug.Log("Progress: " + numReady + "/" + numModules);
            }

            yield return null; // Остановка на один кадр перед следующей проверкой
        }

        Debug.Log("All managers started up");
    }
}
