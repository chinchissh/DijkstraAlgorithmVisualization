using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DijkstraAlgorithmVisualization
{
    public partial class Form1 : Form
    {
        private const int CircleRadius = 20;
        private const int CircleDiameter = CircleRadius * 2;
        private const int NodeFontSize = 12;
        private const int EdgeFontSize = 10;
        private const int Inf = int.MaxValue;

        private int numNodes;
        private List<Point> nodePositions;
        private List<List<int>> adjacencyMatrix;
        private List<int> distances;
        private List<int> previous;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numNodes = 0;
            nodePositions = new List<Point>();
            adjacencyMatrix = new List<List<int>>();
            distances = new List<int>();
            previous = new List<int>();
        }

        private void GenerateGraphButton_Click(object sender, EventArgs e)
        {
            // Получаем количество узлов из текстового поля
            if (!int.TryParse(numNodesTextBox.Text, out numNodes))
            {
                MessageBox.Show("Invalid number of nodes.");
                return;
            }

            // Создаем пустую матрицу смежности
            adjacencyMatrix = new List<List<int>>();
            for (int i = 0; i < numNodes; i++)
            {
                adjacencyMatrix.Add(new List<int>());
                for (int j = 0; j < numNodes; j++)
                {
                    adjacencyMatrix[i].Add(Inf);
                }
            }

            // Вычисляем позиции для отображения узлов на форме
            CalculateNodePositions();

            // Генерируем случайный граф
            GenerateRandomGraph();

            // Обновляем PictureBox
            graphPictureBox.Refresh();
        }

        private void GraphPictureBox_Paint(object sender, PaintEventArgs e)
        {
            // Отрисовка ребер
            DrawEdges(e.Graphics);

            // Отрисовка узлов
            DrawNodes(e.Graphics);
        }

        private void DrawNodes(Graphics g)
        {
            Font font = new Font("Arial", NodeFontSize, FontStyle.Bold);

            for (int i = 0; i < numNodes; i++)
            {
                int x = nodePositions[i].X - CircleRadius;
                int y = nodePositions[i].Y - CircleRadius;

                // Рисуем кружок для узла
                g.DrawEllipse(Pens.Black, x, y, CircleDiameter, CircleDiameter);

                // Рисуем номер узла
                string nodeNumber = (i + 1).ToString();
                SizeF textSize = g.MeasureString(nodeNumber, font);
                float textX = nodePositions[i].X - textSize.Width / 2;
                float textY = nodePositions[i].Y - textSize.Height / 2;
                g.DrawString(nodeNumber, font, Brushes.Black, textX, textY);
            }
        }

        private void DrawEdges(Graphics g)
        {
            Font font = new Font("Arial", EdgeFontSize);

            for (int i = 0; i < numNodes; i++)
            {
                for (int j = 0; j < numNodes; j++)
                {
                    int weight = adjacencyMatrix[i][j];

                    // Пропускаем бесконечные ребра
                    if (weight == Inf)
                        continue;

                    Point p1 = nodePositions[i];
                    Point p2 = nodePositions[j];

                    // Рисуем ребро
                    Pen pen = (i == j) ? Pens.Red : Pens.Black;
                    g.DrawLine(pen, p1, p2);

                    // Рисуем вес ребра
                    string weightString = weight.ToString();
                    SizeF textSize = g.MeasureString(weightString, font);
                    float textX = (p1.X + p2.X) / 2 - textSize.Width / 2;
                    float textY = (p1.Y + p2.Y) / 2 - textSize.Height / 2;
                    g.DrawString(weightString, font, Brushes.Black, textX, textY);
                }
            }
        }

        private void CalculateNodePositions()
        {
            nodePositions = new List<Point>();
            int centerX = graphPictureBox.Width / 2;
            int centerY = graphPictureBox.Height / 2;

            double angleIncrement = 2 * Math.PI / numNodes;
            double currentAngle = 0;

            for (int i = 0; i < numNodes; i++)
            {
                int x = (int)(centerX + Math.Cos(currentAngle) * (graphPictureBox.Width / 2 - CircleDiameter));
                int y = (int)(centerY + Math.Sin(currentAngle) * (graphPictureBox.Height / 2 - CircleDiameter));
                nodePositions.Add(new Point(x, y));
                currentAngle += angleIncrement;
            }
        }

        private void GenerateRandomGraph()
        {
            Random random = new Random();

            for (int i = 0; i < numNodes; i++)
            {
                for (int j = i + 1; j < numNodes; j++)
                {
                    int weight = random.Next(1, 10);

                    // Добавляем ребро в обе стороны
                    adjacencyMatrix[i][j] = weight;
                    adjacencyMatrix[j][i] = weight;
                }
            }
        }

        private void DijkstraButton_Click(object sender, EventArgs e)
        {
            // Инициализируем начальные значения
            distances = new List<int>();
            previous = new List<int>();
            for (int i = 0; i < numNodes; i++)
            {
                distances.Add(Inf);
                previous.Add(-1);
            }
            distances[0] = 0;

            // Список для хранения посещенных узлов
            List<int> visited = new List<int>();

            // Находим кратчайший путь для каждого узла
            for (int i = 0; i < numNodes; i++)
            {
                // Находим узел с минимальным расстоянием, который еще не был посещен
                int minDistance = Inf;
                int minDistanceNode = -1;
                for (int j = 0; j < numNodes; j++)
                {
                    if (!visited.Contains(j) && distances[j] < minDistance)
                    {
                        minDistance = distances[j];
                        minDistanceNode = j;
                    }
                }

                // Посещаем выбранный узел
                visited.Add(minDistanceNode);

                // Обновляем расстояния до соседних узлов
                for (int j = 0; j < numNodes; j++)
                {
                    if (!visited.Contains(j) && adjacencyMatrix[minDistanceNode][j] != Inf)
                    {
                        int newDistance = distances[minDistanceNode] + adjacencyMatrix[minDistanceNode][j];
                        if (newDistance < distances[j])
                        {
                            distances[j] = newDistance;
                            previous[j] = minDistanceNode;
                        }
                    }
                }

                // Обновляем PictureBox
                graphPictureBox.Refresh();
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
            }

            // Отображаем кратчайший путь до каждого узла
            ShowShortestPaths();
        }

        private void ShowShortestPaths()
        {
            Font font = new Font("Arial", EdgeFontSize, FontStyle.Bold);

            for (int i = 1; i < numNodes; i++)
            {
                int currentNode = i;
                while (currentNode != 0)
                {
                    int previousNode = previous[currentNode];

                    // Отрисовка кратчайшего пути
                    Point p1 = nodePositions[previousNode];
                    Point p2 = nodePositions[currentNode];
                    graphPictureBox.CreateGraphics().DrawLine(Pens.Blue, p1, p2);

                    // Отрисовка веса ребра на пути
                    int weight = adjacencyMatrix[previousNode][currentNode];
                    string weightString = weight.ToString();
                    SizeF textSize = graphPictureBox.CreateGraphics().MeasureString(weightString, font);
                    float textX = (p1.X + p2.X) / 2 - textSize.Width / 2;
                    float textY = (p1.Y + p2.Y) / 2 - textSize.Height / 2;
                    graphPictureBox.CreateGraphics().DrawString(weightString, font, Brushes.Black, textX, textY);

                    currentNode = previousNode;
                }
            }
        }
    }
}

