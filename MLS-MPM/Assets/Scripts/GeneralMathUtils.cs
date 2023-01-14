using Unity.Mathematics;
using UnityEngine;

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
}
