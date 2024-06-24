using GLFW;
using OpenTK;
using OpenTK.Audio.OpenAL;
using HelpersNS;
using System.Collections.Generic;

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



        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            /*
            int sourcestate;
            AL.GetSource(source, ALGetSourcei.SourceState, out sourcestate);
            int sourcestate2;
            AL.GetSource(source2, ALGetSourcei.SourceState, out sourcestate2);
            int sourcestate3;
            AL.GetSource(source3, ALGetSourcei.SourceState, out sourcestate3);
            int sourcestate4;
            AL.GetSource(source4, ALGetSourcei.SourceState, out sourcestate4);
            int sourcestate5;
            AL.GetSource(source5, ALGetSourcei.SourceState, out sourcestate5);
            int sourcestate6;
            AL.GetSource(source6, ALGetSourcei.SourceState, out sourcestate6);
            int sourcestate7;
            AL.GetSource(source7, ALGetSourcei.SourceState, out sourcestate7);
            int sourcestate8;
            AL.GetSource(source8, ALGetSourcei.SourceState, out sourcestate8);
            */

        }

        public static short Signal(double t, double f, double A)
        {
            return (short)(A * Math.Sin(2.0 * Math.PI * f * t));
        }

        public static void InitSound()
        {
            device = ALC.OpenDevice(null);
            context = ALC.CreateContext(device, new ALContextAttributes());
            ALC.MakeContextCurrent(context);

            for (int i = 0; i < 8; i++)
            {
                buffers[i] = AL.GenBuffer();
            }

            double[] frequencies = { 261.6, 293.7, 329.6, 349.2, 392.0, 440.0, 493.9, 523.3 };
            double A = short.MaxValue;
            int fp = 44100;
            double op = 1.0 / fp;
            int lp = 1 * fp;

            for (int i = 0; i < 8; i++)
            {
                short[] data = new short[lp];
                for (int x = 0; x < lp; x++)
                {
                    data[x] = Signal(op * x, frequencies[i], A);
                }
                AL.BufferData(buffers[i], ALFormat.Mono16, data, fp);
            }

            for (int i = 0; i < 8; i++)
            {
                sources[i] = AL.GenSource();
                AL.Source(sources[i], ALSourceb.Looping, false);
                AL.BindBufferToSource(sources[i], buffers[i]);
            }

            // Example song sequence
            // You can modify this sequence to create your own song
            int[] songSequence = { 0, 1, 2, 3, 4, 5, 6, 7 };
            foreach (var index in songSequence)
            {
                soundQueue.Enqueue(index);
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

        static void Main(string[] args)
        {
            Glfw.Init();

            Window window = Glfw.CreateWindow(500, 500, "OpenAL", GLFW.Monitor.None, Window.None);

            Glfw.MakeContextCurrent(window);
            Glfw.SetKeyCallback(window, kc);

            InitSound();

            while (!Glfw.WindowShouldClose(window))
            {
                SoundEvents();
                Glfw.PollEvents();
            }

            FreeSound();
            Glfw.Terminate();
        }
    }
}