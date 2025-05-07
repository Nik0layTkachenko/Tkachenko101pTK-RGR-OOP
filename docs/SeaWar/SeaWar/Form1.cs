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
    public partial class Form1 : Form
    {

        public const int mapSize = 10;
        public int cellSize = 30;
        public string alphabet = "АБВГДЕЖЗИК";

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        // Эти масивы хранят информацию о кнопках 
        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public Bot bot;

        bool isPlaying = false;
        public Form1()
        {
            this.Text = "Морской бой";
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
            // Розмер Form1
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

            // Вражеское поле
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
                        // Присваиваем вражеским кнопкам оброботчик playerShoot
                        button.Click += new EventHandler(playerShoot);
                    }
                    enemyButtons[i,j] = button;
                    this.Controls.Add(button);
                }
            }
            Label map1 = new Label();
            map1.Text = "Карта игрока";
            map1.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(map1);

            Label map2 = new Label();
            map2.Text = "Карта врага";
            map2.Location = new Point(350 + mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(map2);

            // Кнопка которой мы наченаем игру
            Button startButton = new Button();
            startButton.Text = "Начать";
            startButton.Click += new EventHandler(Start);
            startButton.Location = new Point(300, mapSize * cellSize + 10);
            this.Controls.Add(startButton);
        }

        // Функцыя отвечает за Старт игры
        public void Start(object sender, EventArgs e)
        {
            isPlaying = true;
        }

        // Функцыя проверяет карты на наличие кораблей 
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

        // Повидение кнопок при нажатии 
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

        // Эта функция обробатывает событие нажатие кнопки
        public void playerShoot(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            bool plyartTurn = Shoot(enemyMap, pressedButton);
            // Этот if отвечает за стрельбу бота, тоесть когда мы стреляем бот стреляет нам в ответ
            if (!plyartTurn)
                bot.Shoot();

            // Начинает игру заново 
            if (!ChackIfMapIsNotEmpty())
            {
                this.Controls.Clear();
                Init();
            }
        }

        // В этой функции находится логика стрельбы
        public bool Shoot(int[,] map, Button pressedButton)
        {
            bool hit = false;

            if (isPlaying)
            { 
                int delta = 0;
                // Этот if отвечает за смещения выстрела на карту врага 
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
