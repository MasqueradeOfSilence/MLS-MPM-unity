using NUnit.Framework;
using Unity.Mathematics;

[TestFixture]
public class G2PMathTest
{
	[Test]
	public void ComputeWeightedVelocityShouldCorrectlyCalculateTheWeightedVelocity()
	{
		double[] neighborCellVelocity = { 0, 1 };
		double weight = 0.16;
		double[] expectedWeightedVelocity = { 0, 0.16 };
		double[] actualWeightedVelocity = G2PMath.ComputeWeightedVelocity(neighborCellVelocity, weight);
		Assert.That(actualWeightedVelocity, Is.EqualTo(expectedWeightedVelocity).Within(0.01));
	}

	[Test]
	public void ComputeTermShouldCorrectlyComputeTheIntermediaryTerm()
	{
		// Term should be 2x2! 
		double[] weightedVelocity = { 0, 0.16 };
		double[] distanceFromCellToNeighbor = { -0.7, 0.4 };
		double[,] expectedTerm = { { 0, -0.112 }, { 0, 0.064 } };
		double[,] actualTerm = G2PMath.ComputeTerm(weightedVelocity, distanceFromCellToNeighbor);
		Assert.That(actualTerm, Is.EqualTo(expectedTerm).Within(0.01));
	}

	[Test]
	public void UpdateBShouldCorrectlyUpdateB()
	{
		double2x2 initialB = new(0, 0, 0, 0);
		double2x2 term = new(0, -0.112, 0, 0.064);
		double2x2 expectedUpdatedB = new(0, -0.112, 0, 0.064);
		double2x2 actualUpdatedB = G2PMath.ComputeUpdatedB(initialB, term);
		Assert.IsTrue(GeneralMathUtils.DeepEquals(actualUpdatedB, expectedUpdatedB));
	}

	[Test]
	public void UpdateParticleVelocityShouldReturnACorrectlyUpdatedParticleVelocity()
	{
		double[] initialParticleVelocity = { 2, 2 };
		double[] weightedVelocity = { 0, 0.16 };
		double[] expectedUpdatedParticleVelocity = { 2, 2.16 };
		double[] actualUpdatedParticleVelocity = G2PMath.ComputeUpdatedParticleVelocity(initialParticleVelocity, weightedVelocity);
		Assert.That(actualUpdatedParticleVelocity, Is.EqualTo(expectedUpdatedParticleVelocity).Within(0.01));
	}

	[Test]
	public void RecomputeCMatrixShouldReturnAnUpdatedCMatrix()
	{
		double2x2 B = new(0, -0.112 , 0, 0.064);
		double2x2 expectedUpdatedC = new(0, -0.448, 0, 0.256);
		double2x2 actualUpdatedC = G2PMath.RecomputeCMatrix(B);
		Assert.IsTrue(GeneralMathUtils.DeepEquals(actualUpdatedC, expectedUpdatedC));
	}

	[Test]
	public void ParticleAdvectionShouldCorrectlyCalculateTheNextPositionForTheParticle()
	{
		double[] initialParticlePosition = { 3.2, 6 };
		double[] particleVelocity = { 2, 2.16 };
		double dt = 0.2;
		double[] expectedParticlePositionAfterAdvection = { 3.6, 6.432 };
		double[] actualParticlePositionAfterAdvection = G2PMath.AdvectParticle(initialParticlePosition, particleVelocity, dt);
		Assert.That(actualParticlePositionAfterAdvection, Is.EqualTo(expectedParticlePositionAfterAdvection).Within(0.01));
	}
}
