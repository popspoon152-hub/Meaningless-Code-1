using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileManager : MonoBehaviour
{
    public List<Transform> GroundTiles = new List<Transform>();  // �洢�����

    public void AddGroundTile(Transform tile)
    {
        if (!GroundTiles.Contains(tile))
        {
            GroundTiles.Add(tile);
        }
    }

    public void RemoveGroundTile(Transform tile)
    {
        if (GroundTiles.Contains(tile))
        {
            GroundTiles.Remove(tile);
        }
    }

    // ��ȡ����ؿ�
    public Transform GetRandomGroundTile()
    {
        if (GroundTiles.Count == 0)
            return null;

        int index = Random.Range(0, GroundTiles.Count);
        return GroundTiles[index];
    }
}
