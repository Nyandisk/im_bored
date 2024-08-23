using im_bored.snek_franchise;
using im_bored.tic_tac_toe;
using SFML.Graphics;
namespace im_bored{
    public class ImBored{
        private static readonly string ASSET_DIR = AppContext.BaseDirectory + @"assets\";
        private static readonly Texture MISSING = new(ASSET_DIR + "missing.png");
        public static Texture TryLoadTexture(string path){
            try{
                Texture texture = new(ASSET_DIR + path);
                return texture;
            }catch(SFML.LoadingFailedException){
                return MISSING;
            }
        }
        public static int AskChoice(string[] choices, string prompt = ""){
            while(true){
                if (prompt != "") Console.WriteLine(prompt);
                for (int i = 0; i < choices.Length; i++){
                    Console.WriteLine($"{i+1}. {choices[i]}");
                }
                Console.Write("> ");
                bool success = int.TryParse(Console.ReadLine()!,out int result);
                if (!success){
                    Console.WriteLine("Invalid input");
                    continue;
                }
                if (result < 1 || result > choices.Length){
                    Console.WriteLine("Invalid choice");
                    continue;
                }
                return result;
            }
        }
        public static void Main(string[] args){
            while(true){
                Console.WriteLine("i'm bored: v0.3.3\npress any key to enter menu...");
                Console.ReadKey();
                Console.Clear();
                switch(AskChoice([
                    "snek 1 (console)",
                    "snek 2 (sfml)\n = tic-tac-toe = ",
                    "tic-tac-toe (console)",
                    "dissolve tic-tac-toe (sfml & mp)\n = other = ",
                    "quit"
                    ],"choose something to do\n = snek franchise = ")){
                    case 1:
                        Snek snek1 = new();
                        snek1.Run();
                        break;
                    case 2:
                        Snek2 snek2 = new();
                        snek2.Run();
                        break;
                    case 3:
                        TTT ttt = new();
                        ttt.Run();
                        break;
                    case 4:
                        break;
                    case 5:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.Write("achievement get: ");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("how did we get here?");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Environment.Exit(69);
                        break;
                }
            }
        }
    }
}