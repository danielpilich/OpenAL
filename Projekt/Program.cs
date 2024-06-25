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
        static int[] buffers = new int[8];
        static int[] sources = new int[8];
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

            AL.GenBuffers(buffers);
            AL.GenSources(sources);
        }

        public static void QueueMusic()
        {
            // Example song: C4 (261.63 Hz), D4 (293.66 Hz), E4 (329.63 Hz), rest, G4 (392.00 Hz)
            List<(double frequency, double duration)> song = new List<(double, double)>
            {
                (261.63, 0.5), // C4
                (293.66, 0.5), // D4
                (329.63, 0.5), // E4
                (0.0, 0.5),    // Rest
                (392.00, 0.5)  // G4
            };

            for (int i = 0; i < song.Count; i++)
            {
                var (frequency, duration) = song[i];
                int sampleRate = 44100;
                int samples = (int)(duration * sampleRate);
                short[] data = new short[samples];

                for (int j = 0; j < samples; j++)
                {
                    double t = j / (double)sampleRate;
                    data[j] = Signal(t, frequency, 32760);
                }

                GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                IntPtr dataPtr = handle.AddrOfPinnedObject();

                AL.BufferData(buffers[i], ALFormat.Mono16, dataPtr, data.Length * sizeof(short), sampleRate);
                handle.Free();

                AL.SourceQueueBuffer(sources[i], buffers[i]);
                soundQueue.Enqueue(i);
            }

            PlayNextSound();
        }

        public static void FreeSound()
        {
            for (int i = 0; i < 8; i++)
            {
                AL.SourceStop(sources[i]);
                AL.DeleteSource(sources[i]);
                AL.DeleteBuffer(buffers[i]);
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