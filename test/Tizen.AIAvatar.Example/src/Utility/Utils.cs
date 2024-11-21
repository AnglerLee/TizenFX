using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AIAvatar
{
    public class WaveData
    {
        public int SampleRate { get; set; }
        public short NumChannels { get; set; }
        public byte[] RawAudioData { get; set; }
    }

    public static class Utils
    {
        public static string LogTag = "Tizen.AIAvatar";
        public static string TTSText = "Select an avatar that will guide you through the functions of your age.";
        public static string ResourcePath = Tizen.Applications.Application.Current.DirectoryInfo.Resource;

        public static WaveData LoadWave(string path)
        {
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                // RIFF chunk identifier
                string riffChunkID = new string(reader.ReadChars(4));
                if (riffChunkID != "RIFF")
                {
                    throw new Exception("Invalid WAV file");
                }

                // RIFF chunk size
                reader.ReadInt32();

                // Format
                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                {
                    throw new Exception("Invalid WAV file");
                }

                // FMT subchunk
                string fmtSubChunkID = new string(reader.ReadChars(4));
                if (fmtSubChunkID != "fmt ")
                {
                    throw new Exception("Invalid WAV file");
                }

                // FMT subchunk size
                int fmtSubChunkSize = reader.ReadInt32();

                // Audio format
                short audioFormat = reader.ReadInt16();
                if (audioFormat != 1)
                {
                    throw new Exception("Unsupported audio format");
                }

                // Number of channels
                short numChannels = reader.ReadInt16();

                // Sample rate
                int sampleRate = reader.ReadInt32();

                // Byte rate
                reader.ReadInt32();

                // Block align
                reader.ReadInt16();

                // Bits per sample
                short bitsPerSample = reader.ReadInt16();

                // Skip any extra bytes in the FMT subchunk
                if (fmtSubChunkSize > 16)
                {
                    reader.ReadBytes(fmtSubChunkSize - 16);
                }

                // Find the data subchunk
                
                int dataSubChunkSize = 0;
                while (true)
                {
                    string dataSubChunkID = new string(reader.ReadChars(4));
                    dataSubChunkSize = reader.ReadInt32();

                    if (dataSubChunkID == "data")
                    {
                        break;
                    }
                    else
                    {
                        reader.ReadBytes(dataSubChunkSize);
                    }
                }

                // Read the raw audio data
                byte[] rawAudioData = reader.ReadBytes(dataSubChunkSize);


                return new WaveData
                {
                    SampleRate = sampleRate,
                    NumChannels = numChannels,
                    RawAudioData = rawAudioData
                };
            }
        }

        public static void FullGC()
        {
            global::System.GC.Collect();
            global::System.GC.WaitForPendingFinalizers();
            global::System.GC.Collect();
        }
    }
}
