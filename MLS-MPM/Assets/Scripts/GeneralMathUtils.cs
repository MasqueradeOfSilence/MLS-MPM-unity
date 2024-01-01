using Unity.Mathematics;
using UnityEngine;
using System;

public class GeneralMathUtils : MonoBehaviour
{
	public static bool DeepEquals(double2 a, double2 b)
	{
		double threshold = 0.0001;
		return math.abs(a[0] - b[0]) < threshold &&
			math.abs(a[1] - b[1]) < threshold;
	}

	public static bool DeepEquals(double2x2 a, double2x2 b)
	{
		double threshold = 0.0001;
		return math.abs(a[0][0] - b[0][0]) < threshold &&
			math.abs(a[0][1] - b[0][1]) < threshold &&
			math.abs(a[1][0] - b[1][0]) < threshold &&
			math.abs(a[1][1] - b[1][1]) < threshold;
	}

	public static bool DeepEquals(Vector3 a, Vector3 b)
    {
		double threshold = 0.0001;
		return math.abs(a[0] - b[0]) < threshold &&
			math.abs(a[1] - b[1]) < threshold &&
            math.abs(a[2] - b[2]) < threshold;
	}

	public static bool ApproximatelyEquals(double a, double b, double threshold = 0.01)
	{
		return math.abs(a - b) < threshold;
    }

	public static int[] ParticlePositionToCellPosition(double[] particlePosition)
    {
		return Array.ConvertAll(particlePosition, x => (int)x);
	}

	public static double[] ComputeDistanceFromParticleToCell(double[] particlePosition, int[] correspondingCellPosition)
    {
		double[] cellPosition = Array.ConvertAll<int, double>(correspondingCellPosition, x => x);
		double[] distance = { particlePosition[0] - cellPosition[0] - 0.5, particlePosition[1] - cellPosition[1] - 0.5 };
		return distance;
	}

	public static double[][] ComputeAllWeights(double[] distanceFromParticleToCell)
    {
		return P2G1Math.ComputeAllWeights(distanceFromParticleToCell);
    }

	public static double ComputeWeight(double[][] weights, int nx, int ny)
    {
		return weights[nx][0] * weights[ny][1];
	}

	public static int[] ComputeNeighborPosition(int[] cellPosition, int nx, int ny)
    {
		int x = cellPosition[0] + nx - 1;
		int y = cellPosition[1] + ny - 1;
		// TODO this is NOT a good overall fix
		int[] neighborPosition = { Math.Abs(x), Math.Abs(y) };
		return neighborPosition;
	}

	public static double[] ComputeDistanceFromCurrentParticleToCurrentNeighbor(int[] currentNeighborPosition, double[] particlePosition)
	{
		double x = (currentNeighborPosition[0] - particlePosition[0]) + 0.5;
		double y = (currentNeighborPosition[1] - particlePosition[1]) + 0.5;
		double[] distanceFromCurrentParticleToCurrentNeighbor = { x, y };
		return distanceFromCurrentParticleToCurrentNeighbor;
	}

	public static double[,] Format2x2MatrixForMath(double2x2 matrix)
    {
		return new double[,] { { matrix.c0.x, matrix.c0.y }, { matrix.c1.x, matrix.c1.y } };
    }

	public static double2x2 Format2x2MatrixForMath(double[,] matrix)
    {
		return new double2x2(matrix[0, 0], matrix[0, 1], matrix[1, 0], matrix[1, 1]);
    }

	public static double[] Format2DVectorForMath(double2 double2)
    {
		return new double[2] { double2.x, double2.y };
    }

	public static double2 Format2DVectorForMath(double[] doubleArray)
    {
		return new double2(doubleArray[0], doubleArray[1]);
    }

	public static int[] Format2DVectorForMath(int2 int2)
    {
		return new int[2] { int2.x, int2.y };
    }

	public static int Random1To10()
    {
		return UnityEngine.Random.Range(1, 11);
	}
}
