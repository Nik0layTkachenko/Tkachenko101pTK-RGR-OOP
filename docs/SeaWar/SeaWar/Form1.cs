using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaWar
{

	using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

    public class Bot
    {
        public int[,] myMap = new int[Form1.mapSize, Form1.mapSize];
        public int[,] enemyMap = new int[Form1.mapSize, Form1.mapSize];

        // Ці масиви зберігають інформацію про кноопки 
        public Button[,] myButtons = new Button[Form1.mapSize, Form1.mapSize];
        public Button[,] enemyButtons = new Button[Form1.mapSize, Form1.mapSize];

        public Bot(int[,] myMap, int[,] enemyMap, Button[,] myButtons, Button[,] enemyButtons)
        {
            this.myMap = myMap;
            this.enemyMap = enemyMap;
            this.myButtons = myButtons;
            this.enemyButtons = enemyButtons;
        }

        // Функція потрібна для того щоб передбачити вихід за кордони ігрового поля
        public bool IsInsideMap(int i, int j)
        {
            if (i < 0 || j < 0 || i >= Form1.mapSize || j >= Form1.mapSize)
            {
                return false;
            }
            return true; 
        }

        public bool IsEmpty(int i, int j, int length)
        {
            bool isEmpty = true;

            for (int k = j; k < j + length; k++)
            {
                if (myMap[i, k] != 0)
                {
                    isEmpty = false;
                    break;
                }
            }

            return isEmpty;
        }

        // Створює кораблі на карті ворога
        public int[,] ConfigureShips()
        {
            int lengthShip = 4;
            int cycleValue = 4;
            int shipsCount = 10;
            Random r = new Random();

            int posX = 0;
            int posY = 0;

            while (shipsCount > 0)
            {
                for (int i = 0; i < cycleValue / 4; i++)
                {
                    posX = r.Next(1, Form1.mapSize);
                    posY = r.Next(1, Form1.mapSize);

                    // Цикл створює нові координати корабля, якщо попередні були створенні не коректно
                    while (!IsInsideMap(posX, posY + lengthShip - 1) || !IsEmpty(posX, posY, lengthShip))
                    {
                        posX = r.Next(1, Form1.mapSize);
                        posY = r.Next(1, Form1.mapSize);
                    }
                    for (int k = posY; k < posY + lengthShip; k++)
                    {
                        myMap[posX, k] = 1;
                    }



                    shipsCount--;
                    if (shipsCount <= 0)
                        break;
                }
                cycleValue += 4;
                lengthShip--;
            }
            return myMap;
        }

        public bool Shoot()
        {
            bool hit = false;
            
            Random random = new Random();

            int posX = random.Next(1, Form1.mapSize);   
            int posY = random.Next(1, Form1.mapSize);
            
            // Цикл потрібен для того щоб бот не стріляв в одне
            while (enemyButtons[posX, posY].BackColor == Color.Blue || enemyButtons[posX, posY].BackColor == Color.Black)
            {
                posX = random.Next(1, Form1.mapSize);
                posY = random.Next(1, Form1.mapSize);
            }

            // Клітинка прі влученні в неї
            if (enemyMap[posX, posY] != 0)
            {
                hit = true;

                enemyMap[posX, posY] = 0;
                enemyButtons[posX, posY].BackColor = Color.Blue;
                enemyButtons[posX, posY].Text = "X";
            }
            else
            {
                hit = false;

                enemyButtons[posX, posY].BackColor = Color.Black;
            }

            // Якщо бот влучив – він стріляє ще раз
            if (hit)
                Shoot();

            return hit;
        }
    }

    public partial class Form1 : Form
    {

        public const int mapSize = 10;
        public int cellSize = 30;
        public string alphabet = "АБВГДЕЖЗИК";

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        // Ці масиви зберігають інформацію про кнопки
        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public Bot bot;

        bool isPlaying = false;
        public Form1()
        {
            this.Text = "Морський бій";
            InitializeComponent();
            Init();
        }

        public void Init()
        {

            isPlaying = false; 
            CreatMap();
            bot = new Bot(enemyMap, myMap, enemyButtons, myButtons);
            enemyMap = bot.ConfigureShips();
        }

        public void CreatMap()
        {
            // Розмір Form1
            this.Width = mapSize * 2 * cellSize + 40; 
            this.Height = (mapSize+1) * cellSize + 50;

            // Наше поле
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
     
                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.Gray;
                        if (i == 0 && j > 0 )
                        {
                            button.Text = alphabet[j - 1].ToString();
                        }
                        if (j == 0 && i > 0)
                        {
                            button.Text = i.ToString();
                        }
                    }
                    else
                    {
                        button.Click += new EventHandler(ConfigureShips);
                    }
                    myButtons[i,j] = button;
                    this.Controls.Add(button);
                }
            }

            // Вороже поле
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;
                    enemyMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(320 + j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;

                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.Gray;
                        if (i == 0 && j > 0 )
                        {
                            button.Text = alphabet[j - 1].ToString();
                        }
                        if (j == 0 && i > 0)
                        {
                            button.Text = i.ToString();
                        }
                    }
                    else
                    {
                        // Присвоюєм ворожим кнопкам обробщик  playerShoot
                        button.Click += new EventHandler(playerShoot);
                    }
                    enemyButtons[i,j] = button;
                    this.Controls.Add(button);
                }
            }
            Label map1 = new Label();
            map1.Text = "Мапа гравця";
            map1.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(map1);

            Label map2 = new Label();
            map2.Text = "Мапа ворога";
            map2.Location = new Point(350 + mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(map2);

            // Кнопка якою ми починаємо гру
            Button startButton = new Button();
            startButton.Text = "Почати";
            startButton.Click += new EventHandler(Start);
            startButton.Location = new Point(300, mapSize * cellSize + 10);
            this.Controls.Add(startButton);
        }

        // Функція відповідає за старт гри
        public void Start(object sender, EventArgs e)
        {
            isPlaying = true;
        }

        // Функція перевіряє карти на наявність кораблів
        public bool ChackIfMapIsNotEmpty()
        {
            bool isEmpty1 = true;
            bool isEmpty2 = true;

            for (int i = 1; i < mapSize; i++)
            {
                for (int j = 1; j < mapSize; j++)
                {
                    if (myMap[i, j] != 0)
                        isEmpty1 = false;
                    if (enemyMap[i, j] != 0)
                        isEmpty2 = false;
                }
            }

            if (isEmpty1 || isEmpty2)
                return false;
            else return true;
        }

        // Пводження кнопок при натисканнях
        public void ConfigureShips(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            if (!isPlaying)
            {
                if (myMap[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] == 0)
                {
                    pressedButton.BackColor = Color.Red;
                    myMap[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = 1;
                }
                else
                {
                    pressedButton.BackColor = Color.White;
                    myMap[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = 0;
                }
            }
        }

        // Ця функція обробляє процес натискання кнопки
        public void playerShoot(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            bool plyartTurn = Shoot(enemyMap, pressedButton);
            // Цей if відповідає за вогонь бота, коли ми стріляємо – бот стріляє у відповідь
            if (!plyartTurn)
                bot.Shoot();

            // Почати игру заново 
            if (!ChackIfMapIsNotEmpty())
            {
                this.Controls.Clear();
                Init();
            }
        }

        // В цій функції знаходиться логіка вогонь
        public bool Shoot(int[,] map, Button pressedButton)
        {
            bool hit = false;

            if (isPlaying)
            { 
                int delta = 0;
                // Цей if відповідає за заміщення постріла на мапу ворога
                if (pressedButton.Location.X > 320)
                {
                    delta = 320;
                }
                if (map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] != 0)
                {
                    pressedButton.BackColor = Color.Blue;
                    pressedButton.Text = "X";
                    map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] = 0;

                    hit = true;

                }
                else
                {
                    pressedButton.BackColor = Color.Black;

                    hit = false;
                }
            }

            return hit;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    
    }

}
