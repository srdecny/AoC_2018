using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public List<Point> points;
        public int iteration = 10117;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PointChart.Series.Add("foo");
            PointChart.Series["foo"].ChartType = SeriesChartType.Point;
            PointChart.Series["foo"].XAxisType = AxisType.Primary;
            List<Point> parsedPoints = new List<Point>();
            char[] delimiters = "position=< ,> velocity=<".ToCharArray();

            using (StreamReader file = new StreamReader(File.Open(@"C:\Users\Vojta\Documents\input.txt", FileMode.Open)))
            {
                while (!file.EndOfStream)
                {
                    string line = file.ReadLine();
                    var s = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    List<int> coords = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToList();
                    parsedPoints.Add(new Point(coords[0], coords[1], coords[2], coords[3]));
                }
            }

            points = parsedPoints;
        }

        private async void NextIteration_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 1; i++)
            {
                PointChart.Series["foo"].Points.Clear();
                foreach (Point point in points)
                {
                    (int, int) newCoords = ValueTuple.Create(point.StartPosition.Item1 + point.Velocity.Item1 * iteration,
                                                                point.StartPosition.Item2 + point.Velocity.Item2 * iteration);
                    PointChart.Series["foo"].Points.AddXY(newCoords.Item1, newCoords.Item2);
                }
                await Task.Delay(5);
                iteration++;
            }
        }
    }

    public class Point
    {
        public (int, int) StartPosition { get; set; }
        public (int, int) Velocity { get; set; }

        public Point(int startX, int startY, int veloX, int veloY)
        {
            StartPosition = ValueTuple.Create(startX, startY);
            Velocity = ValueTuple.Create(veloX, veloY);
        }
    }
}
