using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSurfacePolygonBuffer {

    private LinkedVertexList _linkedVertexList = new();

    private DiagonalEdgeGenerator _diagonalEdgeList;


    public void AddVertex(
        Plane localPlane,
        Vector3 towardPosition,
        Vector3 awayPosition
    ) {
        NonConvexMonotoneCutSurfaceVertex towardCutSurfaceVertex = new(localPlane, towardPosition);
        NonConvexMonotoneCutSurfaceVertex awayCutSurfaceVertex = new(localPlane, awayPosition);

        _linkedVertexList.Add(towardCutSurfaceVertex, awayCutSurfaceVertex);
    }

    public void MakeCutSurfacePolygon(bool addCutSurfaceMaterial) {
        _diagonalEdgeList = new(_linkedVertexList);
    }
}
