using System;
using System.Linq;
using HumanDateParser;

namespace HumanDateParser.Prompt
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new HumanDateParser();
            while (true)
            {
                Console.Write("Enter a string: ");
                var val = Console.ReadLine();
                try
                {
                    var d = parser.DetailedParse(val);
                    Console.WriteLine($"Result: {d.Result}");
                    Console.WriteLine($"Tokens: {string.Join(" ", d.Tokens.Select(c => c.GetType().Name))}");
                } catch (ParseException pe)
                {
                    Console.WriteLine($"ParseException ({pe.FailReason}): {pe.Message}");
                } catch (Exception e)
                {
                    Console.WriteLine($"Unexpected {e.GetType().Name}: {e.Message}");
                }
            }
        }
    }
}
