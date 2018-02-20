using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer.Http.REST
{
    public static class Token
    {

        public static RsaCryption Cryption { get; set; }

        //Vytvoří nový token
        //Není můj kód
        //https://stackoverflow.com/questions/14643735/how-to-generate-a-unique-token-which-expires-after-24-hours
        public static string GenerateNew(int UserId)
        {
            //Vezme aktuální čas a převede na pole bytů
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            //Token vznikne kombinací času, globálně unikatního id, náhodných čísel a id uživatele
            string token = Convert.ToBase64String(time) + UserId;
            //Nakonec je zašifrován
            return Cryption.Encrypt(token);
        }

        //Ověření platnosti
        public static bool IsValid(string token)
        {
            string sub = token.Substring(0, 12);
            byte[] data = Convert.FromBase64String(sub);
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            if (when < DateTime.UtcNow.AddHours(-24))
            {
                return false;
            }
            return true;
        }

        public static string Decrypt(string toDecrypt)
        {
            return Cryption.Decrypt(toDecrypt);
        }

        //Ověření platnosti z hlavičky
        //To kvůli rozdílům v POST request, kde je token posílín v těle a GET request, kde je posílán jako hlavička
        //A hlavička obsahuje text "Basic " a poté následuje token
        /* public static bool IsValidFromHeader(string token)
         {
             string decrypted = cryption.Decrypt(token.Substring(6));
             string sub = decrypted.Substring(0, 24);
             byte[] data = Convert.FromBase64String(sub);
             DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
             if (when < DateTime.UtcNow.AddHours(-24))
             {
                 return false;
             }
             return true;
         }*/


        //Token bude vždy posílaný v HTTP hlavičce
        public static int? Verify(string token)
        {
            string sub = token.Substring(7);
            sub = sub.Substring(0, sub.Length-1);
            try
            {
                string decrypted = Cryption.Decrypt(sub);
                if (Token.IsValid(decrypted))
                {
                    return Int32.Parse(decrypted.Substring(12));
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }


        }


        //Vezme id uživatele z tokenu
      /*  public static int GetUserId(string token)
        {
            string sub = "";
            if (token.Contains("Basic"))
            {
                var decrypted = cryption.Decrypt(token.Substring(6));
                sub = decrypted.Substring(44);
            }
            else
            {
                var decrypted = cryption.Decrypt(token);
                sub = decrypted.Substring(44);
            }
            return Int32.Parse(sub);
        }*/




    }
}
