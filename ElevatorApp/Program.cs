namespace ElevatorApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var system = new ElevatorSystem(4);
            system.Run();
        }
    }
}
