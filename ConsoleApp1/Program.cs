using System;
using System.IO;

namespace ConsoleApp1
{
	class Program
	{
		class fileComparison
		{
			private string sourceFile;
            private string targetFile;
            private string resultFile;

			public void comparison(string sourcePath, string targetPath)
			{

				string sourceFile = createStringInfo(sourcePath); // по введенному пути добирается до файла, считывает информацию и преобразует в string
                string targetFile = createStringInfo(targetPath); // аналогично для второго файла

                resultFile = createResultString(sourceFile, targetFile); // сравнение файлов и возвращает результирующую строку

                createNewFile(sourcePath, resultFile);
			}

			private string createStringInfo(string path) // метод отвечает за считывания информации с файла и преобразования в тип string 
            {
				byte[] output;
				using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
				{
					output = new byte[fstream.Length];
					fstream.Read(output, 0, output.Length); // откуда считывать, начиная с какого символа и сколько символов считывать 
				}
				return System.Text.Encoding.Default.GetString(output);
			}

            private string createResultString(string source, string target) // сравнение файлов
            {
                string result = "";
                int positionSource = 0, positioTarget = 0;
                int firstMatrix = createMatrix(source, target); // вычисляется начальная матрица сравнения 

                while (positionSource < source.Length && positioTarget < target.Length) // выполняется пока обе строки не пройдены 
                {
                    if (source[positionSource] == target[positioTarget]) // символы одинаковы
                    {
                        result += source[positionSource]; // запись в результирующею строку + одновременный переход ко следующим символам строк
                        positionSource++;
                        positioTarget++;
                    }
                    else
                    {
                        firstMatrix = createMatrix(source.Substring(positionSource), target.Substring(positioTarget)); // вычисление текущего состояния матриц ставнения (учитывая текущии позиции в строках)
                        int newMatrix = createMatrix(source.Substring(positionSource), target.Substring(positioTarget));

                        if (positionSource < source.Length - 1 && firstMatrix == createMatrix(source.Substring(positionSource + 1), target.Substring(positioTarget))) // вычисление новой матрицы ставнения с шагом  - positionSource + 1 
                        {
                            // если в результате шага количество одинаковых символов не изменилось - это значит что действие выбрано правильно 
                            string resultTMP = "";

                            while (firstMatrix == newMatrix && positionSource < source.Length) // если нужное действие выполняеться подряд 
                            {
                                resultTMP += source[positionSource++];
                                if (positionSource + 1 < source.Length) newMatrix = createMatrix(source.Substring(positionSource + 1), target.Substring(positioTarget)); // если под конец строки у каждой строки есть отличные (уникальные) элементы, то сработает данная ситуация, когда может нарушится диапазон строки 
                            }

                            result += "[" + resultTMP + "]";
                        }

                        newMatrix = createMatrix(source.Substring(positionSource), target.Substring(positioTarget)); // вычисление текущего состояния матрицы

                        // аналогичные действия, только уже для второй строки
                        if (positioTarget < target.Length - 1 && firstMatrix == createMatrix(source.Substring(positionSource), target.Substring(positioTarget + 1)))
                        {
                            string resultTMP = "";
                            while (firstMatrix == newMatrix && positioTarget < target.Length)
                            {
                                resultTMP += target[positioTarget++];
                                if (positioTarget + 1 < target.Length) newMatrix = createMatrix(source.Substring(positionSource), target.Substring(positioTarget + 1));
                            }
                            result += "(" + resultTMP + ")";
                        }
                    }
                }

                // если оказалось что одна строка короче другой сработают данные проверки 
                if (positionSource < source.Length - 1) result += "[" + source.Substring(positionSource) + "]";
                if (positioTarget < target.Length - 1) result += "(" + target.Substring(positioTarget) + ")";

                return result;
            }

            private int createMatrix(string source, string target)
            {
                int row = source.Length + 1;
                int col = target.Length + 1;
                int[,] matrix = new int[row, col];

                for (int i = 0; i < row; i++) matrix[i, 0] = 0;
                for (int j = 0; j < col; j++) matrix[0, j] = 0;

                for (int i = 1; i < row; i++)
                {
                    for (int j = 1; j < col; j++)
                    {
                        if (source[i - 1] == target[j - 1]) matrix[i, j] = matrix[i - 1, j - 1] + 1;
                        else
                        {
                            if (matrix[i, j - 1] > matrix[i - 1, j]) matrix[i, j] = matrix[i, j - 1];
                            else matrix[i, j] = matrix[i - 1, j];
                        }
                    }
                }

                return matrix[row - 1, col - 1];
            }

            private void createNewFile(string allPath, string result) // метод записывает результирующую строку в созданный файл
            {
				string path = createPathForNewFile(allPath); // метод мозвращает путь к результирующему файлу

				using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
				{
					byte[] input = System.Text.Encoding.Default.GetBytes(result); // преобразования из string в byte

                    fstream.Write(input, 0, input.Length);

					Console.WriteLine("Текст записан в файл. Файл расположен по следующему пути: " + path);
				}
			}

			private string createPathForNewFile(string allPath) // метод описывает директорию + названия результирующего файла 
            {
				string path = "";

				for (int i = 0; i <= allPath.LastIndexOf("\\"); i++) path += allPath[i];

				return path += "result.txt";
			}
		}

		static void Main(string[] args)
		{
			string sourceFilePath = "";
			string targetFilePath = "";
			bool findFilePath = false;

			while (findFilePath == false)
			{
				Console.WriteLine("Введите путь к первому файлу: ");
				sourceFilePath = Console.ReadLine();
				try
				{
                    using (FileStream fstream = new FileStream(sourceFilePath, FileMode.Open)) { findFilePath = true; } // Попытка открыть файл для проверки его существования 
				}
				catch (FileNotFoundException) { Console.WriteLine("Ошибка! Файл не найдено."); }
				catch (DirectoryNotFoundException) { Console.WriteLine("Ошибка! Путь к файлу не найдено."); }
			}
			findFilePath = false;

			while (findFilePath == false)
			{
				Console.WriteLine("Введите путь к второму файлу: ");
				targetFilePath = Console.ReadLine();
				try
				{
					using (FileStream fstream = new FileStream(targetFilePath, FileMode.Open)) { findFilePath = true; } // Такая же проверка, как и для первого файла 
                }
				catch (FileNotFoundException) { Console.WriteLine("Ошибка! Файл не найдено."); }
				catch (DirectoryNotFoundException) { Console.WriteLine("Ошибка! Путь к файлу не найдено."); }
			}

			fileComparison ob = new fileComparison();
			ob.comparison(sourceFilePath, targetFilePath);
		}
	}
}
