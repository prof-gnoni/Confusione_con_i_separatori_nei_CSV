// File: Persona.cs
using System;
using System.Globalization; // Aggiunto per il ToString()

// Il namespace è stato aggiornato come richiesto
namespace Gestione_Corretta_Separatori_Altezze_CSV
{
    public class Persona
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public float Altezza { get; set; }

        public Persona(string nome, string cognome, float altezza)
        {
            Nome = nome;
            Cognome = cognome;
            Altezza = altezza;
        }

        public override string ToString()
        {
            // Usiamo 'F2' per vedere 2 decimali e 'CurrentCulture'
            // per far sì che ToString() rispetti la cultura (es. 1,80)
            return $"Nome: {Nome}, Cognome: {Cognome}, Altezza: {Altezza.ToString("F2", CultureInfo.CurrentCulture)}m";
        }
    }
}