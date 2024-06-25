using System;
using System.Collections.Generic;
using System.Drawing;

class Graph
{
    public Dictionary<int, List<int>> AdjacencyList { get; set; }

    public Graph()
    {
        AdjacencyList = new Dictionary<int, List<int>>();
    }

    public void AddEdge(int v1, int v2)
    {
        if (!AdjacencyList.ContainsKey(v1))
            AdjacencyList[v1] = new List<int>();
        if (!AdjacencyList.ContainsKey(v2))
            AdjacencyList[v2] = new List<int>();

        AdjacencyList[v1].Add(v2);
        AdjacencyList[v2].Add(v1);
    }
}

class GraphColoring
{
    public int[] ColorGraph(Dictionary<int, List<int>> graph)
    {
        int vertexCount = graph.Keys.Count;
        int[] result = new int[vertexCount];

        for (int i = 0; i < vertexCount; i++)
            result[i] = -1;

        result[0] = 0;

        bool[] available = new bool[vertexCount];

        for (int i = 1; i < vertexCount; i++)
        {
            foreach (int neighbor in graph[i + 1])
            {
                if (result[neighbor - 1] != -1)
                    available[result[neighbor - 1]] = true;
            }

            int color;
            for (color = 0; color < vertexCount; color++)
            {
                if (!available[color])
                    break;
            }

            result[i] = color;

            foreach (int neighbor in graph[i + 1])
            {
                if (result[neighbor - 1] != -1)
                    available[result[neighbor - 1]] = false;
            }
        }
        return result;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Структура смежности
        Graph graph = new Graph();
        int[][] edges = new int[][]
        {
            new int[] {1, 2},
            new int[] {1, 5},
            new int[] {2, 5},
            new int[] {3, 4},
            new int[] {3, 5},
            new int[] {3, 7},
            new int[] {4, 5},
            new int[] {5, 6},
            new int[] {6, 7}
        };

        foreach (var edge in edges)
        {
            graph.AddEdge(edge[0], edge[1]);
        }

        // "Жадный" алгоритм раскраски
        GraphColoring coloring = new GraphColoring();
        int[] colors = coloring.ColorGraph(graph.AdjacencyList);

        // Отобразить графически
        DrawGraph(graph.AdjacencyList, colors);
    }

    static void DrawGraph(Dictionary<int, List<int>> adjacencyList, int[] colors)
    {
        int width = 800;
        int height = 600;
        int margin = 50;
        int vertexRadius = 20;

        // Определение позиций для каждой вершины (для простоты, располагаем их по кругу)
        int vertexCount = adjacencyList.Keys.Count;
        PointF[] positions = new PointF[vertexCount];
        float angleStep = 360.0f / vertexCount;

        for (int i = 0; i < vertexCount; i++)
        {
            float angle = i * angleStep * (float)Math.PI / 180.0f;
            float x = width / 2 + (width / 2 - margin) * (float)Math.Cos(angle);
            float y = height / 2 + (height / 2 - margin) * (float)Math.Sin(angle);
            positions[i] = new PointF(x, y);
        }

        // Создание изображения
        Bitmap bitmap = new Bitmap(width, height);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Рисование ребер
            using (Pen pen = new Pen(Color.Black, 2))
            {
                foreach (var vertex in adjacencyList)
                {
                    foreach (var neighbor in vertex.Value)
                    {
                        PointF p1 = positions[vertex.Key - 1];
                        PointF p2 = positions[neighbor - 1];
                        g.DrawLine(pen, p1, p2);
                    }
                }
            }

            // Определение цветов
            Color[] colorPalette = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Magenta, Color.Cyan };

            // Рисование вершин
            for (int i = 0; i < vertexCount; i++)
            {
                PointF position = positions[i];
                Color vertexColor = colorPalette[colors[i] % colorPalette.Length];
                using (Brush brush = new SolidBrush(vertexColor))
                {
                    g.FillEllipse(brush, position.X - vertexRadius, position.Y - vertexRadius, vertexRadius * 2, vertexRadius * 2);
                }
                using (Pen pen = new Pen(Color.Black, 2))
                {
                    g.DrawEllipse(pen, position.X - vertexRadius, position.Y - vertexRadius, vertexRadius * 2, vertexRadius * 2);
                }
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString((i + 1).ToString(), new Font("Arial", 12), brush, position.X - 10, position.Y - 10);
                }
            }
        }

        // Сохранение изображения
        bitmap.Save("C:\\Users\\needs\\source\\repos\\ConsoleApp17\\graph.png");

        Console.WriteLine("Граф сохранен в файл 'graph.png'");
    }
}
