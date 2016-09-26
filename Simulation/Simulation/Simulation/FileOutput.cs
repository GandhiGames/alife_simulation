using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    static class FileOutput
    {
        private static StreamWriter writer;
        private static string fileNameWeights,
            fileNameVelocity, fileNameDistance, fileNameTimeAlive;
        private static int genCount = 0;
        private static List<float> meanDistances = new List<float>();

        static FileOutput()
        {
            fileNameWeights = "weights.js";
            writer = new StreamWriter(fileNameWeights);
            writer.Write("var weights = [");
            writer.Close();

            fileNameVelocity = "velocity.js";
            writer = new StreamWriter(fileNameVelocity);
            writer.Write("var velocity = [");
            writer.Close();

            fileNameDistance = "distance.js";
            writer = new StreamWriter(fileNameDistance);
            writer.Write("var distance = [");
            writer.Close();

            fileNameTimeAlive = "timeAlive.js";

        }

        public static void WriteTimeAliveGeneration(float timeAlive, float genNo)
        {
            if (genNo == 100)
            {
                StringBuilder stringBuilder = new StringBuilder();

                using (StreamWriter sw = File.AppendText(fileNameTimeAlive))
                {
                    stringBuilder.Append(timeAlive + ", "); //genNo

                    sw.Write(stringBuilder.ToString());

                    sw.Close();
                }
            }

        }

        public static void WriteWeightsToFile(List<float> data)
        {
            StringBuilder stringBuilder = new StringBuilder(); 

            using (StreamWriter sw = File.AppendText(fileNameWeights))
            {
                stringBuilder.Append("{timeAlive:" + data[0] + ", "); //timeAlive
                stringBuilder.Append("seekLat:" + data[1] + ", ");
                stringBuilder.Append("seekSight:" + data[2] + ", ");
                stringBuilder.Append("alignSight:" + data[3] + ", ");
                stringBuilder.Append("alignLat:" + data[4] + ", ");
                stringBuilder.Append("cohesionSight:" + data[5] + ", ");
                stringBuilder.Append("cohesionLat:" + data[6] + ", ");
                stringBuilder.Append("seperationSight:" + data[7] + ", ");
                stringBuilder.Append("seperationLat:" + data[8] + ", ");
                stringBuilder.Append("wander:" + data[9] + ", ");
                stringBuilder.Append("velocity:" + data[11] + ", "); //velocity
                stringBuilder.Append("genNo:" + data[10] * 2f + "},"); //genNo
                sw.Write(stringBuilder.ToString());

                sw.Close();
            }

            List<float> velocityList = new List<float>();
            velocityList.Add(data[0]);
            velocityList.Add(data[11]);
            velocityList.Add(data[10]);
            WriteVelocityToFile(velocityList);

            WriteTimeAliveGeneration(data[0], data[10]);
        }

        private static void WriteVelocityToFile(List<float> data)
        {
            StringBuilder stringBuilder = new StringBuilder();

            using (StreamWriter sw = File.AppendText(fileNameVelocity))
            {
                stringBuilder.Append("{x:" + data[0] + ", "); //timeAlive
                stringBuilder.Append("y:" + data[1] + ", "); //velocity
                stringBuilder.Append("z:" + data[2] * 2f + "},"); //genNo
                sw.Write(stringBuilder.ToString());

                sw.Close();
            }


        }

        public static void WritePreyMeanOverallDistancesFromPredToFile(float meanDistance)
        {
            genCount++;

            meanDistances.Add(meanDistance);

            if (genCount >= 10)
            {
                int b = 0;

                using (StreamWriter sw = File.AppendText(fileNameDistance))
                {
                    for (int i = genCount; i > 0; i--)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("{x:" + (Utilities.GenerationNumPrey - i) + ", "); //genNo
                        stringBuilder.Append("y:" + meanDistances[b] + "}, "); //mean distance
                     
                        sw.Write(stringBuilder.ToString());
                        b++;
                    }

                    sw.Close();
                }

                meanDistances.Clear();



                genCount = 0;
            }

        }

        public static void WritePreyMeanOverallDistancesToFile(float meanDistance)
        {
            genCount++;

            meanDistances.Add(meanDistance);

            if (genCount >= 10)
            {
                int b = 0;

                using (StreamWriter sw = File.AppendText(fileNameDistance))
                {
                    for (int i = genCount; i > 0; i--)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        //stringBuilder.Append("{x:" + (Utilities.GenerationNumPrey - i) + ", "); //genNo
                        //stringBuilder.Append("y:" + meanDistances[b] + "}, "); //mean distance

                        stringBuilder.Append(Utilities.GenerationNumPrey + ", "); //genNo
                        stringBuilder.Append(meanDistances[b] + ", "); //genNo
                        sw.Write(stringBuilder.ToString());
                        b++;
                    }

                    sw.Close();
                }

                meanDistances.Clear();



                genCount = 0;
            }

        }

    }
}
