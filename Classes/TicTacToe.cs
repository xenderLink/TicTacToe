namespace TicTacToePC;

public class TicTacToe
{
    private readonly char[,] startingBoard = { { '1', '2', '3' }, { '4', '5', '6' }, { '7', '8', '9' } }; 

    private readonly char player = 'X', AI = 'O';
    
    private short firstAiMove;     // первый ход AI и попытка занять середину или углы поля

    public void Start()
    {
        Console.Clear();

        Console.WriteLine("\nIn order to start the game input \"s\" or \"e\" to leave it.");
        
        while (true)
        {
            var input = Console.ReadLine()[0];

            if (input is 's')
            {
                Play();
                break;
            }
            else if (input is 'e')
            {
                return;
            }
            else
            {
                Console.WriteLine("Wrong input. Input \"s\" or \"e\"");
            }
        }

        while (true)
        {
            Console.WriteLine("\nDo you wanna play one more time? [y/n]");

            var input = Console.ReadLine()[0];

            if (Char.ToUpper(input) == 'Y')
                Play();

            else if (Char.ToUpper(input) == 'N')
                return;
            
            else 
                Console.WriteLine("Wrong input. Input \"y\" or \"n\"");  
        }
    }

    private void Play()
    {
        var playingBoard = new char[3, 3];
        firstAiMove = 0;
        short movesIteration = 1;   // пропускать проверки первые два хода

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                playingBoard[i, j] = startingBoard[i, j];
            }
        }

        Console.Clear();

        Console.WriteLine(" _______________________ ");
        Console.WriteLine("|       |       |       |");
        Console.WriteLine("|   {0}   |   {1}   |   {2}   |", startingBoard[0, 0], startingBoard[0, 1], startingBoard[0, 2]);
        Console.WriteLine("|_______|_______|_______|");
        Thread.Sleep(TimeSpan.FromSeconds(1.5));

        Console.WriteLine("|       |       |       |");
        Console.WriteLine("|   {0}   |   {1}   |   {2}   |", startingBoard[1, 0], startingBoard[1, 1], startingBoard[1, 2]);
        Console.WriteLine("|_______|_______|_______|");
        Thread.Sleep(TimeSpan.FromSeconds(1.5));

        Console.WriteLine("|       |       |       |");
        Console.WriteLine("|   {0}   |   {1}   |   {2}   |", startingBoard[2, 0], startingBoard[2, 1], startingBoard[2, 2]);
        Console.WriteLine("|_______|_______|_______|");

        while (true)
        {           
            PlayerMove(playingBoard);                     // ход игрока

            if (movesIteration >= 2)
            {
                if (CheckCells(playingBoard) is false)    // ничья
                {
                    Console.Clear();
                    Console.WriteLine("\nDraw!");
                    
                    PrintBoard(playingBoard);
                    Thread.Sleep(TimeSpan.FromSeconds(4));

                    return;
                }

                if (CheckWin(playingBoard) is -1)         // победа игрока
                {
                    Console.Clear();
                    Console.WriteLine("\nPlayer wins!");

                    PrintBoard(playingBoard);
                    Thread.Sleep(TimeSpan.FromSeconds(4));

                    return;
                }
            }

            AiMove(playingBoard);                         // ход AI

            if (movesIteration >= 2)
            {
                if (CheckWin(playingBoard) is 1)
                {
                    Console.Clear();
                    Console.WriteLine("\nAI wins!");      // победа AI

                    PrintBoard(playingBoard);
                    Thread.Sleep(TimeSpan.FromSeconds(4));

                    return;
                }             
            }
            
            PrintBoard(playingBoard);
                                    
            movesIteration++;
        }
    }
    
    // метод для проверки ввода игрока и если ввод удовлетворяет условиям, то делается ход за игрока
    private void PlayerMove(char[,] board)
    {
        while(true)
        {
            try
            {
                Console.WriteLine("\nPlayer is choosing cell:");
                var input = Console.ReadLine();
                var cell = Int32.Parse(input) - 1;
                
                if (cell < 0 || cell > 8)
                {
                    Console.WriteLine($"Wrong input. Player should input number from 1 to 9"); 
                    continue;
                }

                if (cell <= 2)
                {
                    if (board[0, cell] is 'X' || board[0, cell] is 'O')
                    {
                        Console.WriteLine($"Wrong input. Cell is busy. Choose another one:");
                        continue;
                    }

                    board[0, cell] = player;
                    break;
                }
                else if (cell <= 5)
                {
                    int idx = cell - 3;

                    if (board[1, idx] is 'X' || board[1, idx] is 'O')
                    {
                        Console.WriteLine($"Wrong input. Cell is busy. Choose another one:");
                        continue;
                    }

                    board[1, idx] = player;
                    break;
                }
                else if (cell <= 8)
                {
                    int idx = cell - 6;

                    if (board[2, idx] is 'X' || board[2, idx] is 'O')
                    {
                        Console.WriteLine($"Wrong input. Cell is busy. Choose another one:");
                        continue;
                    }

                    board[2, idx] = player;
                    break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine($"Wrong input. You should input number from 1 to 9");
                continue;                
            }
        }
    }

    private void AiMove(char[,] board)
    {
        if (firstAiMove is 0)
        {
            firstAiMove++;

            // пробует занять середину поля
            if (board[1, 1] is not 'X')
            {
                board[1, 1] = AI;
            }
            // если нет, то наугад выбирает один из 4 углов поля
            else
            {
                Random rand = new ();
                
                switch (rand.Next(1, 5))
                {
                    case 1:
                        board[0, 0] = AI;
                        break;

                    case 2:
                        board[0, 2] = AI;
                        break;

                    case 3:
                        board[2, 0] = AI;
                        break;

                    case 4:
                        board[2, 2] = AI;
                        break;
                }
            }

            return;
        }

        Move move = new ();

        int bestVal = -1000;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] != 'X' && board[i, j] != 'O') // ищет пустые клетки
                {
                    var temp = board[i, j];                   // записывает номер клетки

                    board[i, j] = AI;                         // ставит нулик

                    int moveVal = MiniMax(board, 0, false);   // false - ход игрока, 0 - глубина возможных вариантов

                    board[i, j] = temp;                       // "откатывает" нулик и записывает номер клетки

                    if (moveVal > bestVal)                    // проверяет лучший вариант хода для AI
                    {
                        bestVal = moveVal;
                        move.row = i;                         // и после нахождения записывает его
                        move.col = j;
                    }                      
                }
            }
        }

        board[move.row, move.col] = AI;                       // делает ход за AI
    }

    private int CheckWin(char[,] board)
    {
        // ряды
        for (int row = 0; row < 3; row++)
        {
            if (board[row, 0] == board[row, 1] &&
                board[row, 1] == board[row, 2])
            {
                if (board[row, 0] == player)
                    return -1;

                else if (board[row, 0] == AI)
                    return  1;
            }
        }

        // колонки
        for (int col = 0; col < 3; col++)
        {
            if (board[0, col] == board[1, col] &&
                board[1, col] == board[2, col])
            {
                if (board[0, col] == player)
                    return -1;

                else if (board[0, col] == AI)
                    return  1;
            }
        }

        // дигональ верхний левый-нижний правый
        if (board[0, 0] == board[1, 1] &&
            board[1, 1] == board[2, 2])
        {
            if (board[0, 0] == player)
                return -1;

            else if (board[0, 0] == AI)
                return  1; 
        }

        // дигональ верхний правый-нижний левый
        if (board[0, 2] == board[1, 1] &&
            board[1, 1] == board[2, 0])
        {
            if (board[0, 2] == player)
                return -1;

            else if (board[0, 2] == AI)
                return  1; 
        }

        // нет совпадений по клеткам
        return 0;
    }


    private int MiniMax(char[,] board, int depth, bool isMax)
    {
        int winCon = CheckWin(board); // исход на данном этапе

        // чем меньше глубина, тем больше очков за удачный ход
        // очки за удачный исход AI
        if (winCon is 1)
            return +10 - depth;  

        // очки за удачный исход игрока    
        if (winCon is -1)
            return -10 + depth;
        
        // проверяет наличие свободых клеток
        if (CheckCells(board) is false)
            return 0;

        // ход AI    
        if (isMax)
        {
            int bestScore = -1000;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] != 'X' && board[i, j] != 'O') // ищет пустые клетки
                    {
                        var temp = board[i, j];                   // записывает номер клетки

                        board[i, j] = AI;                         // ставит нулик

                        bestScore = Math.Max(bestScore, MiniMax(board, depth + 1, false)); 

                        board[i, j] = temp;                       // "откатывает" нулик и записывает номер клетки
                    }
                }
            }

            return bestScore;
        }
        // ход игрока
        else
        {
            int bestScore = 1000;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] != 'X' && board[i, j] != 'O') // ищет пустые клетки
                    {
                        var temp = board[i, j];                   // записывает номер клетки

                        board[i, j] = player;                     // ставит крестик

                        bestScore = Math.Min(bestScore, MiniMax(board, depth + 1, true)); 

                        board[i, j] = temp;                       // "откатывает" крестик и записывает номер клетки
                    }
                }
            }

            return bestScore;
        }
    }

    // метод для проверки наличия свободных клеток
    private bool CheckCells (char[,] board)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] is not 'X' && board[i, j] is not 'O')
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void PrintBoard(char[,] board)
    {
        Console.WriteLine(" _______________________ ");
        Console.WriteLine("|       |       |       |");
        Console.WriteLine("|   {0}   |   {1}   |   {2}   |", board[0, 0], board[0, 1], board[0, 2]);
        Console.WriteLine("|_______|_______|_______|");
        Console.WriteLine("|       |       |       |");
        Console.WriteLine("|   {0}   |   {1}   |   {2}   |", board[1, 0], board[1, 1], board[1, 2]);
        Console.WriteLine("|_______|_______|_______|");
        Console.WriteLine("|       |       |       |");
        Console.WriteLine("|   {0}   |   {1}   |   {2}   |", board[2, 0], board[2, 1], board[2, 2]);
        Console.WriteLine("|_______|_______|_______|");
    }
}