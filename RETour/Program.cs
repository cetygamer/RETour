using EasyHook;
using REHookLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;

namespace RETour
{
    public class Program
    {
        private static string _targetExe = "";
        private static String ChannelName;

        static void Main()
        {
            _targetExe = Path.Combine(Environment.CurrentDirectory, "RESIDENTEVIL.EXE");
            Int32 targetPID = 0;
            try
            {
                PrintInfoAndWarning();

                Console.ReadKey();

                if(!File.Exists(_targetExe))
                {
                    Console.WriteLine(String.Format("RESIDENTEVIL.EXE introuvable dans le dossier {0} !", Environment.CurrentDirectory));
                    Console.WriteLine("Appuyez sur une touche pour quitter... ");
                    Console.ReadKey();
                    return;
                }

                RemoteHooking.IpcCreateServer<IpcInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);
                string injectionLibrary = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "REHookLib.dll");
                RemoteHooking.CreateAndInject(_targetExe, "", 0, InjectionOptions.DoNotRequireStrongName, injectionLibrary, injectionLibrary, out targetPID, ChannelName);
                Console.WriteLine("Created and injected process {0}", targetPID);
                while(true)
                {
                    if (Process.GetProcessesByName("RESIDENTEVIL").Length <= 0)
                    {
                        Environment.Exit(0);
                    }
                    Thread.Sleep(2000);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("There was an error while connecting to target:\r\n{0}", exception.ToString());
                Debug.WriteLine("<Press any key to exit>");
            }
        }

        private static void PrintInfoAndWarning()
        {
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("                 Resident Evil");
            Console.WriteLine("             VERSION CD Francaise");
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("            www.abandonware-france.org");
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine(" Merci aux forumeurs sur le site http://re123.bplaced.net/ pour leurs informations précieuses ! ");
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Commandes par défaut au clavier :");
            Console.WriteLine(" Touches fléchées : se déplacer");
            Console.WriteLine(" C ou Espace ou Entrée : Action/Choisir/Frapper/Tirer");
            Console.WriteLine(" V : Courir");
            Console.WriteLine(" B : Pointer/Viser");
            Console.WriteLine(" Z : Inventaire");
            Console.WriteLine(" A : Options du jeu (permet de configurer les commandes)");
            Console.WriteLine(" ALT + F4 : QUITTER LE JEU");
            Console.WriteLine();
            Console.WriteLine("Commandes par défaut à la manette (testé avec une manette Xbox 360) :");
            Console.WriteLine(" Pad directionnel : se déplacer");
            Console.WriteLine(" A : Action/Choisir/Frapper/Tirer");
            Console.WriteLine(" B : Courir");
            Console.WriteLine(" LB : Pointer/Viser");
            Console.WriteLine(" Y : Inventaire");
            Console.WriteLine(" Start : Options du jeu (permet de configurer les commandes)");

            Console.WriteLine();
            Console.WriteLine(@"/!\ Ne pas utiliser ALT-TAB, ni CTRL-ESC, ni la touche Windows en cours de jeu,");
            Console.WriteLine(@"/!\ sous peine de devoir redémarrer le jeu");
            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour continuer... ");
        }
    }
}
