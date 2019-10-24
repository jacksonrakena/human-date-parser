using System;
using System.Linq;
using HumanDateParser;

namespace HumanDateParser.Prompt
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Input:");
                var val = Console.ReadLine();
                try
                {
                    var d = HumanDateParser.ParseDetailed(val);
                    Console.WriteLine($"Result: {d.Result}");
                    Console.WriteLine($"Tokens: {string.Join(" ", d.Tokens.Select(c => c.GetType().Name))}");
                } catch (ParseException pe)
                {
                    Console.WriteLine($"ParseException ({pe.FailReason}): {pe.Message}");
                }
            }
        }
    }
}
