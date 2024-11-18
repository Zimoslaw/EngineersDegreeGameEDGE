using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class LabyrinthGenerator : MonoBehaviour
{
    private Cell[,] cells;

    public int labyrinthSize = 5;

    [SerializeField]
    private GameObject startCellPrefab;
    [SerializeField]
    private GameObject exitCellPrefab;
	[SerializeField]
	private GameObject extraExitCellPrefab;

	[SerializeField]
    private GameObject exitTrigger;
	[SerializeField]
	private GameObject extraExitTrigger;

	[SerializeField]
    private GameObject[] cellsPrefabs;

    [SerializeField]
    private GameObject scaringSoundPrefab;

    [SerializeField]
    private AudioClip[] scaringSounds;

    public (int z, int x) startCell;
    public (int z, int x) exitCell;
	public (int z, int x) extraExitCell;

	[SerializeField]
    private GameObject demon;

    NavMeshSurface navMesh;

    void Awake()
    {
        navMesh = GetComponent<NavMeshSurface>();

        GenerateMazeArray();

        InstantiateCells();

        if (!navMesh.IsUnityNull())
        {
            NavMeshBaking();
        }

        InstantiateScaringSounds(4);
    }

    /// <summary>
    /// Generates maze in form of cells array. Randomized depth-first search algorythm.
    /// </summary>
    void GenerateMazeArray()
    {
        // Get the seed if it's saved from previous run
        int seed = PlayerPrefs.GetInt("LabyrinthSeed");

        if (seed == -1) // -1 = not saved
        {
            seed = UnityEngine.Random.Range(0, 99999);
            PlayerPrefs.SetInt("LabyrinthSeed", seed);
        }

        UnityEngine.Random.InitState(seed);

        cells = new Cell[labyrinthSize, labyrinthSize];
        for (int z = 0; z < labyrinthSize; z++)
        {
            for (int x = 0; x < labyrinthSize; x++)
            {
                cells[z, x] = new Cell();
            }
        }

        var _coordStack = new List<(int z, int x)>(); // Cells stack

        // Select random cell to start with and add it to the stack
        startCell.z = UnityEngine.Random.Range(1, labyrinthSize - 1);
        startCell.x = UnityEngine.Random.Range(1, labyrinthSize - 1);
        cells[startCell.z, startCell.x].visited = true;
        _coordStack.Add((startCell.z, startCell.x));

        // Calculate coords of exit cell
        exitCell.z = labyrinthSize - 1 - startCell.z;
        exitCell.x = labyrinthSize - 1 - startCell.x;
        // Move exit trigger to exit cell
        exitTrigger.transform.position = new Vector3(exitCell.x * 4, 0, exitCell.z * 4);

        // Calculate coords od extra exit cell
        do
        {
            extraExitCell.z = UnityEngine.Random.Range(1, labyrinthSize - 1);
        } while(extraExitCell.z == startCell.z || extraExitCell.z == exitCell.z);
		do
		{
			extraExitCell.x = UnityEngine.Random.Range(1, labyrinthSize - 1);
		} while(extraExitCell.x == startCell.x || extraExitCell.x == exitCell.x);
		// Move exit trigger to exit cell
		extraExitTrigger.transform.position = new Vector3(extraExitCell.x * 4, 0, extraExitCell.z * 4);

		while (_coordStack.Count != 0)
        {
            // Pop last cell from stack
            var current = _coordStack.Last();
            _coordStack.Remove(current);

            // If cell has unvisited neighbour
            byte neighbour = GetRandomUnvisitedNeighbour(current.z, current.x);
            if (neighbour != 255)
            {
                // Add current cell to the stack
                _coordStack.Add(current);

                // Calculating neighbour cell position
                (int z, int x) neighbourCoord = (current.z - 1, current.x);
                switch (neighbour)
                {
                    case 1:
                        neighbourCoord = (current.z, current.x - 1);
                        break;
                    case 2:
                        neighbourCoord = (current.z + 1, current.x);
                        break;
                    case 3:
                        neighbourCoord = (current.z, current.x + 1);
                        break;
                }

                // Removing walls
                cells[current.z, current.x].walls.Remove(neighbour);
                byte neighbourWallToRemove = neighbour += 2;
                if (neighbourWallToRemove == 4)
                    neighbourWallToRemove = 0;
                if (neighbourWallToRemove == 5)
                    neighbourWallToRemove = 1;
                cells[neighbourCoord.z, neighbourCoord.x].walls.Remove(neighbourWallToRemove);

                // Add chosen neighbour cell to the stack
                cells[neighbourCoord.z, neighbourCoord.x].visited = true;
                _coordStack.Add(neighbourCoord);
            }
        }

        // Deleting starting cell walls for harder start
        cells[startCell.z + 1, startCell.x].walls.Remove(0);
        cells[startCell.z, startCell.x + 1].walls.Remove(1);
        cells[startCell.z - 1, startCell.x].walls.Remove(2);
        cells[startCell.z, startCell.x - 1].walls.Remove(3);
    }

    /// <summary>
    /// Instantiates cell prefabs based on cells array
    /// </summary>
    void InstantiateCells()
    {
        for (int z = 0; z < labyrinthSize; z++)
        {
            for (int x = 0; x < labyrinthSize; x++)
            {
                Cell cell = cells[z, x];

                int indexMultiplier = UnityEngine.Random.Range(0, 4);

                //Starting cell
                if (startCell.z == z && startCell.x == x)
                {
                    Instantiate(startCellPrefab, new Vector3(x * 4, 0, z * 4), Quaternion.identity, gameObject.transform);
                    continue;
                }

                //Exiting cell
                if (exitCell.z == z && exitCell.x == x)
                {
                    Instantiate(exitCellPrefab, new Vector3(x * 4, 0, z * 4), Quaternion.identity, gameObject.transform);
                    continue;
                }

				//Extra exiting cell
				if(extraExitCell.z == z && extraExitCell.x == x)
				{
					Instantiate(extraExitCellPrefab, new Vector3(x * 4, 0, z * 4), Quaternion.identity, gameObject.transform);
					continue;
				}

				// Junction 4-way
				if (cell.walls.Count == 0) 
                {
                    Instantiate(cellsPrefabs[indexMultiplier * 5 + 4], new Vector3(x * 4, 0, z * 4), Quaternion.identity, gameObject.transform);
                    continue;
                }

                // Junction 3-way
                if (cell.walls.Count == 1)
                {
                    Quaternion rotation = Quaternion.identity;

                    switch (cell.walls.First())
                    {
                        case 0:
                            rotation = Quaternion.AngleAxis(180, new Vector3(0, 1, 0));
                            break;
                        case 1:
                            rotation = Quaternion.AngleAxis(270, new Vector3(0, 1, 0));
                            break;
                        case 3:
                            rotation = Quaternion.AngleAxis(90, new Vector3(0, 1, 0));
                            break;
                    }
                    

                    Instantiate(cellsPrefabs[indexMultiplier * 5 + 3], new Vector3(x * 4, 0, z * 4), rotation, gameObject.transform);
                    continue;
                }

                // Dead-end
                if (cell.walls.Count == 3) 
                {
                    Quaternion rotation = Quaternion.identity;

                    int sum = 0;
                    foreach (byte wall in cell.walls)
                        sum += wall;

                    switch(sum)
                    {
                        case 5:
                            rotation = Quaternion.AngleAxis(90, new Vector3(0, 1, 0));
                            break;
                        case 4:
                            rotation = Quaternion.AngleAxis(180, new Vector3(0, 1, 0));
                            break;
                        case 3:
                            rotation = Quaternion.AngleAxis(270, new Vector3(0, 1, 0));
                            break;
                    }

                    Instantiate(cellsPrefabs[indexMultiplier * 5], new Vector3(x * 4, 0, z * 4), rotation, gameObject.transform);
                    continue;
                }

                // Straight
                if (cell.walls.Contains(1) && cell.walls.Contains(3))
                {
                    Instantiate(cellsPrefabs[indexMultiplier * 5 + 1], new Vector3(x * 4, 0, z * 4), Quaternion.identity, gameObject.transform);
                    continue;
                }

                if (cell.walls.Contains(0) && cell.walls.Contains(2))
                {
                    Instantiate(cellsPrefabs[indexMultiplier * 5 + 1], new Vector3(x * 4, 0, z * 4), Quaternion.AngleAxis(90, new Vector3(0, 1, 0)), gameObject.transform);
                    continue;
                }

                // Turn
                if (cell.walls.Contains(0) && cell.walls.Contains(3))
                {
                    Instantiate(cellsPrefabs[indexMultiplier * 5 + 2], new Vector3(x * 4, 0, z * 4), Quaternion.AngleAxis(180, new Vector3(0, 1, 0)), gameObject.transform);
                    continue;
                }
                if (cell.walls.Contains(0) && cell.walls.Contains(1))
                {
                    Instantiate(cellsPrefabs[indexMultiplier * 5 + 2], new Vector3(x * 4, 0, z * 4), Quaternion.AngleAxis(270, new Vector3(0, 1, 0)), gameObject.transform);
                    continue;
                }
                if (cell.walls.Contains(1) && cell.walls.Contains(2))
                {
                    Instantiate(cellsPrefabs[indexMultiplier * 5 + 2], new Vector3(x * 4, 0, z * 4), Quaternion.identity, gameObject.transform);
                    continue;
                }
                if (cell.walls.Contains(2) && cell.walls.Contains(3))
                {
                    Instantiate(cellsPrefabs[indexMultiplier * 5 + 2], new Vector3(x * 4, 0, z * 4), Quaternion.AngleAxis(90, new Vector3(0, 1, 0)), gameObject.transform);
                    continue;
                }
            } 
        }
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
    }

    /// <summary>
    /// Instantiates scaring sounds in random cells.
    /// </summary>
    /// <param name="amount">Number of triggers to instantiate</param>
    private void InstantiateScaringSounds(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int soundIndex = UnityEngine.Random.Range(0, scaringSounds.Length);
            int x = UnityEngine.Random.Range(0, labyrinthSize);
            int z = UnityEngine.Random.Range(0, labyrinthSize);
            var copy = Instantiate(scaringSoundPrefab, new Vector3(x * 4, 1, z * 4), Quaternion.identity, gameObject.transform);
            copy.GetComponent<AudioSource>().clip = scaringSounds[soundIndex];
        }
    }

    /// <summary>
    /// Returns random, univisited neigbour for next maze generation step.
    /// </summary>
    /// <param name="z">Z index of cells array</param>
    /// <param name="x">X index of cells array</param>
    /// <returns>Direction of random, unvisited neighbour. 255 if there's not a single unvisited neighbour</returns>
    byte GetRandomUnvisitedNeighbour(int z, int x)
    {
        List<byte> neighbours = new();

        if (z - 1 >= 0 && !cells[z - 1, x].visited)
            neighbours.Add(0);
        if (x - 1 >= 0 && !cells[z, x - 1].visited)
            neighbours.Add(1);
        if (z + 1 < labyrinthSize && !cells[z + 1, x].visited)
            neighbours.Add(2);
        if (x + 1 < labyrinthSize && !cells[z, x + 1].visited)
            neighbours.Add(3);

        if (neighbours.Count == 0)
            return 255;

        byte chosen = neighbours.ElementAt((byte)UnityEngine.Random.Range(0, neighbours.Count));

        return chosen;
    }

    /// <summary>
    /// Bakes navigation mesh on labyrinth
    /// </summary>
    void NavMeshBaking()
    {
        navMesh.BuildNavMesh();
    }

    /// <summary>
    /// Instantiates demon's prefab a set its destination to starting cell
    /// </summary>
    public void InstantiateDemon()
    {
        Instantiate(demon, new Vector3(exitCell.x * 4 + 1, 0.1f, exitCell.z * 4 + 1), Quaternion.identity, gameObject.transform);

        demon.GetComponent<DemonAi>().CurrentDestination = new Vector3(startCell.x * 4, 0.1f, startCell.z * 4);
    }
}
