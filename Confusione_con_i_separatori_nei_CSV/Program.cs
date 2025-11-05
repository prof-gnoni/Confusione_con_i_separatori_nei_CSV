// File: Program.cs
using System;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq; // Aggiunto per LINQ (opzionale ma pulito)

namespace Gestione_Corretta_Separatori_Altezze_CSV
{
    public class Program
    {
        // La nostra lista di dati di partenza
        private static List<Persona> persone = new List<Persona>
        {
            new Persona("Mario", "Rossi", 1.80f),
            new Persona("Anna", "Verdi", 1.65f)
        };

        private static string percorsoFile = "anagrafica.csv";

        public static void Main(string[] args)
        {
            // --- IMPOSTAZIONE: SIMULIAMO UN PC ITALIANO ---
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("it-IT");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("it-IT");

            Console.WriteLine($"Simulazione avviata. Il nostro PC è ITALIANO ('it-IT').");
            Console.WriteLine($"Separatore Elenco atteso: '{CultureInfo.CurrentCulture.TextInfo.ListSeparator}'");
            Console.WriteLine($"Separatore Decimali atteso: '{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}'");
            Console.WriteLine(new string('=', 40));

            // --- FASE 1: IL PROBLEMA ---
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nFASE 1: Un collega Americano salva un file...");
            Console.ResetColor();

            // Metodo di salvataggio aggiornato
            SalvaCsv_ComeAmericano(percorsoFile, persone);

            Console.WriteLine($"File creato: '{percorsoFile}' (formato USA: ',' e '.')");
            Console.WriteLine("Contenuto del file salvato:");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(File.ReadAllText(percorsoFile));
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nOra, noi (Italiani) proviamo a caricarlo...");
            Console.ResetColor();

            // Questo metodo usa già ReadAllLines (ed è corretto)
            CaricaCsv_ComeItaliano_PROBLEMA(percorsoFile);

            Console.WriteLine(new string('=', 40));

            // --- FASE 2: LA SOLUZIONE (LOCALE) ---
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nFASE 2: Un Italiano salva e ricarica il file correttamente...");
            Console.ResetColor();

            // Metodo di salvataggio aggiornato
            SalvaCsv_ComeItaliano(percorsoFile, persone);

            Console.WriteLine($"File salvato: '{percorsoFile}' (formato ITA: ';' e ',')");
            Console.WriteLine("Contenuto del file salvato:");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(File.ReadAllText(percorsoFile));
            Console.ResetColor();

            Console.WriteLine("\nOra ricarichiamo (da Italiani) il nostro stesso file:");

            // Questo metodo usa già ReadAllLines (ed è corretto)
            List<Persona> personeCaricate = CaricaCsv_ComeItaliano_SOLUZIONE(percorsoFile);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n--- CARICAMENTO RIUSCITO ---");
            foreach (var p in personeCaricate)
            {
                Console.WriteLine(p.ToString());
            }
            Console.ResetColor();

            Console.ReadLine();
        }

        /// <summary>
        /// PROBLEMA: Simula un PC Americano che salva un CSV.
        /// Usa ',' come separatore elenco E '.' come decimale.
        /// *** USA File.WriteAllLines ***
        /// </summary>
        public static void SalvaCsv_ComeAmericano(string percorso, List<Persona> lista)
        {
            var culturaUSA = CultureInfo.GetCultureInfo("en-US");
            char sepElenco = culturaUSA.TextInfo.ListSeparator[0]; // Sarà ','

            // 1. Prepariamo un elenco di stringhe da scrivere
            var righe = new List<string>();

            // 2. Aggiungiamo l'intestazione
            righe.Add($"Nome{sepElenco}Cognome{sepElenco}Altezza");

            // 3. Cicliamo e aggiungiamo ogni persona come stringa
            foreach (var p in lista)
            {
                string altezzaString = p.Altezza.ToString(culturaUSA);
                string riga = $"{p.Nome}{sepElenco}{p.Cognome}{sepElenco}{altezzaString}";
                righe.Add(riga);
            }

            // 4. Scriviamo tutte le righe sul file in un colpo solo
            File.WriteAllLines(percorso, righe, Encoding.UTF8);
        }

        /// <summary>
        /// PROBLEMA: Simula un PC Italiano che carica un file Americano.
        /// *** USA File.ReadAllLines *** (Questo era già coerente)
        /// </summary>
        public static void CaricaCsv_ComeItaliano_PROBLEMA(string percorso)
        {
            char sepElencoItaliano = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0]; // Sarà ';'

            string[] righe = File.ReadAllLines(percorso);
            string rigaDati = righe[1]; // Leggiamo la prima riga di dati: "Mario,Rossi,1.80"

            // ... (La logica del problema rimane identica) ...
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n--- 1° ERRORE (ListSeparator) ---");
            string[] campi = rigaDati.Split(sepElencoItaliano);
            Console.WriteLine($"Dividendo con ';', ho trovato {campi.Length} campo/i:");
            Console.WriteLine($"Campo 0: '{campi[0]}'");
            Console.ResetColor();

            Console.WriteLine("\n... un programmatore 'ingenuo' corregge usando la virgola...");
            campi = rigaDati.Split(',');
            string altezzaString = campi[2]; // "1.80"

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n--- 2° ERRORE (NumberDecimalSeparator) ---");
            Console.WriteLine($"Ora provo a convertire: float.Parse(\"{altezzaString}\")");

            float altezzaCorrotta = float.Parse(altezzaString);
            Console.WriteLine($"Conversione riuscita! Il valore è: {altezzaCorrotta}");

            Persona p = new Persona(campi[0], campi[1], altezzaCorrotta);
            Console.WriteLine("\n--- RISULTATO FINALE CORROTTO ---");
            Console.WriteLine(p.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// SOLUZIONE: Un PC Italiano salva usando le SUE regole.
        /// *** USA File.WriteAllLines ***
        /// </summary>
        public static void SalvaCsv_ComeItaliano(string percorso, List<Persona> lista)
        {
            var culturaCorrente = CultureInfo.CurrentCulture;
            char sepElenco = culturaCorrente.TextInfo.ListSeparator[0]; // Sarà ';'

            var righe = new List<string>();
            righe.Add($"Nome{sepElenco}Cognome{sepElenco}Altezza");

            foreach (var p in lista)
            {
                string altezzaString = p.Altezza.ToString(culturaCorrente);
                string riga = $"{p.Nome}{sepElenco}{p.Cognome}{sepElenco}{altezzaString}";
                righe.Add(riga);
            }

            File.WriteAllLines(percorso, righe, Encoding.UTF8);
        }

        /// <summary>
        /// SOLUZIONE: Un PC Italiano carica un file Italiano.
        /// *** USA File.ReadAllLines *** (Questo era già coerente)
        /// </summary>
        public static List<Persona> CaricaCsv_ComeItaliano_SOLUZIONE(string percorso)
        {
            var personeCaricate = new List<Persona>();
            var culturaCorrente = CultureInfo.CurrentCulture;
            char sepElenco = culturaCorrente.TextInfo.ListSeparator[0]; // Sarà ';'

            // 1. Legge tutte le righe in un colpo solo
            var righe = File.ReadAllLines(percorso);

            // 2. Le processa (saltando l'intestazione)
            for (int i = 1; i < righe.Length; i++)
            {
                string riga = righe[i]; // es. "Mario;Rossi;1,80"
                string[] campi = riga.Split(sepElenco);

                if (campi.Length == 3)
                {
                    string nome = campi[0];
                    string cognome = campi[1];
                    string altezzaString = campi[2]; // "1,80"

                    float altezza = float.Parse(altezzaString, culturaCorrente);

                    personeCaricate.Add(new Persona(nome, cognome, altezza));
                }
            }
            return personeCaricate;
        }
    }
}