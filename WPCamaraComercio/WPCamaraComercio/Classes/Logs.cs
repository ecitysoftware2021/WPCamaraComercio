using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPCamaraComercio.Classes
{
    class Logs
    {
    }

    public class LogDispenser
    {
        public string SendMessage { get; set; }

        public string ResponseMessage { get; set; }

        public string TransactionId { get; set; }

        public DateTime DateDispenser { get; set; }
    }

    public class LogService
    {
        public string NamePath { get; set; }

        public string FileName { get; set; }

        public void CreateLogs<T>(T model)
        {
            var json = JsonConvert.SerializeObject(model);
            if (!Directory.Exists(NamePath))
            {
                Directory.CreateDirectory(NamePath);
            }

            var nameFile = Path.Combine(NamePath, FileName);
            if (!File.Exists(nameFile))
            {
                var archivo = File.CreateText(nameFile);
                archivo.Close();
            }

            using (StreamWriter sw = File.AppendText(nameFile))
            {
                sw.WriteLine(json);
            }
        }

        public void CreateLogsTransactions<T>(T model)
        {
            var json = JsonConvert.SerializeObject(model);
            string fullPath = string.Format(@"C:\\LogsCamaraMedellin\{0}\", NamePath);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var nameFile = Path.Combine(fullPath, FileName);
            if (!File.Exists(nameFile))
            {
                var archivo = File.CreateText(nameFile);
                archivo.Close();
            }

            using (StreamWriter sw = File.AppendText(nameFile))
            {
                sw.WriteLine(json);
            }
        }

    }

    public class LogErrorGeneral
    {
        public int IdTransaction { get; set; }
        public decimal ValuePay { get; set; }
        public string Date { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public int IDCorresponsal { get; set; }
    }

    public class LogError
    {
        public DateTime Fecha { get; set; }
        public int IDTrsansaccion { get; set; }
        public string Operacion { get; set; }
        public string Error { get; set; }
    }

    public class LogErrorMethods
    {
        public int IDError { get; set; }
        public string NameClass { get; set; }
        public string NameMethod { get; set; }
        public string Message { get; set; }
        public string Fecha { get; set; }
        public int IDCorresponsal { get; set; }

        public void CreateLogsMethods(LogErrorMethods log)
        {
            var json = JsonConvert.SerializeObject(log);
            string fullPath = string.Format(@"C:\LogsMetodosCCM\");
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            string path2 = string.Concat(log.Fecha, "-", log.NameMethod, ".json");
            var nameFile = Path.Combine(fullPath, path2);
            if (!File.Exists(nameFile))
            {
                var archivo = File.CreateText(nameFile);
                archivo.Close();
            }

            using (StreamWriter sw = File.AppendText(nameFile))
            {
                sw.WriteLine(json);
            }
        }
    }
}
