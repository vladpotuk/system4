using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace Wpf2
{
    public partial class MainWindow : Window
    {
        private int[] numbers;
        private int max, min;
        private double average;
        private readonly object lockObject = new object();
        private readonly string filePath = "results.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGenerateAndCalculate_Click(object sender, RoutedEventArgs e)
        {
            numbers = GenerateNumbers(10000);
            lstNumbers.Items.Clear();

            Thread maxThread = new Thread(FindMax);
            Thread minThread = new Thread(FindMin);
            Thread averageThread = new Thread(FindAverage);
            Thread writeThread = new Thread(WriteResultsToFile);

            maxThread.Start();
            minThread.Start();
            averageThread.Start();
            writeThread.Start();

            maxThread.Join();
            minThread.Join();
            averageThread.Join();
            writeThread.Join();

            DisplayResults();
        }

        private int[] GenerateNumbers(int count)
        {
            Random rand = new Random();
            int[] numbers = new int[count];
            for (int i = 0; i < count; i++)
            {
                numbers[i] = rand.Next(1, 10001);
                lstNumbers.Items.Add(numbers[i]);
            }
            return numbers;
        }

        private void FindMax()
        {
            int maxVal = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                if (numbers[i] > maxVal)
                {
                    maxVal = numbers[i];
                }
            }
            lock (lockObject)
            {
                max = maxVal;
            }
        }

        private void FindMin()
        {
            int minVal = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                if (numbers[i] < minVal)
                {
                    minVal = numbers[i];
                }
            }
            lock (lockObject)
            {
                min = minVal;
            }
        }

        private void FindAverage()
        {
            double sum = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                sum += numbers[i];
            }
            lock (lockObject)
            {
                average = sum / numbers.Length;
            }
        }

        private void WriteResultsToFile()
        {
            lock (lockObject)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Набір чисел:");
                    foreach (int number in numbers)
                    {
                        writer.WriteLine(number);
                    }
                    writer.WriteLine();
                    writer.WriteLine($"Максимум: {max}");
                    writer.WriteLine($"Мінімум: {min}");
                    writer.WriteLine($"Середнє арифметичне: {average}");
                }
            }
        }

        private void DisplayResults()
        {
            txtMax.Text = $"Максимум: {max}";
            txtMin.Text = $"Мінімум: {min}";
            txtAverage.Text = $"Середнє арифметичне: {average}";
        }
    }
}
