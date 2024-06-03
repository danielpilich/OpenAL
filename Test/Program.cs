using GLFW;

using OpenTK;

using OpenTK.Audio.OpenAL;

using HelpersNS;

namespace PMLabs
{


    //Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenAL do współpracy z OpenTK.
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {

        static ALDevice device; //pole klasy
        static ALContext context; //pole klasy
        static int buf; //pole klasy
        static int source; //pole klasy

        public static void InitSound()
        {
            device = ALC.OpenDevice(null);
            context = ALC.CreateContext(device, new ALContextAttributes());
            ALC.MakeContextCurrent(context);

            buf = AL.GenBuffer();

            int channels, bits, sampleFreq;
            byte[] data = Helpers.LoadWave("test.wav", out channels, out bits, out sampleFreq);
            AL.BufferData<byte>(buf, Helpers.GetFormat(channels, bits), data, sampleFreq);

            source = AL.GenSource();

            AL.BindBufferToSource(source, buf);

            AL.SourcePlay(source);

        }

        public static void FreeSound()
        {
            AL.SourceStop(source);
            AL.DeleteSource(source); 
            AL.DeleteBuffer(buf);

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

        public static void SoundEvents()
        {

        }

        //Metoda główna
        static void Main(string[] args)
        {
            Glfw.Init();//Zainicjuj bibliotekę GLFW

            Window window = Glfw.CreateWindow(500, 500, "OpenAL", GLFW.Monitor.None, Window.None); //Utwórz okno o wymiarach 500x500 i tytule "OpenAL"

            Glfw.MakeContextCurrent(window); //Ustaw okno jako aktualny kontekst OpenAL - tutaj będą realizowane polecenia OpenAL
 
            InitSound();


            while (!Glfw.WindowShouldClose(window)) //Wykonuj tak długo, dopóki użytkownik nie zamknie okna
            {
                SoundEvents();
                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }


            FreeSound();
            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }


    }
}