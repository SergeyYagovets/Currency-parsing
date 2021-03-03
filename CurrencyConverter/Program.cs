using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace CurrencyConverter
{
    class Program
    {
        public class Neuron
        {
            private decimal weight = 0.1m; // Вес входящего нейрона.
            public decimal Error { get; private set; }
            public decimal Step { get; set; } = 0.1m;
            public decimal InputData(decimal input) // Получает входящий сигнал, конвертирует в исходящие данные.
            {
                return input * weight;
            }

            public decimal RestoreInputData(decimal output) // Выполняет обратный процесс конвертации.
            {
                return output / weight;
            }

            public void TrainNeuron(decimal input, decimal expectedResult) // Обучение нейрона.
            {
                var actualResult = input * weight;
                Error = expectedResult - actualResult;
                var correction = (Error / actualResult) * Step;
                weight += correction;
            }
        }

        static void Main(string[] args)
        {   
            WebClient client = new WebClient();
            var xml = client.DownloadString("http://www.cbr.ru/scripts/XML_daily.asp");
            XDocument xdoc = XDocument.Parse(xml);
            var element = xdoc.Element("ValCurs").Elements("Valute");
            string hKDdollar = element.Where(x => x.Attribute("ID").Value == "R01200")
                .Select(x => x
                .Element("Value").Value)
                .FirstOrDefault();

            Console.WriteLine($"{hKDdollar}RUB  Exchange rate of the Hong Kong dollar to the Russian ruble");
            Console.ReadLine();

            var hkd = Convert.ToDecimal(hKDdollar);
            decimal hongKongDollar = 1;
            Neuron neuron = new Neuron();

            int i = 0; // Счетчик итераций 
            do
            {
                i++;
                neuron.TrainNeuron(hongKongDollar, hkd); // Передаем точные данные HKD, ruble.
                Console.WriteLine($"Iteration: {i}\tError: \t{neuron.Error}"); // Вывод итераций и ошибок.

            } while (neuron.Error > neuron.Step || neuron.Error < -neuron.Step); // Сравнение значений и повтор цикла.

            Console.WriteLine("Training completed!");
            Console.WriteLine($"{neuron.InputData(1)}:RUB in HKD");
            Console.WriteLine($"{neuron.RestoreInputData(1)}:HKD in RUB");
            Console.ReadLine();
        }
    }
}

