using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using static FFF_Optimized_3D;

public class CSVExporter : ScriptableObject
{
    private static bool UsingWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    private static bool UsingMac()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }

    private static string BuildName(string simType, string timestamp, int numFrames)
    {
        // NumFrames are not necessarily frames, but update calls. Might rename.
        string part1 = "ParticleData_";
        return part1 + simType + "_" + timestamp + "_" + numFrames + "_frames.csv";
    }

    public void Export2DFluidOnlySimToCSV(Particle[] fluidParticlesOnly, int frame, string timestamp, int numFrames)
    {
        string simType = "FLUID_ONLY";
        StreamWriter sw = BuildStreamWriter(simType, timestamp, numFrames);
        sw?.WriteLine("X,Y,Frame");

        foreach (Particle p in fluidParticlesOnly)
        {
            double x = p.GetPosition().x;
            double y = p.GetPosition().y;
            sw?.WriteLine(x + "," + y + "," + frame);
        }
        sw?.Close();
    }

    private StreamWriter BuildStreamWriter(string simType, string timestamp, int numFrames)
    {
        StreamWriter sw;
        string name = BuildName(simType, timestamp, numFrames);
        if (UsingWindows())
        {
            sw = new StreamWriter(@"c:\Users\alexc\School_Repos\MLS-MPM-unity\MLS-MPM\Assets\Resources\AnimData\" + name, true);
        }
        else if (UsingMac())
        {
            sw = new StreamWriter(@"/Users/Alex/Documents/Alex's Crap/Escuela/MS/Winter_2023/MLS-MPM-unity/MLS-MPM/Assets/Resources/AnimData/" + name, true);
        }
        else
        {
            // Linux
            sw = new StreamWriter(@"~/Desktop/" + name, true);
        }
        return sw;
    }

    public void ExportParticleDataToCSV(Particle_3D[] fluidParticlesOnly, int frame, string timestamp, int numFrames, string simType="DEFAULT")
    {
        //Debug.Log("Exporting to CSV");
        StreamWriter sw = BuildStreamWriter(simType, timestamp, numFrames);
        sw?.WriteLine("X,Y,Z,Frame");

        foreach (Particle_3D p in fluidParticlesOnly)
        {
            double x = p.GetPosition().x;
            double y = p.GetPosition().y;
            double z = p.GetPosition().z;
            sw?.WriteLine(x + "," + y + "," + z + "," + frame);
        }

        sw?.Close();
        //Debug.Log("End export");
    }
}
