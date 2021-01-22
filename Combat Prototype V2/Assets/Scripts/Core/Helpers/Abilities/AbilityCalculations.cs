using UnityEngine;
using System;

namespace Wayfinder {
  public class AbilityCalculations {
    public static float TriangleArea(Vector2 point1, Vector2 point2, Vector2 point3) {
      return Mathf.Abs((point1.x * (point2.y - point3.y) +
                        point2.x * (point3.y - point1.y) +
                        point3.x * (point1.y - point2.y)) / 2f);
    }

    public static bool VectorInsideTriangle(Vector2 checkingPoint, Vector2 point1, Vector2 point2, Vector2 point3) {
      float areaABC = TriangleArea(point1, point2, point3);
      float areaPBC = TriangleArea(checkingPoint, point2, point3);
      float areaPAC = TriangleArea(point1, checkingPoint, point3);
      float areaPAB = TriangleArea(point1, point2, checkingPoint);

      return Mathf.Approximately(areaABC, areaPBC + areaPAC + areaPAB);
    }
  }
}