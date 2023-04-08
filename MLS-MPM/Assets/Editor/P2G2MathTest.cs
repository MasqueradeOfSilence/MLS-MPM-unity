using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

[TestFixture]
public class P2G2MathTest
{
	[Test]
	public void FindNearestGridCellToParticleShouldDetermineTheCorrectGridCell()
	{
		double[] particlePosition = { 0.76, 1.61 };
		int[] expectedGridCell = { 1, 2 };
		int[] actualGridCell = P2G2Math.FindNearestGridCellToParticle(particlePosition);
		Assert.That(expectedGridCell, Is.EqualTo(actualGridCell).Within(0.01));
	}

	[Test]
	public void UpdateDensityShouldReturnACorrectlyUpdatedDensityValue()
	{
		// Not to be confused with particle mass!
		double gridCellMass = 3;
		double weight = 1.5376;
		double initialDensity = 0;
		double expectedUpdatedDensity = 4.6128;
		double actualUpdatedDensity = P2G2Math.ComputeUpdatedDensity(weight, gridCellMass, initialDensity);
		Assert.That(actualUpdatedDensity, Is.EqualTo(expectedUpdatedDensity).Within(0.01));
	}

	[Test]
	public void ComputeVolumeShouldCorrectlyComputeVolume()
	{
		double particleMass = 2;
		double density = 4.6128;
		double expectedVolume = 0.433576136;
		double actualVolume = P2G2Math.ComputeVolume(particleMass, density);
		Assert.That(actualVolume, Is.EqualTo(expectedVolume).Within(0.01));
	}

	[Test]
	public void ComputePressureShouldCorrectlyCalculatePressure()
	{
		double expectedPressure = 7.6855;
		double eosStiffness = 10;
		// updated density from previous step
		double density = 4.6128;
		double restDensity = 4;
		double eosPower = 4;
		double actualPressure = P2G2Math.ComputePressure(eosStiffness, density, restDensity, eosPower);
		Assert.That(actualPressure, Is.EqualTo(expectedPressure).Within(0.01));
	}

	[Test]
	public void CreateStressMatrixShouldCorrectlyBuildStressMatrix()
	{
		double pressure = 7.6855;
		double2x2 expectedStress = new(-pressure, 0, 0, -pressure);
		double2x2 actualStress = P2G2Math.CreateStressMatrix(pressure);
		Assert.That(actualStress, Is.EqualTo(expectedStress).Within(0.01));
	}

	[Test]
	public void InitializeStrainMatrixShouldCorrectlyBuildStrainMatrix()
	{
		// This C value will come from the corresponding particle
		double2x2 C = new(2, 3, 4, 1);
		double2x2 expectedStrain = new(2, 3, 4, 1);
		double2x2 actualStrain = P2G2Math.InitializeStrainMatrix(C);
		// equality is checking for exact pointers, but is OK in this case
		Assert.That(actualStrain, Is.EqualTo(expectedStrain));
	}

	[Test]
	public void ComputeTraceShouldCorrectlyCalculateTraceValue()
	{
		double[,] initialStrain = { { 2, 3 }, { 4, 1 } };
		double expectedTrace = 7;
		double actualTrace = P2G2Math.ComputeTrace(initialStrain);
		Assert.That(actualTrace, Is.EqualTo(expectedTrace));
	}

	[Test]
	public void UpdateStrainShouldReturnAnUpdatedStrainValue()
	{
		double[,] expectedUpdatedStrain = { { 2, 7 }, { 7, 1 } };
		double[,] initialStrain = { { 2, 3 }, { 4, 1 } };
		double trace = 7;
		double[,] actualUpdatedStrain = P2G2Math.UpdateStrain(initialStrain, trace);
		Assert.That(actualUpdatedStrain, Is.EqualTo(expectedUpdatedStrain));
	}

	[Test]
	public void ComputeViscosityShouldCorrectlyCalculateViscosity()
	{
		double dynamicViscosity = 0.1;
		double2x2 strain = new(2, 7, 7, 1);
		double2x2 expectedViscosity = new(0.2, 0.7, 0.7, 0.1);
		double2x2 actualViscosity = P2G2Math.ComputeViscosity(strain, dynamicViscosity);
		// Test for deep equality -- default equals checks pointers
		Assert.IsTrue(GeneralMathUtils.DeepEquals(actualViscosity, expectedViscosity));
	}

	[Test]
	public void UpdateStressShouldReturnAnUpdatedStressValue()
	{
		double2x2 viscosity = new(0.2, 0.7, 0.7, 0.1);
		double2x2 initialStress = new(-7.6855, 0, 0, -7.6855);
		// stress += viscosity, with viscosity also being a 2x2 matrix
		double2x2 expectedUpdatedStress = new(-7.4855, 0.7, 0.7, -7.5855);
		double2x2 actualUpdatedStress = P2G2Math.UpdateStress(initialStress, viscosity);
		Assert.IsTrue(GeneralMathUtils.DeepEquals(actualUpdatedStress, expectedUpdatedStress));
	}

	[Test]
	public void ComputeEquation16Term0ShouldCorrectlyCalculateTheTerm()
	{
		double2x2 stress = new(-7.4855, 0.7, 0.7, -7.5855);
		double volume = 1;
		double dt = 0.2;
		// From Wolfram Alpha calculator
		double2x2 expectedTerm0 = new( 5.9884, -0.56, -0.56, 6.0684);
		double2x2 actualTerm0 = P2G2Math.ComputeEquation16Term0(stress, volume, dt);
		Assert.IsTrue(GeneralMathUtils.DeepEquals(actualTerm0, expectedTerm0));
	}

	[Test]
	public void ComputeDistanceFromCellToNeighborShouldCorrectlyCalculateDistance()
	{
		int[] neighborCellPosition = { 0, 1 };
		int[] currentCellPosition = { 1, 2 };
		double[] expectedDistance = { -0.5, -0.5 };
		double[] actualDistance = P2G2Math.ComputeDistanceFromCellToNeighbor(neighborCellPosition, currentCellPosition);
		Assert.That(actualDistance, Is.EqualTo(expectedDistance).Within(0.01));
	}

	[Test]
	public void ComputeMomentumShouldCorrectlyCalculateMomentum()
	{
		// fused force + momentum update
		double2x2 equation16Term0 = new(5.9884, -0.56, -0.56, 6.0684);
		double weight = 1.5376;
		double2 distanceFromCellToNeighbor = new(-0.5, -0.5);
		double2 expectedMomentum = new(-4.17335, -4.23486);
		double2 actualMomentum = P2G2Math.ComputeMomentum(equation16Term0, weight, distanceFromCellToNeighbor);
		Assert.IsTrue(GeneralMathUtils.DeepEquals(actualMomentum, expectedMomentum));
	}

	[Test]
	public void TestUpdateCellVelocity()
	{
		// initial state has this velocity at 0
		double2 initialCellVelocity = new(0, 0);
		double2 momentum = new(-4.17335, -4.23486);
		double2 expectedUpdatedCellVelocity = new(-4.17335, -4.23486);
		double2 actualUpdatedCellVelocity = P2G2Math.UpdateCellVelocity(momentum, initialCellVelocity);
		Assert.IsTrue(GeneralMathUtils.DeepEquals(actualUpdatedCellVelocity, expectedUpdatedCellVelocity));
	}

	[Test]
	public void ComputeHerschelBulkleyStressShouldCorrectlyComputeTheCauchyStressTensor()
	{
		// Inputs -- I am taking most of these values from the Columbia paper on Continuum Foam.
		// Specifically these are shaving cream values (except for strain, for which I just chose an easy matrix for my manual tests, and density, which I also chose an easy value for)
		double yieldStress_T0 = 31.9;
		double2x2 strain_deltaVPlusDeltaVTransposed = new(2, 0, 0, 2);
		double viscosity_mu = 27.2;
		double flowIndex_n = 0.22;
		double eosStiffness = 109;
		double density = 90;
		double restDensity = 77.7;
		int eosPower = 7;
		// Tests
		double2x2 expectedHerschelBulkleyStress = new(-132.3329425379, 31.9, 31.9, -132.3329425379);
		double2x2 actualHerschelBulkleyStress = P2G2Math.ComputeHerschelBulkleyStress(yieldStress_T0, strain_deltaVPlusDeltaVTransposed, viscosity_mu, flowIndex_n, eosStiffness, density, restDensity, eosPower);
		Debug.Log(actualHerschelBulkleyStress);
		Assert.IsTrue(GeneralMathUtils.DeepEquals(expectedHerschelBulkleyStress, actualHerschelBulkleyStress));
	}

	// add a unit test for correctly reducing to a Newtonian fluid

}
