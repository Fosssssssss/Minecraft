using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int seed;
    public BiomeAttributes biome;
    public Transform player;
    public Vector3 spawnPosition;

    public Material material;
    public BlockType[] blocktypes;

    chunk[,] chunks = new chunk[voxel.WorldSizeChunks, voxel.WorldSizeChunks];

    List<chunkCoord> activeChunks = new List<chunkCoord>();
    chunkCoord playerChunkCoord;
    chunkCoord playerLastChunkCoord;

    List<chunkCoord> chunksToCreate = new List<chunkCoord>();
    private bool isCreatingChunks;

    private void Start() {

        Random.InitState(seed);
        Application.targetFrameRate = 120;
        spawnPosition = new Vector3((voxel.WorldSizeChunks * voxel.ChunkWidth) / 2f, voxel.ChunkHeight -30f, (voxel.WorldSizeChunks * voxel.ChunkWidth) / 2f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
       
    }

    private void Update() {

        playerChunkCoord = GetChunkCoordFromVector3(player.position);

       if (!playerChunkCoord.Equals(playerLastChunkCoord))
       CheckViewDistance();

        if (chunksToCreate.Count > 0 && !isCreatingChunks)
            StartCoroutine("createChunks");

    }



    void GenerateWorld() {
        for (int x = (voxel.WorldSizeChunks / 2) - voxel.ViewDistanceInChunks; x < (voxel.WorldSizeChunks / 2) + voxel.ViewDistanceInChunks; x++)
        {
            for (int z = (voxel.WorldSizeChunks / 2) - voxel.ViewDistanceInChunks; z < (voxel.WorldSizeChunks / 2) + voxel.ViewDistanceInChunks; z++)
            {


                chunks[x, z] = new chunk(new chunkCoord(x, z), this, true);
                activeChunks.Add(new chunkCoord(x,z));


               }
}
        player.position = spawnPosition;

    }

    IEnumerator createChunks() { 
    
        isCreatingChunks = true;

        while (chunksToCreate.Count > 0) {

            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
           
            chunksToCreate.RemoveAt(0);
            yield return null;
        
        }



        isCreatingChunks = false;
    
    }




    chunkCoord GetChunkCoordFromVector3(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x / voxel.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / voxel.ChunkWidth);
       
        return new chunkCoord(x, z);
    }

    public chunk getChunkFromVector3(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x / voxel.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / voxel.ChunkWidth);

        return chunks[x,z];

    }

    void CheckViewDistance()
    {

        chunkCoord coord = GetChunkCoordFromVector3(player.position);
        playerLastChunkCoord = playerChunkCoord;
        List<chunkCoord> previouslyActChunks = new List<chunkCoord>(activeChunks);

        for (int x = coord.x - voxel.ViewDistanceInChunks; x < coord.x + voxel.ViewDistanceInChunks; x++) {
            for (int z = coord.z - voxel.ViewDistanceInChunks; z < coord.z + voxel.ViewDistanceInChunks; z++) {
               
                if (isChunkInWorld(new chunkCoord(x,z))) {

                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new chunk(new chunkCoord(x, z), this, false);
                        chunksToCreate.Add(new chunkCoord(x, z));
                    }
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(new chunkCoord(x, z));
                    }
                }

                for (int i = 0; i < previouslyActChunks.Count; i++) {

                    if (previouslyActChunks[i].Equals(new chunkCoord(x, z)))
                        previouslyActChunks.RemoveAt(i);
                
                }

            }

        }


        foreach (chunkCoord c in previouslyActChunks)
            chunks[c.x, c.z].isActive = false;


    }

    public bool CheckForVoxel(Vector3 pos) {

        chunkCoord thisChunk = new chunkCoord(pos);

        if (!isChunkInWorld(thisChunk) || pos.y < 0 || pos.y > voxel.ChunkHeight) 
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return blocktypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;

        return blocktypes[GetVoxel(pos)].isSolid;


    }


    public byte GetVoxel(Vector3 pos) {

        int yPos = Mathf.FloorToInt(pos.y);

        // <--! NIE RUSZAÆ -->
        //powietrze
        if (!IsVoxelInWorld(pos))
            return 0;
        //bedrok
        if (yPos == 0)
            return 1;

        //basic

        int terrainHeight = Mathf.FloorToInt((biome.terrainHeight) * noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;

        byte voxelValue = 0;
      
        if (yPos == terrainHeight)
            voxelValue = 4;
        else if (yPos < terrainHeight && yPos > terrainHeight -4)
            voxelValue = 3;
        else if (yPos > terrainHeight)
            voxelValue = 0;
        
        else
            voxelValue = 2;

        //<<<<<<<<<<<<<<

        if (voxelValue == 2)
        {
            foreach (Lode lode in biome.lodes) {

                if (yPos > lode.minHeight && yPos < lode.maxHeight) {

                    if (noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;
                }

            }
        }
        return voxelValue;

    }



    bool isChunkInWorld (chunkCoord coord) {

        if (coord.x > 0 && coord.x < voxel.WorldSizeChunks - 1 && coord.z > 0 && coord.z < voxel.WorldSizeChunks - 1) 
            return true;
        else
            return false;


    }

    bool IsVoxelInWorld(Vector3 pos) {
        if (pos.x >= 0 && pos.x < voxel.WorldSizeInVoxels && pos.y >= 0 && pos.y < voxel.ChunkHeight && pos.z >= 0 && pos.z < voxel.WorldSizeInVoxels)
            return true;
        else
            return false;
    }

}

[System.Serializable]
public class BlockType {
    public string blockName;
    public bool isSolid;
    //back front top bottom left right

  

    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;


    public int GetTextureID(int faceIndex) {
        switch (faceIndex) {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                return 0;
            
        }
    }
}