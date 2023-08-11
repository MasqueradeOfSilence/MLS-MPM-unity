using Unity.Mathematics;
using UnityEngine;

public class VolumeFractionCalculator : MonoBehaviour
{
    public static bool IsParticleInsideCell(Particle particle, int2 gridCellPosition)
    {
        int2 castedParticlePosition = new(particle.GetPosition());
        return GeneralMathUtils.DeepEquals(gridCellPosition, castedParticlePosition);
    }
}
