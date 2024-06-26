using OpenTK;
using OpenTK.Audio.OpenAL;
using GLFW;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PMLabs
{
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {
        static KeyCallback kc = KeyProcessor;

        static ALDevice device;
        static ALContext context;
        static List<int> buffers = new List<int>();
        static List<int> sources = new List<int>();
        static Queue<int> soundQueue = new Queue<int>();
        static int currentSourceIndex = -1;

        static void Main(string[] args)
        {
            Glfw.Init();

            Window window = Glfw.CreateWindow(500, 500, "OpenAL", GLFW.Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Glfw.SetKeyCallback(window, kc);

            InitSound();
            QueueMusic();

            while (!Glfw.WindowShouldClose(window))
            {
                SoundEvents();
                Glfw.PollEvents();
            }

            FreeSound();
            Glfw.Terminate();
        }

        public static void KeyProcessor(IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            // Implement key processing if needed
        }

        public static short Signal(double t, double f, double A)
        {
            return (short)(A * Math.Sin(2.0 * Math.PI * f * t));
        }

        public static void InitSound()
        {
            device = ALC.OpenDevice(null);
            context = ALC.CreateContext(device, (int[])null);
            ALC.MakeContextCurrent(context);
        }

        public static void QueueMusic()
        {
            List<(double frequency, double duration)> song = new List<(double, double)>
            {
                //(261.6, 1),  // c4
                //(293.7, 1),  // d4
                //(311.1, 1),  // d#4
                //(349.2, 1),  // f4
                //(392.0, 1),  // g4
                //(415.3, 1),  // g#4
                //(466.2, 1),  // a#4
                //(523.3, 1),  // c5

                (392.0, 0.5),  // g4

                (261.6, 0.5),  // c4
                (261.6, 0.25),  // c4
                (261.6, 0.25),  // c4
                (261.6, 0.5),  // c4
                (311.1, 0.5),  // d#4
                
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.25),  // g4

                (415.3, 0.5),  // g#4
                (349.2, 0.25),  // f4
                (349.2, 0.25),  // f4
                (349.2, 0.5),  // f4
                (349.2, 0.25),  // f4
                (415.3, 0.25),  // g#4

                (523.3, 0.25),  // c5
                (523.3, 0.5),  // c5
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.25),  // g4

                (261.6, 0.5),  // c4
                (261.6, 0.5),  // c4
                (261.6, 0.5),  // c4
                (311.1, 0.5),  // d#4

                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4

                (349.2, 0.5),  // f4
                (349.2, 0.5),  // f4
                (311.1, 0.25),  // d#4
                (311.1, 0.25),  // d#4
                (293.7, 0.5),  // d4

                (261.6, 2),  // c4
                
                (0.0, 0.25),  // Pause

                (523.3, 1),  // c5
                (523.3, 0.5),  // c5
                (415.3, 0.25),  // g#4

                (466.2, 0.25),  // a#4
                (466.2, 0.25),  // a#4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.25),  // g4

                (415.3, 0.5),  // g#4
                (349.2, 0.5),  // f4
                (349.2, 0.25),  // f4
                (392.0, 0.25),  // g4
                (415.3, 0.5),  // g#4

                (523.3, 0.5),  // c5
                (392.0, 0.5),  // g4
                (392.0, 1),  // g4

                (523.3, 1),  // c5
                (523.3, 0.5),  // c5
                (415.3, 0.25),  // g#4
                (415.3, 0.25),  // g#4

                (466.2, 0.25),  // a#4
                (466.2, 0.25),  // a#4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4

                (392.0, 0.5),  // g4
                (349.2, 0.5),  // f4
                (311.1, 0.5),  // d#4
                (293.7, 0.5),  // d4

                (261.6, 1),  // c4
                
                (0.0, 0.5),  // Pause

                (392.0, 0.5),  // g4

                (261.6, 0.5),  // c4
                (261.6, 0.25),  // c4
                (261.6, 0.25),  // c4
                (261.6, 0.5),  // c4
                (311.1, 0.5),  // d#4
                
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.25),  // g4

                (415.3, 0.5),  // g#4
                (349.2, 0.25),  // f4
                (349.2, 0.25),  // f4
                (349.2, 0.5),  // f4
                (349.2, 0.25),  // f4
                (415.3, 0.25),  // g#4

                (523.3, 0.25),  // c5
                (523.3, 0.5),  // c5
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.25),  // g4

                (261.6, 0.5),  // c4
                (261.6, 0.5),  // c4
                (261.6, 0.5),  // c4
                (311.1, 0.5),  // d#4

                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4

                (349.2, 0.5),  // f4
                (349.2, 0.5),  // f4
                (311.1, 0.25),  // d#4
                (311.1, 0.25),  // d#4
                (293.7, 0.5),  // d4

                (261.6, 2),  // c4
                
                (0.0, 0.25),  // Pause

                (523.3, 1),  // c5
                (523.3, 0.5),  // c5
                (415.3, 0.25),  // g#4

                (466.2, 0.25),  // a#4
                (466.2, 0.25),  // a#4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.25),  // g4

                (415.3, 0.5),  // g#4
                (349.2, 0.5),  // f4
                (349.2, 0.25),  // f4
                (392.0, 0.25),  // g4
                (415.3, 0.5),  // g#4

                (523.3, 0.5),  // c5
                (392.0, 0.5),  // g4
                (392.0, 1),  // g4

                (523.3, 1),  // c5
                (523.3, 0.5),  // c5
                (415.3, 0.25),  // g#4
                (415.3, 0.25),  // g#4

                (466.2, 0.25),  // a#4
                (466.2, 0.25),  // a#4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4
                (392.0, 0.5),  // g4

                (392.0, 0.5),  // g4
                (349.2, 0.5),  // f4
                (311.1, 0.5),  // d#4
                (293.7, 0.5),  // d4

                (261.6, 1),  // c4
            };

            foreach (var (frequency, duration) in song)
            {
                int sampleRate = 44100;
                int samples = (int)(duration * sampleRate);
                short[] data = new short[samples];

                for (int j = 0; j < samples; j++)
                {
                    double t = j / (double)sampleRate;
                    data[j] = Signal(t, frequency, 32760);
                }

                int buffer = AL.GenBuffer();
                int source = AL.GenSource();

                GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                IntPtr dataPtr = handle.AddrOfPinnedObject();

                AL.BufferData(buffer, ALFormat.Mono16, dataPtr, data.Length * sizeof(short), sampleRate);
                handle.Free();

                AL.SourceQueueBuffer(source, buffer);

                buffers.Add(buffer);
                sources.Add(source);
                soundQueue.Enqueue(sources.Count - 1);
            }

            PlayNextSound();
        }

        public static void FreeSound()
        {
            foreach (var source in sources)
            {
                AL.SourceStop(source);
                AL.DeleteSource(source);
            }

            foreach (var buffer in buffers)
            {
                AL.DeleteBuffer(buffer);
            }

            if (context != ALContext.Null)
            {
                ALC.MakeContextCurrent(ALContext.Null);
                ALC.DestroyContext(context);
            }
            context = ALContext.Null;

            if (device != ALDevice.Null)
            {
                ALC.CloseDevice(device);
            }
            device = ALDevice.Null;
        }

        public static void PlayNextSound()
        {
            if (soundQueue.Count > 0)
            {
                currentSourceIndex = soundQueue.Dequeue();
                AL.SourcePlay(sources[currentSourceIndex]);
            }
        }

        public static void SoundEvents()
        {
            if (currentSourceIndex != -1)
            {
                int state;
                AL.GetSource(sources[currentSourceIndex], ALGetSourcei.SourceState, out state);
                if (state != (int)ALSourceState.Playing)
                {
                    PlayNextSound();
                }
            }
        }
    }
}