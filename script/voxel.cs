using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class voxel {
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 128;
    public static readonly int WorldSizeChunks = 100;
    public static readonly int TextureAtlasSizeInBlock = 4;
    public static int WorldSizeInVoxels {
        get { return WorldSizeChunks * ChunkWidth; }
    }

    public static float NormalizedTextureSize {
        get { return 1f / (float)TextureAtlasSizeInBlock; }
    }


    public static readonly int ViewDistanceInChunks = 5;



    public static readonly Vector3[] voxelVerts = new Vector3[8] {
    new Vector3(0.0f, 0.0f, 0.0f), // index 0
    new Vector3(1.0f, 0.0f, 0.0f), // index 1
    new Vector3(1.0f, 1.0f, 0.0f), // index 2
    new Vector3(0.0f, 1.0f, 0.0f), // index 3
    new Vector3(0.0f, 0.0f, 1.0f), // index 4
    new Vector3(1.0f, 0.0f, 1.0f), // index 5
    new Vector3(1.0f, 1.0f, 1.0f), // index 6 
    new Vector3(0.0f, 1.0f, 1.0f), // index 7
       

    };
    public static readonly Vector3[] faceChecks = new Vector3[6] {
      new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),

        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),


        
        

        
    };


    public static readonly int[,] voxelTris = new int[6, 4] {

    {0, 3, 1, 2}, //back
    {5, 6 ,4, 7}, //front
    {3, 7, 2, 6}, //top
    {1, 5, 0, 4}, //bottom
    {4, 7 ,0, 3}, //left
    {1, 2, 5, 6}, //right
    

    };

    public static readonly Vector2[] voxelUvs = new Vector2[6] {
        
        

        
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        
        
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
    };


}


/*
 031,132 front
 470,073 left
 564,467 back
 125,526 right
 372,276 top
 041,145 bottom
 */